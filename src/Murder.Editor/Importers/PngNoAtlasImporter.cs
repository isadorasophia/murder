using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Serialization;

namespace Murder.Editor.Importers
{
    [ImporterSettings(FilterType.OnlyTheseFolders, [RelativeDirectory], [".png"])]
    public class PngNoAtlasImporter : ResourceImporter
    {
        private const string RelativeDirectory = "no_atlas";

        public override string RelativeSourcePath => RelativeDirectory;
        public override string RelativeOutputPath => "images";
        public override string RelativeDataOutputPath => string.Empty;

        public override bool SupportsAsyncLoading => true;

        public PngNoAtlasImporter(EditorSettingsAsset editorSettings) : base(editorSettings) { }

        public override ValueTask LoadStagedContentAsync(bool reload, bool skipIfNoChangesFound)
        {
            string sourcePath = GetRawResourcesPath();
            string outputPath = GetSourcePackedPath();
            string binPath = GetBinPackedPath();

            int skippedFiles = AllFiles.Count - ChangedFiles.Count;

            FileManager.GetOrCreateDirectory(outputPath);

            if (AllFiles.Count == 0)
            {
                return default;
            }

            if (ChangedFiles.Count == 0 && reload)
            {
                return default;
            }

            if (reload)
            {
                foreach (var image in ChangedFiles)
                {
                    CopyImage(sourcePath, outputPath, image);
                    CopyImage(sourcePath, binPath, image);
                }

                if (skippedFiles > 0)
                {
                    GameLogger.Log($"Png(no-atlas) importer skipped {skippedFiles} files because they were not modified.");
                }

                if (ChangedFiles.Count > 0)
                {
                    CopyOutputToBin = true;
                }

                Game.Data.ClearUniqueTextures();
            }
            else
            {
                // Cleanup folder for the new assets
                FileManager.DeleteContent(outputPath, deleteRootFiles: true);

                foreach (var image in AllFiles)
                {
                    CopyImage(sourcePath, outputPath, image);
                }

                if (AllFiles.Count > 0)
                {
                    CopyOutputToBin = true;
                }
            }

            GameLogger.Log($"Png(no-atlas) importer loaded {ChangedFiles.Count} files.");
            return default;
        }

        private void CopyImage(string sourcePath, string outputPath, string image)
        {
            var target = Path.Join(outputPath, Path.GetRelativePath(sourcePath, image));
            File.Copy(image, target, true);

            if (Verbose)
            {
                GameLogger.Log($"Copied {image} to {target}");
            }
        }

        public override string GetSourcePackedAtlasDescriptorPath() => string.Empty;
    }
}