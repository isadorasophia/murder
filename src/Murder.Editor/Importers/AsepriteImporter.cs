using Gum;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Data;
using Murder.Editor.Data.Graphics;
using Murder.Editor.Utilities;
using Murder.Serialization;
using static Murder.Utilities.StringHelper;

namespace Murder.Editor.Importers
{
    internal abstract class AsepriteImporter(EditorSettingsAsset editorSettings) : ResourceImporter(editorSettings)
    {
        protected abstract AtlasId Atlas { get; }

        public override bool SupportsAsyncLoading => true;

        /// <summary>
        /// Track reloaded sprites. This will be recalculated every time a temporary atlas needs to be created.
        /// </summary>
        private readonly HashSet<string> _reloadedSprites = new();

        private Packer? _pendingPacker = null;

        public override async ValueTask LoadStagedContentAsync(bool reload)
        {
            if (AllFiles.Count == 0)
            {
                return;
            }

            if (reload)
            {
                if (ChangedFiles.Count == 0)
                {
                    // Nothing really to reload...?
                    return;
                }

                ReloadChangedFiles();
                return;
            }

            await Task.Run(ProcessAllFiles);

            // On a clean operation, do not track any reloaded sprites.
            _reloadedSprites.Clear();
        }

        public override string GetSourcePackedAtlasDescriptorPath() => GetSourcePackedAtlasDescriptorPath(Atlas.GetDescription());

        protected override void StageFileImpl(string file, bool changed)
        {
            if (changed)
            {
                _reloadedSprites.Add(file);
            }
        }

        internal override void Flush()
        {
            if (_pendingPacker is null)
            {
                return;
            }

            SerializeAtlas(Atlas, _pendingPacker, SerializeAtlasFlags.EnableLogging | SerializeAtlasFlags.DeleteTemporaryAtlas);

            _pendingPacker = null;
        }

        [Flags]
        enum SerializeAtlasFlags
        {
            None = 0,
            EnableLogging = 0b1,
            DeleteTemporaryAtlas = 0b10
        }

        private void ReloadChangedFiles()
        {
            using PerfTimeRecorder recorder = new("Reloading Changed Aseprites");

            AtlasId targetAtlasId = AtlasId.Temporary;

            Packer? packer = CreateAtlasPacker(targetAtlasId, files: [.. _reloadedSprites]);
            if (packer is null)
            {
                return;
            }

            // Generate animation aseprite asset files
            for (int i = 0; i < packer.AsepriteFiles.Count; i++)
            {
                Aseprite animation = packer.AsepriteFiles[i];

                foreach (SpriteAsset asset in animation.CreateAssets(targetAtlasId))
                {
                    if (Game.Data.HasAsset<SpriteAsset>(asset.Guid))
                    {
                        // Remove the previous sprite asset.
                        Game.Data.RemoveAsset<SpriteAsset>(asset.Guid);
                    }

                    SaveAsset(asset, cleanDirectory: false);

                    // Load the new asset, as if nothing happened... >:)
                    Game.Data.AddAsset(asset);
                }
            }

            SerializeAtlas(targetAtlasId, packer, SerializeAtlasFlags.None);
        }

        private async Task ProcessAllFiles()
        {
            using PerfTimeRecorder recorder = new($"Reloading All Aseprites on {RelativeSourcePath}");

            await Task.Yield();

            AtlasId targetAtlasId = Atlas;

            Packer? packer = CreateAtlasPacker(Atlas, AllFiles);
            if (packer is null)
            {
                return;
            }

            // Check whether the target path has previously been loaded (and hence ignore any warnings).
            bool skipLoadingWarnings = Game.Data.IsPathOnSkipLoading(GetSourceResourcesPath());
            bool hasCleanedDirectory = false;

            // Generate animation aseprite asset files
            for (int i = 0; i < packer.AsepriteFiles.Count; i++)
            {
                var animation = packer.AsepriteFiles[i];

                foreach (SpriteAsset asset in animation.CreateAssets(targetAtlasId))
                {
                    bool cleanDirectoryBeforeSaving = false;
                    if (!hasCleanedDirectory)
                    {
                        // Make sure we clean the directory before saving this asset.
                        cleanDirectoryBeforeSaving = true;
                        hasCleanedDirectory = true;
                    }

                    SaveAsset(asset, cleanDirectoryBeforeSaving);

                    if (!skipLoadingWarnings && Game.Data.HasAsset<SpriteAsset>(asset.Guid))
                    {
                        if (skipLoadingWarnings)
                        {
                            // Remove the previous sprite asset.
                            Game.Data.RemoveAsset<SpriteAsset>(asset.Guid);
                        }
                        else
                        {
                            GameLogger.Warning($"Found a duplicated slice at {asset.Name}.");
                            continue;
                        }
                    }

                    // Instead of loading the asset we just saved (slow), track it right away!
                    Game.Data.AddAsset(asset);
                }
            }

            _pendingPacker = packer;
        }

