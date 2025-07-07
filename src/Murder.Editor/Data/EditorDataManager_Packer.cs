using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Assets.Save;
using Murder.Core.Sounds;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Utilities;
using System.Collections.Generic;

namespace Murder.Editor.Data;

/// <summary>
/// This is a class that leverages the file system watcher to automatically hot reload resources.
/// </summary>
public partial class EditorDataManager
{
    /// <summary>
    /// File relativePath of the packed contents for the released game.
    /// </summary>
    public override string PublishedPackedAssetsFullPath => FileHelper.GetPath(Architect.EditorData.PackedSourceDirectoryPath, _packedGameDataDirectory);

    private string[]? _preloadRelativePaths = null;

    private string[]? _preloadSourcePath = null;

    /// <summary>
    /// This will pack all the collectionOfAssets into a single asset for publishing the game.
    /// </summary>
    public void PackPublishedGame()
    {
        if (HasTemporaryAtlas())
        {
            GameLogger.Error("Unable to pack published game with a temporary atlas (yet, sorry)! Please restart the editor before packing.");
            return;
        }

        List<GameAsset> preloadAssets = new(32);

        int maxAssetsPerSaveData = 500;
        int currentSplitSave = 0;

        List<List<GameAsset>> collectionOfAssets = [];

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

            if (collectionOfAssets.Count <= currentSplitSave)
            {
                collectionOfAssets.Add([]);
            }

            collectionOfAssets[currentSplitSave].Add(asset);

            if (collectionOfAssets[currentSplitSave].Count > maxAssetsPerSaveData)
            {
                currentSplitSave++;
            }
        }

        int totalData = collectionOfAssets.Count;
        PreloadPackedGameData preloadData = new(totalData, preloadAssets);

        PackedGameData[] packedGameData = new PackedGameData[totalData];
        for (int i = 0; i < totalData; i++)
        {
            packedGameData[i] = new(collectionOfAssets[i])
            {
                TexturesNoAtlasPath = i == 0 ? AvailableUniqueTextures : []
            };
        }

        PackedSoundData soundData = new()
        {
            Banks = Game.Sound.FetchAllBanks(),
            Plugins = Game.Sound.FetchAllPlugins()
        };

        FileManager.GetOrCreateDirectory(PublishedPackedAssetsFullPath);

        Task.Run(async delegate
        {
            // so we can actually switch threads!
            await Task.Yield();

            string preloadPackedGameDataPath = Path.Join(PublishedPackedAssetsFullPath, PreloadPackedGameData.Name);
            string packedSoundDataPath = Path.Join(PublishedPackedAssetsFullPath, PackedSoundData.Name);

            FileManager.PackContent(preloadData, preloadPackedGameDataPath);
            FileManager.PackContent(soundData, packedSoundDataPath);

            int totalAssets = 0;
            for (int i = 0; i < packedGameData.Length; i++)
            {
                string packedGameDataPath = Path.Join(PublishedPackedAssetsFullPath, string.Format(PackedGameData.Name, i));
                FileManager.PackContent(packedGameData[i], packedGameDataPath);

                totalAssets += packedGameData[i].Assets.Count;
            }

            GameLogger.Log($"Published game content with {preloadAssets.Count} (preload) and {totalAssets} (gameplay) assets at '{PublishedPackedAssetsFullPath}'.");

            if (EditorSettings.CheckForPackedAssetsIntegrity)
            {
                await CheckForPackedAssetsIntegrity(packedGameData.Length);
            }
        });
    }

    private async Task CheckForPackedAssetsIntegrity(int total)
    {
        GameLogger.Log($"Checking for files integrity after publishing...");

        Dictionary<Guid, string> packedJsonContent = [];
        for (int i = 0; i < total; i++)
        {
            string packedGameDataPath = Path.Join(PublishedPackedAssetsFullPath, string.Format(PackedGameData.Name, i));
            PackedGameData? data = FileManager.UnpackContent<PackedGameData>(packedGameDataPath);
            if (data is null)
            {
                GameLogger.Error($"Unable to unpack data file {i}!");
                return;
            }

            foreach (GameAsset packedAsset in data.Assets)
            {
                if (Game.Data.TryGetAsset(packedAsset.Guid) is not GameAsset asset)
                {
                    GameLogger.Error($"Unable to find matching asset {packedAsset.Name}!");
                    return;
                }

                string jsonForAsset = FileManager.SerializeToJson(asset);
                string jsonForPackedAsset = FileManager.SerializeToJson(packedAsset);
                if (jsonForAsset != jsonForPackedAsset)
                {
                    GameLogger.Error($"Mismatch found when comparing json for {packedAsset.Name}!");
                    GameLogger.Log(jsonForAsset);
                    GameLogger.Log("----------");
                    GameLogger.Log(jsonForPackedAsset);
                    return;
                }

                if (asset.Equals(packedAsset))
                {
                    GameLogger.Error($"Mismatch found when comparing assets for {packedAsset.Name}!");
                    return;
                }

                packedJsonContent[packedAsset.Guid] = jsonForPackedAsset;
            }
        }

        GameLogger.Log($"Loaded assets look good so far, now we'll check for all json assets in the bin...");

        string[] relativePaths =
        {
            Path.Join(_binResourcesDirectory, GameProfile.AssetResourcesPath, GameProfile.GenericAssetsPath),
            Path.Join(_binResourcesDirectory, GameProfile.AssetResourcesPath, GameProfile.ContentECSPath)
        };

        foreach (string relativePath in relativePaths)
        {
            string fullPath = FileHelper.GetPath(relativePath);

            foreach (string filename in EditorFileManager.GetAllFilesInFolder(fullPath, "*.json", recursive: true))
            {
                if (ShouldSkipAsset(filename))
                {
                    continue;
                }

                GameAsset? binAsset = TryLoadAsset(filename, fullPath, skipFailures: true, hasEditorPath: false);
                if (binAsset == null)
                {
                    GameLogger.Error($"Unable to find asset at {filename}!");
                    return;
                }

                if (binAsset.Name.StartsWith('_'))
                {
                    continue;
                }

                if (!packedJsonContent.TryGetValue(binAsset.Guid, out string? jsonForPackedAsset))
                {
                    GameLogger.Error($"We did not pack {binAsset.Name}?");
                    return;
                }

                string jsonForBinAsset = await File.ReadAllTextAsync(filename);
                if (jsonForPackedAsset != jsonForBinAsset && binAsset is not SpriteAsset)
                {
                    GameLogger.Error($"Mismatch found when comparing json for {binAsset.Name}!");
                    GameLogger.Log(jsonForBinAsset);
                    GameLogger.Log("----------");
                    GameLogger.Log(jsonForPackedAsset);
                    GameLogger.Log("----------=end=");

                    return;
                }
            }
        }

        GameLogger.Log($"Everything looks good!");
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

    private bool HasTemporaryAtlas()
    {
        // Check whether our atlas is up to date.
        string atlasBinDirectoryPath = Path.Join(FileHelper.GetPath(EditorSettings.BinResourcesPath), Game.Profile.AtlasFolderName);
        if (Directory.EnumerateFiles(atlasBinDirectoryPath, "temporary*").Any())
        {
            return true;
        }

        return false;
    }
}
