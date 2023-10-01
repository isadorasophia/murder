using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Utilities;
using Murder.Serialization;

namespace Murder.Editor.Importers
{
    internal abstract class ResourceImporter
    {
        /// <summary>
        /// Not supported yet. This tracks the deleted files.
        /// </summary>
        public List<string> DeletedFiles = new();

        public List<string> ChangedFiles = new();
        public List<string> AllFiles = new();

        /// <summary>
        /// EditorDataManager will copy the output from <see cref="RelativeDataOutputPath"/> and <see cref="RelativeOutputPath"/> to bin (if any)
        /// </summary>
        public bool CopyOutputToBin = false;

        public bool Verbose = false;

        /// <summary>
        /// Whether this resource importer should be run asynchronously and does not require the main thread.
        /// </summary>
        public bool HasChanges => ChangedFiles.Count > 0;

        /// <summary>
        /// Whether this resource importer should be run asynchronously and does not require the main thread.
        /// </summary>
        public virtual bool SupportsAsyncLoading => false;

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

        /// <summary>
        /// Loads this importer's content into the "Generated/<see cref="RelativeOutputPath"/>" folder.
        /// It's expected that you should perform a Clean Import before shipping your game.
        /// </summary>
        internal abstract ValueTask LoadStagedContentAsync(bool clean);

        public abstract string GetSourcePackedAtlasDescriptorPath();

        /// <summary>
        /// The rooted path to raw sources folder. Usually /resources/ + <see cref="RelativeSourcePath"/>
        /// </summary>
        public string GetRawResourcesPath() => FileHelper.GetPath(_editorSettings.RawResourcesPath, RelativeSourcePath);

        /// <summary>
        /// The rooted path to the packed resources folder. Usually /src/GameName/resources/ + <see cref="RelativeDataOutputPath"/>
        /// </summary>
        public string GetSourcePackedPath() => FileHelper.GetPath(_editorSettings.SourcePackedPath, RelativeOutputPath);

        // Is this too hardcoded? Maybe this should be a responsibility of EditorSettings
        /// <summary>
        /// The rooted path of the assets folder. Usually /src/GameName/resources/assets/data/ + <see cref="RelativeDataOutputPath"/>
        /// </summary>
        public string GetSourceResourcesPath() => FileHelper.GetPath(_editorSettings.SourceResourcesPath, "assets", "data", "Generated", RelativeDataOutputPath);

        public string GetBinPackedPath() => FileHelper.GetPath(_editorSettings.BinResourcesPath);

        protected readonly EditorSettingsAsset _editorSettings;

        public ResourceImporter(EditorSettingsAsset editorSettings)
        {
            _editorSettings = editorSettings;
        }

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

        public bool ShouldRecalculate()
        {
            string atlasPath = GetSourcePackedAtlasDescriptorPath();
            if (string.IsNullOrEmpty(atlasPath))
            {
                return FileLoadHelpers.ShouldRecalculate(GetRawResourcesPath(), _editorSettings.LastImported);
            }

            return FileLoadHelpers.ShouldRecalculate(GetRawResourcesPath(), GetSourcePackedAtlasDescriptorPath());
        }
    }
}