using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Utilities;

namespace Murder.Editor.Data
{
    public static class Processor
    {
        /// <summary>
        /// This will get all the images in <paramref name="rawResourcesPath"/> and output the atlas in
        /// <paramref name="sourcePackedPath"/> and <paramref name="binPackedPath"/>.
        /// </summary>
        public static void Pack(string rawResourcesPath, string sourcePackedPath, string binPackedPath, AtlasId atlasId, bool force)
        {
            GameLogger.Verify(Path.IsPathRooted(rawResourcesPath) && Path.IsPathRooted(sourcePackedPath));

            if (!Directory.Exists(rawResourcesPath))
            {
                // If there is no content to be packed, make sure we load the atlas.
                _ = Game.Data.TryFetchAtlas(atlasId);

                // Skip empty atlas.
                return;
            }

            string atlasSourceDirectoryPath = Path.Join(sourcePackedPath, Game.Profile.AtlasFolderName);

            string atlasName = atlasId.GetDescription();
            string atlasDescriptorName = Path.Join(atlasSourceDirectoryPath, $"{atlasName}.json");
            
            // First, check if there are any changes that require an atlas repack.
            if (!force && !ShouldRecalculate(rawResourcesPath, atlasDescriptorName))
            {
                GameLogger.Log($"No changes found for {atlasName} atlas!", Game.Profile.Theme.Accent);

                return;
            }

            var timeStart = DateTime.Now;

            // Make sure our target exists.
            FileHelper.GetOrCreateDirectory(sourcePackedPath);

            Packer packer = new();
            packer.Process(rawResourcesPath, 4096, 1, false);

            (int atlasCount, int maxWidth, int maxHeight) = packer.SaveAtlasses(
                Path.Join(atlasSourceDirectoryPath, atlasName));

            using TextureAtlas atlas = new(atlasName, atlasId);
            atlas.PopulateAtlas(PopulateAtlas(packer, atlasId, rawResourcesPath));

            if (atlas.CountEntries == 0)
            {
                GameLogger.Error($"I did't find any content to pack! ({rawResourcesPath})");
            }

            // Make sure we also have the atlas save at the binaries path.
            string atlasBinDirectoryPath = Path.Join(binPackedPath, Game.Profile.AtlasFolderName);
            _ = FileHelper.GetOrCreateDirectory(atlasBinDirectoryPath);
            
            // Save atlas descriptor at the source and binaries directory.
            FileHelper.SaveSerialized(atlas, atlasDescriptorName);
            FileHelper.DirectoryDeepCopy(atlasSourceDirectoryPath, atlasBinDirectoryPath);

            // Create animation asset files
            for (int i = 0; i < packer.AsepriteFiles.Count; i++)
            {
                var animation = packer.AsepriteFiles[i];
                
                foreach (var asset in animation.CreateAssets(atlasId))
                {
                    string sourceAsepritePath = asset.GetEditorAssetPath()!;
                    string binAsepritePath = asset.GetEditorAssetPath(useBinPath: true)!;

                    // Clear aseprite animation folders
                    if (i == 0)
                    {
                        // Make sure we keep our bin directory clean.
                        FileHelper.DeleteDirectoryIfExists(binAsepritePath);
                        FileHelper.DeleteDirectoryIfExists(sourceAsepritePath);

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