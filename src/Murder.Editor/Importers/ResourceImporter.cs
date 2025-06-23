using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Editor.Assets;
using Murder.Editor.Data;
using Murder.Editor.Utilities;
using Murder.Serialization;

namespace Murder.Editor.Importers
{
    public abstract class ResourceImporter
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
        public bool HasChanges
        {
            get
            {
                if (ChangedFiles.Count > 0)
                {
                    return true;
                }

                // Otherwise, this will process all files if no descriptor has been created before.
                string path = GetSourcePackedAtlasDescriptorPath();
                if (!string.IsNullOrEmpty(path) && !File.Exists(path))
                {
                    return true;
                }

                return false;
            }
        }

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
        public abstract ValueTask LoadStagedContentAsync(bool reload, bool skipIfNoChangesFound);

        /// <summary>
        /// Flush changes and populate atlas with the file content. Only implemented when <see cref="SupportsAsyncLoading"/>. 
        /// </summary>
        internal virtual void Flush() { }

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
        /// The rooted path of the assets folder. Usually /src/GameName/resources/assets/data/Generated/ + <see cref="RelativeDataOutputPath"/>
        /// </summary>
        public string GetSourceResourcesPath() => FileHelper.GetPath(_editorSettings.SourceResourcesPath, "assets", "data", "Generated", RelativeDataOutputPath);

        public string GetBinPackedPath() => FileHelper.GetPath(_editorSettings.BinResourcesPath, RelativeOutputPath);

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

            // Track any optional extra implementation.
            StageFileImpl(file, changed);
        }

        protected virtual void StageFileImpl(string file, bool changed) { }

        public bool ShouldRecalculate()
        {
            string atlasPath = GetSourcePackedAtlasDescriptorPath();
            if (string.IsNullOrEmpty(atlasPath))
            {
                return FileLoadHelpers.ShouldRecalculate(GetRawResourcesPath(), _editorSettings.LastImported);
            }

            return FileLoadHelpers.ShouldRecalculate(GetRawResourcesPath(), GetSourcePackedAtlasDescriptorPath());
        }

        /// <summary>
        /// Populate an atlas based on the packer information of the images database.
        /// </summary>
        /// <param name="packer">Packer information (previously initialized with the files).</param>
        /// <param name="atlasId">Atlas identifier.</param>
        /// <param name="sourcesPath">Root sources path. Used to build the relative path of each final file in the atlas.</param>
        protected static IEnumerable<(string id, AtlasCoordinates coord)> GetCoordinatesForAtlas(Packer packer, string atlasId, string sourcesPath)
        {
            for (int i = 0; i < packer.Atlasses.Count; i++)
            {
                foreach (var node in packer.Atlasses[i].Nodes)
                {
                    if (node.Texture == null)
                    {
                        continue;
                    }

                    string name = FileHelper.GetPathWithoutExtension(Path.GetRelativePath(sourcesPath, node.Texture.Source)).EscapePath()
                        + (node.Texture.HasSlices ? $"_{(node.Texture.SliceName)}" : string.Empty)
                        + (node.Texture.HasLayers ? $"_{node.Texture.LayerName}" : "")
                        + (node.Texture.IsAnimation ? $"_{node.Texture.Frame:0000}" : "");

                    AtlasCoordinates coord = new(
                            name: name,
                            atlasId: atlasId,
                            atlasRectangle: new IntRectangle(node.Bounds.X, node.Bounds.Y, node.Bounds.Width, node.Bounds.Height),
                            trimArea: node.Texture.TrimArea,
                            size: node.Texture.SliceSize,
                            atlasIndex: i,
                            atlasWidth: packer.Atlasses[i].Width,
                            atlasHeight: packer.Atlasses[i].Height
                        );

                    yield return (id: name, coord);
                }
            }
        }
    }
}