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
        [HideInEditor]
        public string Name { get; set; } = string.Empty;

        public string GetSimplifiedName() => Name.Split(new char[] { '\\', '/' }).Last();

        private string _filePath = string.Empty;

        [JsonIgnore]
        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value.EscapePath();
            }
        }

        [HideInEditor]
        [JsonProperty]
        public Guid Guid { get; protected set; }

        [JsonIgnore, HideInEditor]
        public bool FileChanged = false;

        public virtual char Icon => '\uf187';
        public virtual string EditorFolder => string.Empty;
        public virtual Vector4 EditorColor => Game.Profile.Theme.White;
        public virtual bool CanBeRenamed => true;
        public virtual bool CanBeDeleted => true;
        public virtual bool CanBeCreated => true;
        public virtual bool CanBeSaved => true;
        public virtual string? CustomPath => null;
        public virtual string SaveLocation => Path.Join(Game.Profile.ContentDataPath, FileHelper.Clean(EditorFolder));

        /// <summary>
        /// Whether this asset should be stored in the database.
        /// True by default.
        /// </summary>
        public virtual bool StoreInDatabase => true;

        [JsonIgnore]
        public bool TaggedForDeletion = false;

        internal virtual void AfterDeserialized() { }

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
    }
}
