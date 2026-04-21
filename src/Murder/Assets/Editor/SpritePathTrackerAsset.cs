using Bang;
using Murder.Attributes;
using Murder.Data;

namespace Murder.Assets.Editor;

[HideInEditor] // This is created by the engine and should never be actually exposed to the UI.
public class SpritePathTrackerAsset : GameAsset
{
    /// <summary>
    /// Use <see cref="GameDataManager.SKIP_CHAR"/> to hide this in the editor.
    /// </summary>
    public override string EditorFolder => GameDataManager.HiddenAssetsRelativePath;

    [Serialize]
    private readonly Dictionary<Guid, HashSet<string>> _assetToLocalPath = [];

    [Serialize]
    private readonly Dictionary<string, Guid> _localPathsToAsset = new(StringComparer.InvariantCultureIgnoreCase);

    public void AddPathToGuid(Guid guid, string file, bool @overwrite)
    {
        lock (_assetToLocalPath) 
        {
            if (!@overwrite && _assetToLocalPath.TryGetValue(guid, out HashSet<string>? currentPaths))
            {
                currentPaths.Add(file);
            }
            else
            {
                currentPaths = [file];
                _assetToLocalPath[guid] = currentPaths;
            }

            _localPathsToAsset[file] = guid;
        }
    }

    public HashSet<string>? FetchAllPathsWith(Guid guid)
    {
        lock (_assetToLocalPath)
        {
            if (_assetToLocalPath.TryGetValue(guid, out var paths))
            {
                return paths;
            }

            return null;
        }
    }

    public Guid? FetchGuidFromPath(string path)
    {
        lock (_localPathsToAsset)
        {
            if (_localPathsToAsset.TryGetValue(path, out Guid guid))
            {
                return guid;
            }

            return null;
        }
    }

    public void Reset()
    {
        _assetToLocalPath.Clear();
        _localPathsToAsset.Clear();
    }
}