        private Packer? CreateAtlasPacker(AtlasId targetAtlasId, List<string> files)
        {
            string sourcePackedPath = GetSourcePackedPath();   // Path where the atlas (.png/.json) will be saved in src.
            FileManager.GetOrCreateDirectory(sourcePackedPath); // Make sure it exists.

            Packer packer = new();
            packer.Process(files, 4096, 1, false);

            string atlasName = targetAtlasId.GetDescription();

            // Disposed by Game.Data
            TextureAtlas atlas = new(atlasName, targetAtlasId);

            string rawResourcesPath = GetRawResourcesPath(); // Path where the raw .aseprite files are.
            atlas.PopulateAtlas(GetCoordinatesForAtlas(packer, targetAtlasId, rawResourcesPath));

            if (atlas.CountEntries == 0)
            {
                GameLogger.Error($"I didn't find any content to pack! ({rawResourcesPath})");
                atlas.Dispose();

                return null;
            }

            Game.Data.ReplaceAtlas(targetAtlasId, atlas);

            return packer;
        }

        private void SerializeAtlas(AtlasId targetAtlasId, Packer packer, SerializeAtlasFlags flags)
        {
            TextureAtlas atlas = Game.Data.FetchAtlas(targetAtlasId);
            string atlasName = targetAtlasId.GetDescription();

            // Delete any previous atlas in the source directory.
            string atlasSourceDirectoryPath = Path.Join(GetSourcePackedPath(), Game.Profile.AtlasFolderName);
            foreach (string file in Directory.EnumerateFiles(atlasSourceDirectoryPath, $"{atlasName}*"))
            {
                File.Delete(file);
            }

            (int atlasCount, int maxWidth, int maxHeight) = packer.SaveAtlasses(Path.Join(atlasSourceDirectoryPath, atlasName));

            // Make sure we also have the atlas save at the binaries path.
            string atlasBinDirectoryPath = Path.Join(FileHelper.GetPath(_editorSettings.BinResourcesPath), Game.Profile.AtlasFolderName);
            _ = FileManager.GetOrCreateDirectory(atlasBinDirectoryPath);

            if (flags.HasFlag(SerializeAtlasFlags.DeleteTemporaryAtlas))
            {
                // If there is a temporary atlas, manually get rid of it.
                foreach (string file in Directory.EnumerateFiles(atlasBinDirectoryPath, "temporary*"))
                {
                    File.Delete(file);
                }

                foreach (string file in Directory.EnumerateFiles(atlasSourceDirectoryPath, "temporary*"))
                {
                    File.Delete(file);
                }
            }

            // Save atlas descriptor at the source and binaries directory.
            string atlasDescriptorName = GetSourcePackedAtlasDescriptorPath(atlasName);

            Game.Data.FileManager.SaveSerialized(atlas, atlasDescriptorName);
            EditorFileManager.DirectoryDeepCopy(atlasSourceDirectoryPath, atlasBinDirectoryPath);

            if (flags.HasFlag(SerializeAtlasFlags.EnableLogging))
            {
                GameLogger.LogPerf($"Pack '{atlasName}' ({atlasCount} images, {maxWidth}x{maxHeight}) completed with {atlas.CountEntries} entries", Game.Profile.Theme.Accent);
            }
        }

        private void SaveAsset(SpriteAsset asset, bool cleanDirectory)
        {
            string sourceAtlasAssetPath = Path.Join(asset.GetEditorAssetPath()!, RelativeSourcePath);
            string binAtlasAssetPath = Path.Join(asset.GetEditorAssetPath(useBinPath: true)!, RelativeSourcePath);

            // Clear aseprite animation folders. Delete them and proceed by creating new ones.
            if (cleanDirectory)
            {
                FileManager.DeleteDirectoryIfExists(sourceAtlasAssetPath);
                FileManager.DeleteDirectoryIfExists(binAtlasAssetPath);

                Architect.EditorData.SkipLoadingAssetsAt(binAtlasAssetPath);
            }

            // Add the relative directory to the editor path.
            asset.AppendEditorPath(RelativeSourcePath);

            string assetNameWithJson = $"{asset.Name}.json";

            string sourceFilePath = Path.Join(sourceAtlasAssetPath, assetNameWithJson);
            Game.Data.FileManager.SaveSerialized<GameAsset>(asset, sourceFilePath);

            string binFilePath = Path.Join(binAtlasAssetPath, assetNameWithJson);
            FileManager.GetOrCreateDirectory(Path.GetDirectoryName(binFilePath)!);

            File.Copy(sourceFilePath, binFilePath, overwrite: true);
        }

        private string GetSourcePackedAtlasDescriptorPath(string atlasName)
        {
            string atlasSourceDirectoryPath = Path.Join(GetSourcePackedPath(), Game.Profile.AtlasFolderName);
            return Path.Join(atlasSourceDirectoryPath, $"{atlasName}.json");
        }
    }
}