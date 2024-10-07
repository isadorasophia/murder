using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Serialization;

namespace Murder.Editor.Data
{
    public static class Processor
    {
        public static void CleanDirectory(string sourceDirectoryPath, string binDirectoryPath)
        {
            if (Directory.Exists(sourceDirectoryPath))
                foreach (string file in Directory.GetFiles(sourceDirectoryPath))
                {
                    File.Delete(file.Replace(sourceDirectoryPath, binDirectoryPath));
                }

            FileManager.DeleteDirectoryIfExists(sourceDirectoryPath);
        }

        private static IEnumerable<(string id, AtlasCoordinates coord)> PopulateAtlas(Packer packer, string atlasId, string sourcesPath)
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