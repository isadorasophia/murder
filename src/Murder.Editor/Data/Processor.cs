using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Utilities;

namespace Murder.Editor.Data
{
    public static class Processor
    {
        public static void Pack(string sourcesPath, string destinationPath, AtlasId atlasId, bool force)
        {
            var atlasName = atlasId.GetDescription();
            string atlasDescriptorName = Path.Join(destinationPath, Game.Profile.AtlasFolderName, $"{atlasName}.json");
            
            if (!force && !ShouldRecalculate(sourcesPath, atlasDescriptorName))
            {
                GameLogger.Log($"No changes found for {atlasName} atlas!", Game.Profile.Theme.Accent);

                return;
            }
            var timeStart = DateTime.Now;

            FileHelper.GetOrCreateDirectory(destinationPath);

            var packer = new Packer();
            packer.Process(sourcesPath, 4096, 1, false);
            (int atlasCount, int maxWidth, int maxHeight) = packer.SaveAtlasses(Path.Join(destinationPath, Game.Profile.AtlasFolderName, $"{atlasName}.txt"));

            using TextureAtlas atlas = new(atlasName, atlasId);
            atlas.PopulateAtlas(PopulateAtlas(packer, atlasId, sourcesPath));

            if (atlas.CountEntries == 0)
                GameLogger.Error($"I did't find any content to pack! ({sourcesPath})");
            
            // Save atlas descriptor
            FileHelper.SaveSerialized(atlas, atlasDescriptorName);


            // Create animation asset files
            for (int i = 0; i < packer.AsepriteFiles.Count; i++)
            {
                var animation = packer.AsepriteFiles[i];
                foreach (var asset in animation.CreateAssets())
                {
                    var asepritePath = Path.Join(destinationPath, asset.SaveLocation);

                    // Clear aseprite animation folders
                    if (i == 0)
                    {
                        FileHelper.DeleteDirectoryIfExists(asepritePath);
                        FileHelper.GetOrCreateDirectory(Path.Join(asepritePath, ""));
                    }

                    FileHelper.SaveSerialized(asset, Path.Join(asepritePath, $"{asset.Name}.json"));
                }
            }


            GameLogger.Log($"Packing '{atlas.Name}'({atlasCount} images, {maxWidth}x{maxHeight}) complete in {(DateTime.Now - timeStart).TotalSeconds}s with {atlas.CountEntries} entries", Game.Profile.Theme.Accent);
        }

        private static bool ShouldRecalculate(string sourceRootPath, string atlasResultJsonPath)
        {
            if (!File.Exists(atlasResultJsonPath))
            {
                // Atlas have not been created, repopulate!
                return true;
            }

            if (FileHelper.TryGetLastWrite(sourceRootPath) is DateTime lastSourceModified)
            {
                DateTime lastDestinationCreated = File.GetLastWriteTime(atlasResultJsonPath);
                return lastSourceModified > lastDestinationCreated;
            }

            GameLogger.Warning("Unable to get last write time of source root path!");
            return false;
        }

        private static IEnumerable<(string id, AtlasTexture coord)> PopulateAtlas(Packer packer, AtlasId atlasId, string sourcesPath){

            for (int i = 0; i < packer.Atlasses.Count; i++)
            {
                foreach (var node in packer.Atlasses[i].Nodes)
                {
                    //GameLogger.Verify(node.Texture != null, "Atlas node has no texture info");
                    if (node.Texture == null)
                        continue;

                    string name = FileHelper.GetPathWithoutExtension(Path.GetRelativePath(sourcesPath, node.Texture.Source)).EscapePath()
                        + (node.Texture.HasLayers ? $"_{node.Texture.LayerName}" : "")
                        + (node.Texture.IsAnimation ? $"_{node.Texture.Frame:0000}" : "");
                    AtlasTexture coord = new AtlasTexture(
                        name:           name,
                        atlasId:        atlasId,
                        atlasRectangle: new IntRectangle(node.Bounds.X, node.Bounds.Y, node.Bounds.Width, node.Bounds.Height),
                        trimArea:       node.Texture.CroppedBounds,
                        originalSize:   node.Texture.OriginalSize,
                        atlasIndex:     i,
                        atlasWidth:     packer.Atlasses[i].Width,
                        atlasHeight:    packer.Atlasses[i].Height
                    );

                    yield return (id: name, coord: coord);
                }
            }
        }

    }
}