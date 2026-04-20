using Bang;
using Murder.Attributes;
using Murder.Data;
using System.Collections.Immutable;

namespace Murder.Assets.Editor;

[HideInEditor] // This is created by the engine and should never be actually exposed to the UI.
public class SpritePathsTrackerAsset : GameAsset
{
    public override string EditorFolder => GameDataManager.HiddenAssetsRelativePath;

    [Serialize]
    private readonly Dictionary<Guid, ImmutableHashSet<string>> _assetToPaths = new();
    private readonly Dictionary<string, Guid> _pathToAsset = new();

    public void Clear()
    {
        _assetToPaths.Clear();
        _pathToAsset.Clear();
    }

    public void Link(Guid guid, string file)
    {
        if (_assetToPaths.ContainsKey(guid))
        {
            _assetToPaths[guid] = _assetToPaths[guid].Add(file);
        }
        else
        {
            _assetToPaths[guid] = [file];
        }

        _pathToAsset[file] = guid;
    }

    public ImmutableHashSet<string>? TryGetPathFor(Guid guid)
    {
        if (_assetToPaths.TryGetValue(guid, out var paths))
        {
            return paths;
        }

        return null;
    }
}
