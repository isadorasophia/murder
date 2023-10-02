﻿using Murder.Assets.Graphics;
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

        internal override ValueTask LoadStagedContentAsync(bool forceAll)
        {
            string sourcePath = GetRawResourcesPath();
            string outputPath = GetSourcePackedPath();
            string dataPath = GetSourceResourcesPath();

            List<string> files = ChangedFiles;
            if (forceAll)
            {
                // Cleanup generated assets folder
                FileHelper.DeleteDirectoryIfExists(dataPath);
                FileHelper.GetOrCreateDirectory(outputPath);

                files = AllFiles;
            }

            if (files.Count == 0)
            {
                return default;
            }

            if (files.Count > 0)
            {
                FileHelper.GetOrCreateDirectory(outputPath);

                PackImages(files, sourcePath, outputPath, dataPath);

                GameLogger.Log($"Png importer loaded {files.Count} files.");
            }

            CopyOutputToBin = true;

            if (!forceAll)
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
            (int atlasCount, int maxWidth, int maxHeight) = packer.SaveAtlasses(Path.Join(outputPath, "static"));

            // Then, make and save the atlas data using the packer information
            string atlasName = AtlasId.GetDescription();

            string atlasDescriptorFullPath = GetSourcePackedAtlasDescriptorPath();
            using TextureAtlas atlas = new(atlasName, AtlasId);

            atlas.PopulateAtlas(GetCoordinatesFromPacker(packer, AtlasId, inputPath));

            if (atlas.CountEntries == 0)
            {
                GameLogger.Error($"I didn't find any content to pack! ({inputPath})");
            }

            // Make sure we also have the atlas save at the binaries path.
            _ = FileHelper.GetOrCreateDirectory(outputPath);
            
            // Save atlas descriptor at the output path.
            FileHelper.SaveSerialized(atlas, atlasDescriptorFullPath);

            // Prepare an empty dictionary for a simple animation
            var animations = ImmutableDictionary.CreateBuilder<string, Animation>();
            animations.Add(string.Empty, Animation.Empty);
            ImmutableDictionary<string, Animation> emptyAnimations = animations.ToImmutable();

            // Now, create SpriteAssets for each image
            foreach (AtlasCoordinates image in atlas.GetAllEntries())
            {
                GameLogger.Log(image.Name);

                SpriteAsset asset = new(
                        FileHelper.GuidFromName(image.Name),
                        atlas,
                        image.Name,
                        ImmutableArray.Create(image.Name),
                        emptyAnimations,
                        Point.Zero,
                        image.SourceRectangle.Size,
                        Rectangle.Empty
                    );

                string sourceFilePath = Path.Join(dataPath, $"{asset.Name}.json");
                FileHelper.SaveSerialized(asset, sourceFilePath);
            }

            return default;
        }

        private static IEnumerable<(string id, AtlasCoordinates coord)> GetCoordinatesFromPacker(Packer packer, AtlasId atlasId, string sourcesPath)
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

                    yield return (id: name, coord: coord);
                }
            }
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