using Murder.Assets.Graphics;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Editor.Data;

/// <summary>
/// This is a class that leverages the file system watcher to automatically hot reload resources.
/// </summary>
public partial class EditorDataManager
{
    protected override void PreloadContentImpl()
    {
        _preloadRelativePaths ??= [
            Path.Join(GameProfile.AssetResourcesPath, GameProfile.GenericAssetsPath, "Generated/", "preload_images"),
                Path.Join(GameProfile.AssetResourcesPath, GameProfile.GenericAssetsPath, "Libraries")];

        foreach (string relativePath in _preloadRelativePaths)
        {
            string fullPath = FileHelper.GetPath(_binResourcesDirectory, relativePath);
            LoadAssetsAtPath(fullPath, hasEditorPath: true);

            SkipLoadingAssetsAt(fullPath);
        }

        PreprocessVideoFiles();
    }

    protected override async Task LoadAllAssetsAsync()
    {
        await Task.WhenAll(
                LoadAssetsAtPathAsync(Path.Join(_binResourcesDirectory, GameProfile.AssetResourcesPath, GameProfile.GenericAssetsPath)),
                LoadAssetsAtPathAsync(Path.Join(_binResourcesDirectory, GameProfile.AssetResourcesPath, GameProfile.ContentECSPath))
            );
    }

    protected override async Task LoadFontsAndTexturesAsync()
    {
        using PerfTimeRecorder recorder = new("Loading Fonts and Textures");

        GameLogger.Verify(PackedBinDirectoryPath is not null, "Why hasn't LoadContent() been called?");

        string? murderFontsFolder = Path.Join(PackedBinDirectoryPath, "fonts");
        string? noAtlasFolder = Path.Join(PackedBinDirectoryPath, "images");

        ImmutableArray<string>.Builder uniqueTextures = ImmutableArray.CreateBuilder<string>();

        foreach (string texture in Directory.EnumerateFiles(noAtlasFolder))
        {
            if (Path.GetExtension(texture) == TextureServices.PNG_EXTENSION)
            {
                uniqueTextures.Add(FileHelper.GetPathWithoutExtension(Path.GetRelativePath(PackedBinDirectoryPath, texture)));
            }
        }

        await Parallel.ForEachAsync(Directory.EnumerateFiles(murderFontsFolder), async (file, cancellation) =>
        {
            if (Path.GetExtension(file) == TextureServices.QOI_GZ_EXTENSION)
            {
                uniqueTextures.Add(FileHelper.GetPathWithoutExtension(Path.GetRelativePath(PackedBinDirectoryPath, file)));
            }
            else if (Path.GetExtension(file) == ".json")
            {
                FontAsset? asset = await FileManager.DeserializeAssetAsync<FontAsset>(file);
                if (asset is null)
                {
                    GameLogger.Error($"Unable to load font: {file}. Duplicate index found!");
                    return;
                }

                Game.Data.AddAsset(asset);
                TrackFont(asset);
            }
        });

        AvailableUniqueTextures = uniqueTextures.ToImmutable();
    }

    /// <summary>
    /// Fetch all assets at a given path.
    /// </summary>
    /// <param name="fullPath">Full directory path.</param>
    /// <param name="recursive">Whether it should iterate over its nested elements.</param>
    /// <param name="skipFailures">Whether it should skip reporting load errors as warnings.</param>
    /// <param name="stopOnFailure">Whether it should immediately stop after finding an issue.</param>
    /// <param name="hasEditorPath">Whether the editor path is already appended in <paramref name="fullPath"/>.</param>
    protected IEnumerable<GameAsset> FetchAssetsAtPath(string fullPath,
        bool recursive = true, bool skipFailures = true, bool stopOnFailure = false, bool hasEditorPath = false)
    {
        foreach (string filename in EditorFileManager.GetAllFilesInFolder(fullPath, "*.json", recursive))
        {
            if (ShouldSkipAsset(filename))
            {
                continue;
            }

            GameAsset? asset = TryLoadAsset(filename, fullPath, skipFailures, hasEditorPath: hasEditorPath);
            if (asset == null && stopOnFailure)
            {
                // Immediately stop iterating.
                yield break;
            }

            if (asset != null)
            {
                yield return asset;
            }
            else
            {
                GameLogger.Warning($"Unable to deserialize {filename}.");
            }
        }
    }

    /// <summary>
    /// Fetch all assets at a given path asynchronously and store them.
    /// </summary>
    /// <param name="fullPath">Full directory path.</param>
    /// <param name="recursive">Whether it should iterate over its nested elements.</param>
    /// <param name="skipFailures">Whether it should skip reporting load errors as warnings.</param>
    /// <param name="stopOnFailure">Whether it should immediately stop after finding an issue.</param>
    /// <param name="hasEditorPath">Whether the editor path is already appended in <paramref name="fullPath"/>.</param>
    protected async Task FetchAndAddAssetsAtPathAsync(
        string fullPath,
        bool recursive = true,
        bool skipFailures = true,
        bool stopOnFailure = false,
        bool hasEditorPath = false)
    {
        bool stop = false;

        IEnumerable<string> files = EditorFileManager.GetAllFilesInFolder(fullPath, "*.json", recursive);
        await Parallel.ForEachAsync(files, async (f, cancellation) =>
        {
            if (stop || ShouldSkipAsset(f))
            {
                return;
            }

            GameAsset? asset = await TryLoadAssetAsync(f, fullPath, skipFailures, hasEditorPath: hasEditorPath);
            if (asset == null && stopOnFailure)
            {
                // Immediately stop iterating.
                stop = true;
                return;
            }

            if (asset != null)
            {
                AddAsset(asset);
            }
            else
            {
                GameLogger.Warning($"Unable to deserialize {f}.");
            }
        });
    }

    protected void LoadAssetsAtPath(in string relativePath, bool hasEditorPath = false)
    {
        string fullPath = FileHelper.GetPath(relativePath);

        using PerfTimeRecorder recorder = new($"Loading Assets at {fullPath}");

        foreach (GameAsset asset in FetchAssetsAtPath(fullPath, skipFailures: true, hasEditorPath: hasEditorPath))
        {
            AddAsset(asset);
        }
    }

    protected async Task LoadAssetsAtPathAsync(string relativePath, bool hasEditorPath = false)
    {
        string fullPath = FileHelper.GetPath(relativePath);

        using PerfTimeRecorder recorder = new($"Loading Assets at {fullPath}");
        await FetchAndAddAssetsAtPathAsync(fullPath, skipFailures: true, hasEditorPath: hasEditorPath);
    }
}
