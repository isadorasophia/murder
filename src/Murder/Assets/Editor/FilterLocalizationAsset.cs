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

    [JsonConstructor]
    public AssetPropertiesForEditor(Guid guid)
    {
        Guid = guid;
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
        if (_localizationCandidates is not null)
        {
            return;
        }

        Dictionary<string, AssetInfoPropertiesForEditor> builder = [];
        foreach (GameAsset asset in Game.Data.GetAllAssets())
        {
            if (ShouldSkip(asset))
            {
                continue;
            }

            string type = asset.GetType().Name;
            if (!builder.TryGetValue(type, out AssetInfoPropertiesForEditor? info))
            {
                info = new();
                builder[type] = info;
            }

            info.Assets = info.Assets.Add(new(asset.Guid));
        }

        _localizationCandidates = Convert(builder);
    }

    public void FetchLatestAssets()
    {
        if (_localizationCandidates is null)
        {
            InitializeLocalizationCandidates();
            return;
        }

        Dictionary<string, (bool Selected, Dictionary<Guid, bool> Assets)> currentResources = [];
        foreach ((string label, AssetInfoPropertiesForEditor properties) in _localizationCandidates)
        {
            Dictionary<Guid, bool> assetsInQuickCheck = [];
            for (int i = 0; i < properties.Assets.Length; ++i)
            {
                AssetPropertiesForEditor assetProperties = properties.Assets[i];
                assetsInQuickCheck[assetProperties.Guid] = assetProperties.Show;
            }

            currentResources[label] = (properties.IsSelected, assetsInQuickCheck);
        }

        Dictionary<string, AssetInfoPropertiesForEditor> builder = [];
        foreach (GameAsset asset in Game.Data.GetAllAssets())
        {
            if (ShouldSkip(asset))
            {
                continue;
            }

            string label = asset.GetType().Name;
            if (!currentResources.TryGetValue(label, out (bool Selected, Dictionary<Guid, bool> Assets) priorResource))
            {
                priorResource = (true, []);
            }

            if (!builder.TryGetValue(label, out AssetInfoPropertiesForEditor? info))
            {
                info = new();
                builder[label] = info;

                info.IsSelected = priorResource.Selected;
            }

            AssetPropertiesForEditor assetProperty = new(asset.Guid);
            info.Assets = info.Assets.Add(assetProperty);

            if (priorResource.Assets.TryGetValue(asset.Guid, out bool assetSelected))
            {
                assetProperty.Show = assetSelected;
            }
        }

        _localizationCandidates = Convert(builder);
    }

    private ImmutableArray<(string, AssetInfoPropertiesForEditor)> Convert(Dictionary<string, AssetInfoPropertiesForEditor> builder)
    {
        var candidates = ImmutableArray.CreateBuilder<(string, AssetInfoPropertiesForEditor)>();
        foreach ((string type, AssetInfoPropertiesForEditor list) in builder)
        {
            list.Assets = list.Assets.Sort((a, b) =>
            {
                GameAsset assetA = Game.Data.GetAsset(a.Guid);
                GameAsset assetB = Game.Data.GetAsset(b.Guid);

                return assetA.Name.CompareTo(assetB.Name);
            });

            candidates.Add((type, list));
        }

        candidates.Sort((a, b) => a.Item1.CompareTo(b.Item1));
        return candidates.ToImmutable();
    }

    private bool ShouldSkip(GameAsset asset)
    {
        if (asset is LocalizationAsset or SpriteAsset or SpriteEventDataManagerAsset or
            FontAsset or FloorAsset)
        {
            return true;
        }

        if (asset.Name.StartsWith(GameDataManager.SKIP_CHAR))
        {
            return true;
        }

        return false;
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

    public void FetchLatestAssets()
    {
        foreach ((_, FilteredAssetsForLocalization f) in Filters)
        {
            f.FetchLatestAssets();
        }
    }

    public FilterLocalizationAsset() { }
}
