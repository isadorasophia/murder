using Murder.Assets;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Utilities;
using Murder.Serialization;

namespace Murder.Editor.Data;

/// <summary>
/// This is a class that leverages the file system watcher to automatically hot reload resources.
/// </summary>
public partial class EditorDataManager
{
    /// <summary>
    /// File path of the packed contents for the released game.
    /// </summary>
    public override string PublishedPackedAssetsFullPath => FileHelper.GetPath(Architect.EditorData.PackedSourceDirectoryPath, _packedGameDataDirectory);

    private string[]? _preloadRelativePaths = null;

    private string[]? _preloadSourcePath = null;

    /// <summary>
    /// This will pack all the assets into a single asset for publishing the game.
    /// </summary>
    public void PackPublishedGame()
    {
        // Check whether our atlas is up to date.
        string atlasBinDirectoryPath = Path.Join(FileHelper.GetPath(EditorSettings.BinResourcesPath), Game.Profile.AtlasFolderName);
        if (Directory.EnumerateFiles(atlasBinDirectoryPath, "temporary*").Any())
        {
            GameLogger.Error("Unable to pack published game with a temporary atlas (yet, sorry)! Please restart the editor before packing.");
            return;
        }

        List<GameAsset> preloadAssets = new(32);
        List<GameAsset> assets = new(_allAssets.Count);

        foreach (GameAsset asset in _allAssets.Values)
        {
            string? path = asset.GetEditorAssetPath();
            if (path is null)
            {
                continue;
            }

            if (IsPathAtPreload(path))
            {
                preloadAssets.Add(asset);
                continue;
            }

            if (base.ShouldSkipAsset(path))
            {
                continue;
            }

            assets.Add(asset);
        }

        PackedGameData data = new(preloadAssets, assets);

        string packedGameDataPath = Path.Join(PublishedPackedAssetsFullPath, _packedGameDataFilename);
        FileManager.CreateDirectoryPathIfNotExists(packedGameDataPath);

        Task.Run(async delegate
        {
            // so we can actually switch threads!
            await Task.Yield();

            FileManager.PackContent(data, packedGameDataPath);
        });

        GameLogger.Log($"Published game content with {preloadAssets.Count} (preload) and {assets.Count} (gameplay) assets at '{packedGameDataPath}'.");
    }

    private bool IsPathAtPreload(string path)
    {
        if (_preloadRelativePaths is null || _assetsSourceDirectoryPath is null)
        {
            return false;
        }

        if (_preloadSourcePath is null)
        {
            _preloadSourcePath = new string[_preloadRelativePaths.Length];

            for (int i = 0; i < _preloadRelativePaths.Length; i++)
            {
                _preloadSourcePath[i] = FileHelper.EscapePath(FileHelper.GetPath(_sourceResourcesDirectory, _preloadRelativePaths[i]));
            }
        }

        foreach (string preloadPath in _preloadSourcePath)
        {
            if (path.Contains(preloadPath))
            {
                return true;
            }
        }

        return false;
    }
}
