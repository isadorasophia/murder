using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Data;
using Murder.Serialization;
using System.Collections.Immutable;
using static Murder.Utilities.StringHelper;

namespace Murder.Editor.Importers
{
    [ImporterSettings(FilterType.OnlyTheseFolders, new[] { RelativeDirectory }, new[] { ".png" })]
    internal class PngImporter : ResourceImporter
    {
        public readonly AtlasId AtlasId = AtlasId.Static;

        private const string RelativeDirectory = "images";

        public override string RelativeSourcePath => RelativeDirectory;
        public override string RelativeOutputPath => "atlas";
        public override string RelativeDataOutputPath => "sprites_static";

        public PngImporter(EditorSettingsAsset editorSettings) : base(editorSettings) { }

        public override ValueTask LoadStagedContentAsync(bool reload, bool skipIfNoChangesFound)
        {
            string sourcePath = GetRawResourcesPath();
            string outputPath = GetSourcePackedPath();
            string dataPath = GetSourceResourcesPath();

            List<string> files = ChangedFiles;
            if (!reload)
            {
                // Cleanup generated assets folder
                FileManager.DeleteDirectoryIfExists(dataPath);
                FileManager.GetOrCreateDirectory(outputPath);

                files = AllFiles;
            }

            if (files.Count == 0)
            {
                return default;
            }

            if (files.Count > 0)
            {
                FileManager.GetOrCreateDirectory(outputPath);

                PackImages(files, sourcePath, outputPath, dataPath);

                GameLogger.Log($"Png importer loaded {files.Count} files.");
            }

            CopyOutputToBin = true;

            if (reload)
            {
                int skippedFiles = AllFiles.Count - ChangedFiles.Count;
                if (skippedFiles > 0)
                {
                    GameLogger.Log($"Png importer skipped {skippedFiles} files because they were not modified.");
                }

                GameLogger.Log($"Png importer loaded {files.Count} files.");
            }

            return default;
        }

        private ValueTask PackImages(List<string> files, string inputPath, string outputPath, string dataPath)
        {
            Packer packer = new();

            // This currently can't be async because it calls into Texture2D.FromFile, which reads data synchronously...
            packer.Process(files, 4096, 1, false);

            // First, pack and save the png files
            _ = packer.SaveAtlasses(Path.Join(outputPath, "static"));

            // Then, make and save the atlas data using the packer information
            string atlasName = AtlasId.GetDescription();

            string atlasDescriptorFullPath = GetSourcePackedAtlasDescriptorPath();
            using TextureAtlas atlas = new(atlasName);

            atlas.PopulateAtlas(GetCoordinatesForAtlas(packer, atlasName, inputPath));

            if (atlas.CountEntries == 0)
            {
                GameLogger.Error($"I didn't find any content to pack! ({inputPath})");
            }

            // Make sure we also have the atlas save at the binaries path.
            _ = FileManager.GetOrCreateDirectory(outputPath);

            // Save atlas descriptor at the output path.
            Game.Data.FileManager.SaveSerialized(atlas, atlasDescriptorFullPath);

            // Prepare an empty dictionary for a simple animation
            var animations = ImmutableDictionary.CreateBuilder<string, Animation>();
            animations.Add(string.Empty, Animation.Empty);
            ImmutableDictionary<string, Animation> emptyAnimations = animations.ToImmutable();

            // Now, create SpriteAssets for each image
            foreach (AtlasCoordinates image in atlas.GetAllEntries())
            {
                SpriteAsset asset = new(
                        EditorFileHelper.GuidFromName(image.Name),
                        atlas,
                        image.Name,
                        [image.Name],
                        emptyAnimations,
                        Point.Zero,
                        image.SourceRectangle.Size,
                        Rectangle.Empty
                    );

                string sourceFilePath = Path.Join(dataPath, $"{asset.Name}.json");
                Game.Data.FileManager.SaveSerialized<GameAsset>(asset, sourceFilePath);
            }

            return default;
        }

        private string? _cachedAtlasDescriptorPath = null;

        public override string GetSourcePackedAtlasDescriptorPath()
        {
            if (_cachedAtlasDescriptorPath is not null)
            {
                return _cachedAtlasDescriptorPath;
            }

            string atlasName = AtlasId.GetDescription();
            _cachedAtlasDescriptorPath = Path.Join(GetSourcePackedPath(), $"{atlasName}.json");

            return _cachedAtlasDescriptorPath;
        }
    }
}