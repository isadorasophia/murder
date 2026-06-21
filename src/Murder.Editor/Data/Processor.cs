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
    }
}