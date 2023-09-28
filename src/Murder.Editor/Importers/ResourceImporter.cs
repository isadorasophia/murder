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

        public abstract string OutputFolder { get; }

        public bool Verbose = false;

        /// <summary>
        /// Loads this importer's content into the "Generated/<see cref="OutputFolder"/>" folder.
        /// It's expected that you should perform a Clean Import before shipping your game.
        /// </summary>
        internal abstract void LoadStagedContent(EditorSettingsAsset editorSettings, bool cleanImport);


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
