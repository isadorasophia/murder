using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Serialization;
using Murder.Utilities;

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
        /// EditorDataManager will copy the output from <see cref="RelativeDataOutputPath"/> and <see cref="RelativeOutputPath"/> to bin (if any)
        /// </summary>
        public bool CopyOutputToBin = false;

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
        
        /// <summary>
        /// The path to raw sources folder. Usually /resources/ + <see cref="RelativeSourcePath"/>
        /// </summary>
        public string GetFullSourcePath(EditorSettingsAsset editorSettings) => FileHelper.GetPath(editorSettings.RawResourcesPath, RelativeSourcePath);

        /// <summary>
        /// The path to the packed resources folder. Usually /src/GameName/resources/ + <see cref="RelativeDataOutputPath"/>
        /// </summary>
        public string GetFullOutputPath(EditorSettingsAsset editorSettings) => FileHelper.GetPath(editorSettings.SourcePackedPath, RelativeOutputPath);

        // Is this too hardcoded? Maybe this should be a responsability of EditorSettings
        /// <summary>
        /// The path of the assets folder. Usually /src/GameName/resources/assets/data/ + <see cref="RelativeDataOutputPath"/>
        /// </summary>
        public string GetFullDataPath(EditorSettingsAsset editorSettings) => FileHelper.GetPath(editorSettings.SourceResourcesPath, "assets", "data", "Generated", RelativeDataOutputPath);
        
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
