using Murder.Assets.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Data;
using Murder.Serialization;
using System.Collections.Immutable;
using System.Numerics;
using static Murder.Utilities.StringHelper;

namespace Murder.Editor.Importers
{
    [ImporterSettings(FilterType.OnlyTheseFolders, new string[] { "images" }, new string[] { ".png" })]

    internal class PngImporter : ResourceImporter
    {
        public override string RelativeSourcePath => "images";
        public override string RelativeOutputPath => "atlas";
        public override string RelativeDataOutputPath => "sprites_static";

        internal override ValueTask LoadStagedContentAsync(EditorSettingsAsset editorSettings, bool cleanImport)
        {
            string sourcePath = GetFullSourcePath(editorSettings);
            string outputPath = GetFullOutputPath(editorSettings);
            string dataPath = GetFullDataPath(editorSettings);

            // Quick import not implemented yet
            cleanImport = true; // ChangedFiles.Count > 0;
            
            if (cleanImport)
            {
                // Cleanup generated assets folder
                FileHelper.DeleteDirectoryIfExists(dataPath);

                FileHelper.GetOrCreateDirectory(outputPath);

                _ = PackImages(AllFiles.ToImmutableArray(), sourcePath, outputPath, dataPath);

                GameLogger.Log($"Png(no-atlas) importer loaded {AllFiles.Count} files.");
            }
            else
            {
                int skippedFiles = AllFiles.Count - ChangedFiles.Count;

                if (skippedFiles > 0)
                {
                    GameLogger.Log($"Png(no-atlas) importer skipped {skippedFiles} files because they were not modified.");
                }

                GameLogger.Log($"Png(no-atlas) importer loaded {ChangedFiles.Count} files.");
            }



            return default;
        }

        private ValueTask PackImages(ImmutableArray<string> allFiles, string inputPath, string outputPath, string dataPath)
        {
            Packer packer = new();
            packer.Process(allFiles, 4096, 1, false);
            
            // First, pack and save the png files
            (int atlasCount, int maxWidth, int maxHeight) = packer.SaveAtlasses(Path.Join(outputPath, "static"));

            // Then, make and save the atlas data using the packer information
            AtlasId atlasId = AtlasId.Static;
            string atlasName = atlasId.GetDescription();

            string atlasDescriptorFullPath = Path.Join(outputPath, $"{atlasName}.json");
            using TextureAtlas atlas = new(atlasName, atlasId);

            atlas.PopulateAtlas(GetCoordinatesFromPacker(packer, atlasId, inputPath));

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

                var asset = new SpriteAsset(
                        FileHelper.GuidFromName(image.Name),
                        AtlasId.Static,
                        image.Name,
                        ImmutableArray.Create(string.Empty),
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
                    //GameLogger.Verify(node.Texture != null, "Atlas node has no texture info");
                    if (node.Texture == null)
                        continue;

                    string name = FileHelper.GetPathWithoutExtension(Path.GetRelativePath(sourcesPath, node.Texture.Source)).EscapePath()
                        + (node.Texture.HasSlices ? $"_{(node.Texture.SliceName)}" : string.Empty)
                        + (node.Texture.HasLayers ? $"_{node.Texture.LayerName}" : "")
                        + (node.Texture.IsAnimation ? $"_{node.Texture.Frame:0000}" : "");

                    AtlasCoordinates coord = new AtlasCoordinates(
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
    }
}
