using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Data;
using Murder.Serialization;
using System.Collections.Immutable;

namespace Murder.Editor.Importers
{
    [ImporterSettings(FilterType.OnlyTheseFolders, new string[] { "images" }, new string[] { ".png" })]

    internal class PngImporter : ResourceImporter
    {
        public override string RelativeSourcePath => "images";
        public override string RelativeOutputPath => "atlas_static";
        public override string RelativeDataOutputPath => "sprites_static";

        internal override ValueTask LoadStagedContentAsync(EditorSettingsAsset editorSettings, bool cleanImport)
        {
            string sourcePath = GetFullSourcePath(editorSettings);
            string outputPath = GetFullOutputPath(editorSettings);
            string dataPath = GetFullDataPath();
            
            // Quick import not implemented yet
            
            cleanImport = true;
            
            if (cleanImport)
            {
                // Cleanup generated assets folder
                FileHelper.DeleteDirectoryIfExists(dataPath);

                FileHelper.GetOrCreateDirectory(outputPath);

                PackImages(AllFiles.ToImmutableArray(), outputPath);

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
                
                throw new NotImplementedException("Quick load not implemented yet!");
            }



            return default;
        }

        private void PackImages(ImmutableArray<string> allFiles, string outputFolder)
        {
            Packer packer = new();
            packer.Process(allFiles, 4096, 1, false);
            packer.SaveAtlasses(Path.Join(outputFolder, "atlas"));
        }
    }
}
