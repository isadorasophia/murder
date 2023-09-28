using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Serialization;

namespace Murder.Editor.Importers
{
    internal abstract class ResourceImporter
    {
        /// <summary>
        /// Not supported yet
        /// </summary>
        public List<string> DeletedFiles = new();

        public List<string> ChangedFiles = new();
        public List<string> AllFiles = new();

        /// <summary>
        /// Source path of the raw resources, relative to the game's resource folder
        /// </summary>
        public abstract string RelativeSourcePath { get; }
        
        /// <summary>
        /// Output path for the imported resources, relative to the game's packed resources folder
        /// </summary>
        public abstract string RelativeOutputPath { get; }


        /// <summary>
        /// Output path for the generated assets, relative to the "Generated" assets data folder
        /// </summary>
        public abstract string RelativeDataOutputPath { get; }


        public bool Verbose = false;

        /// <summary>
        /// Loads this importer's content into the "Generated/<see cref="RelativeOutputPath"/>" folder.
        /// It's expected that you should perform a Clean Import before shipping your game.
        /// </summary>
        internal abstract ValueTask LoadStagedContentAsync(EditorSettingsAsset editorSettings, bool cleanImport);

        protected string GetFullSourcePath(EditorSettingsAsset editorSettings) => FileHelper.GetPath(editorSettings.RawResourcesPath, RelativeSourcePath);
        protected string GetFullOutputPath(EditorSettingsAsset editorSettings) => FileHelper.GetPath(editorSettings.SourcePackedPath, RelativeOutputPath);
        protected string GetFullDataPath() => FileHelper.GetPath(Game.Profile.GenericAssetsPath, "Generated", RelativeOutputPath);
        internal void ClearStage()
        {
            ChangedFiles.Clear();
            AllFiles.Clear();
        }

        internal void StageDeletedFile(string file)
        {
            DeletedFiles.Add(file);
        }

        internal void StageFile(string file, bool changed)
        {
            if (changed)
            {
                ChangedFiles.Add(file);
            }
            AllFiles.Add(file);
        }
    }
}
