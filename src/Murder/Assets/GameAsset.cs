using Murder.Attributes;
using Murder.Serialization;
using Murder.Utilities;
using Newtonsoft.Json;
using System.Numerics;

namespace Murder.Assets
{
    [Serializable]
    public abstract class GameAsset
    {
        public const char SkipDirectoryIconCharacter = '#';
            
        [HideInEditor]
        public string Name { get; set; } = string.Empty;

        /** Cache strings **/
        private string[]? _nameSplit = null;

        public string[] GetSplitNameWithEditorPath() =>
            _nameSplit ??= Path.Combine(EditorFolder, Name).Split('\\', '/');

        private string? _simplifiedName = null;

        public string GetSimplifiedName() =>
            _simplifiedName ??= GetSplitNameWithEditorPath().Last();

        /** **/

        private string _filePath = string.Empty;

        /// <summary>
        /// Path to this asset file, relative to its base directory where this asset is stored.
        /// </summary>
        [JsonIgnore]
        public string FilePath
        {
            get => _filePath.ToLowerInvariant();
            set
            {
                _filePath = value.EscapePath().ToLowerInvariant();

                _nameSplit = null;
                _simplifiedName = null;
            }
        }

        [HideInEditor]
        [JsonProperty]
        public Guid Guid { get; protected set; }

        private bool _fileChanged = false;

        [JsonIgnore, HideInEditor]
        public bool FileChanged 
        { 
            get => _fileChanged; 
            set
            {
                _fileChanged = value;
                OnModified();
            }
        }

        private bool _rename = false;

        /// <summary>
        /// Whether it should rename the file and delete the previous name.
        /// </summary>
        [JsonIgnore, HideInEditor]
        public bool Rename 
        {
            get => _rename; 
            set
            {
                _rename = value;
                FileChanged = value;
            }
        }

        public virtual char Icon => '\uf187';
        public virtual string EditorFolder => string.Empty;
        public virtual Vector4 EditorColor => Game.Profile.Theme.White;
        public virtual bool CanBeRenamed => true;
        public virtual bool CanBeDeleted => true;
        public virtual bool CanBeCreated => true;
        public virtual bool CanBeSaved => true;

        public virtual string SaveLocation => Path.Join(Game.Profile.GenericAssetsPath, FileHelper.Clean(EditorFolder));

        /// <summary>
        /// Whether this file is saved relative do the save path.
        /// </summary>
        public virtual bool IsStoredInSaveData => false;

        /// <summary>
        /// Whether this file should be stored following a database hierarchy of the files.
        /// True by default.
        /// </summary>
        public virtual bool StoreInDatabase => true;

        [JsonIgnore]
        public bool TaggedForDeletion = false;

        public virtual void AfterDeserialized() { }

        public void MakeGuid()
        {
            Guid = Guid.NewGuid();
        }

        /// <summary>
        /// Create a duplicate of the current asset.
        /// </summary>
        public GameAsset Duplicate(string name)
        {
            GameAsset asset = SerializationHelper.DeepCopy(this);

            asset.Name = name;
            asset.MakeGuid();

            return asset;
        }

        /// <summary>
        /// Implemented by assets that may cache data.
        /// This notifies it that it has been modified (usually by an editor).
        /// </summary>
        protected virtual void OnModified() { }
    }
}
