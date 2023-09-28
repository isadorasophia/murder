using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Serialization;

namespace Murder.Editor.Importers
{
    [ImporterSettings(FilterType.OnlyTheseFolders, new string[] { "no_atlas" }, new string[] { ".png" })]
    internal class PngNoAtlasImporter : ResourceImporter
    {
        public override string OutputFolder => "images";

        internal override void LoadStagedContent(EditorSettingsAsset editorSettings, bool forceAll)
        {
            var sourcePath = FileHelper.GetPath(editorSettings.RawResourcesPath, "no_atlas");
            string outputPath = FileHelper.GetPath(Path.Join(editorSettings.SourceResourcesPath, "images"));

            int skippedFiles = AllFiles.Count - ChangedFiles.Count;

            FileHelper.GetOrCreateDirectory(outputPath);

            if (!forceAll)
            {
                foreach (var image in ChangedFiles)
                {
                    LoadImage(sourcePath, outputPath, image);
                }

                if (skippedFiles > 0)
                {
                    GameLogger.Log($"Png importer skipped {skippedFiles} files because they were not modified.");
                }
            }
            else
            {
                // Cleanup folder for the new assets
                FileHelper.DeleteContent(outputPath, deleteRootFiles: true);
                
                foreach (var image in AllFiles)
                {
                    LoadImage(sourcePath, outputPath, image);
                }

            }
            GameLogger.Log($"Png importer loaded {ChangedFiles.Count} files.");

            // Make sure we are sending this to the bin folder!
            string noAtlasImageBinPath = FileHelper.GetPath(Path.Join(editorSettings.BinResourcesPath, "/images/"));
            FileHelper.DirectoryDeepCopy(outputPath, noAtlasImageBinPath);
        }

        private void LoadImage(string sourcePath, string outputPath, string image)
        {
            var target = Path.Join(outputPath, Path.GetRelativePath(sourcePath, image));
            File.Copy(image, target, true);

            if (Verbose)
            {
                GameLogger.Log($"Copied {image} to {target}");
            }
        }
    }
}
