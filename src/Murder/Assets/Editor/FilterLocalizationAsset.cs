using Bang;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Assets.Localization;
using Murder.Attributes;
using Murder.Data;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Murder.Editor.Assets;

public class AssetInfoPropertiesForEditor
{
    public bool IsSelected = true;
    public ImmutableArray<AssetPropertiesForEditor> Assets = [];

    public AssetInfoPropertiesForEditor() { }
}

public class AssetPropertiesForEditor
{
    public Guid Guid = Guid.Empty;
    public bool Show = false;
    public string Name = string.Empty;

    [JsonConstructor]
    public AssetPropertiesForEditor(Guid guid, string name)
    {
        Guid = guid;
        Name = name;

        Show = true;
    }
}

public class FilteredAssetsForLocalization
{
    [Serialize]
    private ImmutableArray<(string, AssetInfoPropertiesForEditor)>? _localizationCandidates = null;

    public ImmutableArray<(string, AssetInfoPropertiesForEditor)> GetLocalizationCandidates()
    {
        if (_localizationCandidates is null)
        {
            InitializeLocalizationCandidates();
        }

        return _localizationCandidates.Value;
    }

    [MemberNotNull(nameof(_localizationCandidates))]
    private void InitializeLocalizationCandidates()
    {
        Dictionary<string, AssetInfoPropertiesForEditor> builder = [];
        foreach (GameAsset asset in Game.Data.GetAllAssets())
        {
            if (asset is LocalizationAsset or SpriteAsset or SpriteEventDataManagerAsset or
                FontAsset or FloorAsset)
            {
                continue;
            }

            if (asset.Name.StartsWith(GameDataManager.SKIP_CHAR))
            {
                continue;
            }

            string type = asset.GetType().Name;
            if (!builder.TryGetValue(type, out AssetInfoPropertiesForEditor? info))
            {
                info = new();
                builder[type] = info;
            }

            info.Assets = info.Assets.Add(new(asset.Guid, asset.Name));
        }

        var candidates = ImmutableArray.CreateBuilder<(string, AssetInfoPropertiesForEditor)>();
        foreach ((string type, AssetInfoPropertiesForEditor list) in builder)
        {
            list.Assets = list.Assets.Sort((a, b) => a.Name.CompareTo(b.Name));

            candidates.Add((type, list));
        }

        candidates.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        _localizationCandidates = candidates.ToImmutable();
    }

    public void Clear()
    {
        _localizationCandidates = null;
    }
}

[HideInEditor] // This is created by the engine and should never be actually exposed to the UI.
public class FilterLocalizationAsset : GameAsset
{
    /// <summary>
    /// Use <see cref="GameDataManager.SKIP_CHAR"/> to hide this in the editor.
    /// </summary>
    public override string EditorFolder => GameDataManager.HiddenAssetsRelativePath;

    public static string DefaultFilterName = "all";

    public ImmutableDictionary<string, FilteredAssetsForLocalization> Filters =
        ImmutableDictionary<string, FilteredAssetsForLocalization>.Empty;

    public ImmutableArray<(string, AssetInfoPropertiesForEditor)> GetLocalizationCandidates(string name)
    {
        if (!Filters.TryGetValue(name, out FilteredAssetsForLocalization? filtered))
        {
            filtered = GetOrCreate(name);
        }

        return filtered.GetLocalizationCandidates();
    }

    public FilteredAssetsForLocalization GetOrCreate(string name)
    {
        if (Filters.TryGetValue(name, out FilteredAssetsForLocalization? filtered))
        {
            return filtered;
        }

        filtered = new();
        Filters = Filters.Add(name, filtered);

        return filtered;
    }

    public void Remove(string name)
    {
        Filters = Filters.Remove(name);
    }

    public void Clear()
    {
        foreach ((_, FilteredAssetsForLocalization f) in Filters)
        {
            f.Clear();
        }
    }

    public FilterLocalizationAsset() { }
}
