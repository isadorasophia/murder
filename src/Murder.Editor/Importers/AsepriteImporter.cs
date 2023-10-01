using Gum;
using Murder.Assets.Graphics;
using Murder.Core.Geometry;
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

        internal override ValueTask LoadStagedContentAsync(bool clean)
        {
            if (AllFiles.Count == 0)
            {
                return default;
            }

            if (clean)
            {
                ReloadAllFiles();
                return default;
            }

            if (ChangedFiles.Count == 0)
            {
                return default;
            }

            ReloadChangedFiles();
            return default;
        }

        public override string GetSourcePackedAtlasDescriptorPath() => GetSourcePackedAtlasDescriptorPath(Atlas.GetDescription());

        private void ReloadChangedFiles()
        {
            using PerfTimeRecorder recorder = new("Reloading Changed Aseprites");

            string sourcePackedPath = GetSourcePackedPath();   // Path where the atlas (.png/.json) will be saved in src.
            FileHelper.GetOrCreateDirectory(sourcePackedPath); // Make sure it exists.

            Packer packer = new();
            packer.Process(ChangedFiles, 4096, 1, false);

            AtlasId targetAtlasId = AtlasId.Temporary;

            string atlasSourceDirectoryPath = Path.Join(sourcePackedPath, Game.Profile.AtlasFolderName);
            string atlasName = targetAtlasId.GetDescription();

            (int atlasCount, int maxWidth, int maxHeight) = packer.SaveAtlasses(Path.Join(atlasSourceDirectoryPath, atlasName));

            // Disposed by Game.Data
            TextureAtlas atlas = new(atlasName, targetAtlasId);

            string rawResourcesPath = GetRawResourcesPath(); // Path where the raw .aseprite files are.
            atlas.PopulateAtlas(PopulateAtlas(packer, targetAtlasId, rawResourcesPath));

            if (atlas.CountEntries == 0)
            {
                GameLogger.Error($"I didn't find any content to pack! ({rawResourcesPath})");
            }

            // Make sure we also have the atlas save at the binaries path.
            string atlasBinDirectoryPath = Path.Join(GetBinPackedPath(), Game.Profile.AtlasFolderName);
            _ = FileHelper.GetOrCreateDirectory(atlasBinDirectoryPath);

            // Save atlas descriptor at the source and binaries directory.
            string atlasDescriptorName = GetSourcePackedAtlasDescriptorPath(atlasName);

            FileHelper.SaveSerialized(atlas, atlasDescriptorName);
            FileHelper.DirectoryDeepCopy(atlasSourceDirectoryPath, atlasBinDirectoryPath);

            Game.Data.ReplaceAtlas(targetAtlasId, atlas);

            // Generate animation aseprite asset files
            for (int i = 0; i < packer.AsepriteFiles.Count; i++)
            {
                Aseprite animation = packer.AsepriteFiles[i];

                foreach (SpriteAsset asset in animation.CreateAssets(targetAtlasId))
                {
                    if (Game.Data.TryGetAsset<SpriteAsset>(asset.Guid) is SpriteAsset previouslyLoadedAsset)
                    {
                        // Remove the previous sprite asset.
                        Game.Data.RemoveAsset<SpriteAsset>(asset.Guid);
                    }

                    string sourceAsepritePath = asset.GetEditorAssetPath()!;
                    string binAsepritePath = asset.GetEditorAssetPath(useBinPath: true)!;

                    string assetName = $"{asset.Name}.json";

                    string sourceFilePath = Path.Join(sourceAsepritePath, assetName);
                    FileHelper.SaveSerialized(asset, sourceFilePath);

                    string binFilePath = Path.Join(binAsepritePath, assetName);
                    _ = FileHelper.GetOrCreateDirectory(Path.GetDirectoryName(binFilePath)!);

                    File.Copy(sourceFilePath, binFilePath, overwrite: true);

                    // Load the new asset, as if nothing happened... >:)
                    Game.Data.AddAsset(asset);
                }
            }

            // Also save the generated assets at the binaries directory.
            FileHelper.DirectoryDeepCopy(atlasSourceDirectoryPath, atlasBinDirectoryPath);
        }

        // TODO: Check this out for async...
        //private Packer? _pendingPacker = null;

        //private void SaveAtlas()
        //{
        //    if (_pendingPacker is null)
        //    {
        //        return;
        //    }

        //    AtlasId targetAtlasId = Atlas;

        //    string atlasSourceDirectoryPath = Path.Join(GetSourcePackedPath(), Game.Profile.AtlasFolderName);
        //    string atlasName = targetAtlasId.GetDescription();

        //    _pendingPacker.SaveAtlasses(Path.Join(atlasSourceDirectoryPath, atlasName));
        //    _pendingPacker = null;
        //}

        private void ReloadAllFiles()
        {
            using PerfTimeRecorder recorder = new("Reloading All Aseprites");

            string sourcePackedPath = GetSourcePackedPath();   // Path where the atlas (.png/.json) will be saved in src.
            FileHelper.GetOrCreateDirectory(sourcePackedPath); // Make sure it exists.

            Packer packer = new();
            packer.Process(AllFiles, 4096, 1, false);

            AtlasId targetAtlasId = Atlas;

            string atlasSourceDirectoryPath = Path.Join(sourcePackedPath, Game.Profile.AtlasFolderName);
            string atlasName = targetAtlasId.GetDescription();

            // Delete any previous atlas in the source directory.
            Directory.Delete(atlasSourceDirectoryPath, recursive: true);

            // I can't make this part async T_T
            (int atlasCount, int maxWidth, int maxHeight) = packer.SaveAtlasses(Path.Join(atlasSourceDirectoryPath, atlasName));

            // Disposed by Game.Data
            TextureAtlas atlas = new(atlasName, targetAtlasId);

            string rawResourcesPath = GetRawResourcesPath(); // Path where the raw .aseprite files are.
            atlas.PopulateAtlas(PopulateAtlas(packer, targetAtlasId, rawResourcesPath));

            if (atlas.CountEntries == 0)
            {
                GameLogger.Error($"I didn't find any content to pack! ({rawResourcesPath})");
            }

            // Make sure we also have the atlas save at the binaries path.
            string atlasBinDirectoryPath = Path.Join(GetBinPackedPath(), Game.Profile.AtlasFolderName);
            _ = FileHelper.GetOrCreateDirectory(atlasBinDirectoryPath);

            // If there is a temporary atlas, manually get rid of it.
            foreach (string file in Directory.EnumerateFiles(atlasBinDirectoryPath, "temporary*"))
            {
                File.Delete(file);
            }

            // Save atlas descriptor at the source and binaries directory.
            string atlasDescriptorName = GetSourcePackedAtlasDescriptorPath(atlasName);

            FileHelper.SaveSerialized(atlas, atlasDescriptorName);
            FileHelper.DirectoryDeepCopy(atlasSourceDirectoryPath, atlasBinDirectoryPath);

            Game.Data.ReplaceAtlas(targetAtlasId, atlas);

            bool hasCleanedDirectory = false;
            
            // Generate animation aseprite asset files
            for (int i = 0; i < packer.AsepriteFiles.Count; i++)
            {
                var animation = packer.AsepriteFiles[i];

                foreach (SpriteAsset asset in animation.CreateAssets(targetAtlasId))
                {
                    string sourceAsepritePath = asset.GetEditorAssetPath()!;
                    string binAsepritePath = asset.GetEditorAssetPath(useBinPath: true)!;

                    // Clear aseprite animation folders. Delete them and proceed by creating new ones.
                    if (!hasCleanedDirectory)
                    {
                        hasCleanedDirectory = true;

                        Directory.Delete(sourceAsepritePath, recursive: true);

                        FileHelper.GetOrCreateDirectory(sourceAsepritePath);
                        FileHelper.GetOrCreateDirectory(binAsepritePath);
                    }

                    string assetName = $"{asset.Name}.json";

                    string sourceFilePath = Path.Join(sourceAsepritePath, assetName);
                    FileHelper.SaveSerialized(asset, sourceFilePath);

                    string binFilePath = Path.Join(binAsepritePath, assetName);
                    _ = FileHelper.GetOrCreateDirectory(Path.GetDirectoryName(binFilePath)!);

                    File.Copy(sourceFilePath, binFilePath, overwrite: true);
                }
            }

            // Also save the generated assets at the binaries directory.
            FileHelper.DirectoryDeepCopy(atlasSourceDirectoryPath, atlasBinDirectoryPath);
            GameLogger.LogPerf($"Pack '{atlas.Name}' ({atlasCount} images, {maxWidth}x{maxHeight}) completed with {atlas.CountEntries} entries", Game.Profile.Theme.Accent);
        }

        /// <summary>
        /// Populate an atlas based on the packer information of the images database.
        /// </summary>
        /// <param name="packer">Packer information (previously initialized with the files).</param>
        /// <param name="atlasId">Atlas identifier.</param>
        /// <param name="sourcesPath">Root sources path. Used to build the relative path of each final file in the atlas.</param>
        private static IEnumerable<(string id, AtlasCoordinates coord)> PopulateAtlas(Packer packer, AtlasId atlasId, string sourcesPath)
        {
            for (int i = 0; i < packer.Atlasses.Count; i++)
            {
                foreach (var node in packer.Atlasses[i].Nodes)
                {
                    if (node.Texture == null)
                    {
                        continue;
                    }

                    string name = FileHelper.GetPathWithoutExtension(Path.GetRelativePath(sourcesPath, node.Texture.Source)).EscapePath()
                        + (node.Texture.HasSlices ? $"_{(node.Texture.SliceName)}" : string.Empty)
                        + (node.Texture.HasLayers ? $"_{node.Texture.LayerName}" : "")
                        + (node.Texture.IsAnimation ? $"_{node.Texture.Frame:0000}" : "");

                    AtlasCoordinates coord = new(
                            name: name,
                            atlasId: atlasId,
                            atlasRectangle: new IntRectangle(node.Bounds.X, node.Bounds.Y, node.Bounds.Width, node.Bounds.Height),
                            trimArea: node.Texture.TrimArea,
                            size: node.Texture.SliceSize,
                            atlasIndex: i,
                            atlasWidth: packer.Atlasses[i].Width,
                            atlasHeight: packer.Atlasses[i].Height
                        );

                    yield return (id: name, coord);
                }
            }
        }

        private string GetSourcePackedAtlasDescriptorPath(string atlasName)
        {
            string atlasSourceDirectoryPath = Path.Join(GetSourcePackedPath(), Game.Profile.AtlasFolderName);
            return Path.Join(atlasSourceDirectoryPath, $"{atlasName}.json");
        }
    }
}
