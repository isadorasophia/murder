using Bang;
using Murder.Attributes;
using Murder.Serialization;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Murder.Assets
{
    /// <summary>
    /// The GameAsset is the core functionality on how Murder Engine serializes and persists data of the game. Its design is made so most of its data structures should be immutable while the game is running and any field modification is left to the editor project.
    /// You can override its fields and methods to specify where how it will be displayed in the editor, e.g.EditorFolder and EditorIcon.
    /// </summary>
    /// <remarks>
    /// Usage:
    /// - Extend this class for creating specific types of game assets (e.g., InventoryItemAsset, CharacterAsset, LevelAsset).
    /// - Override virtual properties and methods to customize behavior and appearance in the editor.
    /// 
    /// Example:
    /// Here's a simple example of how to create a new asset type, 'InventoryItem', extending 'GameAsset':
    /// 
    /// <code>
    /// public class InventoryItemAsset : GameAsset
    /// {
    ///     public override char Icon => '\uf291'; // Custom icon for the inventory item
    ///     public override string EditorFolder => "#\uf291Items"; // Default folder in the editor using custom icon
    ///     public override Vector4 EditorColor => new Vector4(1, 0.5f, 0, 1); // Custom color
    ///     
    ///     // Additional properties and methods specific to InventoryItem...
    ///     public readonly string ItemName;
    ///     public readonly float Value;
    /// }
    /// </code>
    /// </remarks>
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
            get => _filePath;
            set
            {
                _filePath = value.EscapePath();

                _nameSplit = null;
                _simplifiedName = null;
            }
        }

        [HideInEditor]
        [Serialize]
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
                Game.Data.OnAssetRenamedOrAddedOrDeleted();
            }
        }
        
        /// <summary>
         /// Gets the icon character for the asset, override to provide a custom icon. Use a free FontAwesome character for this to work.
         /// </summary>
        public virtual char Icon => '\uf187';

        /// <summary>
        /// Gets the default folder path in the editor for the asset, override to specify a different folder.
        /// </summary>
        /// <remarks>
        /// Use <see cref="SkipDirectoryIconCharacter"/>(default '#') to add a custom icon to the folder. For example:<code>"#\uf57dWorld";</code>
        /// </remarks>
        public virtual string EditorFolder => string.Empty;

        /// <summary>
        /// Gets the default color used in the editor for the asset, override to use a custom color. From 0 to 1.
        /// </summary>
        public virtual Vector4 EditorColor => Game.Profile.Theme.White;

        /// <summary>
        /// Determines if the asset can be renamed, override to change this capability.
        /// </summary>
        public virtual bool CanBeRenamed => true;

        /// <summary>
        /// Determines if the asset can be deleted, override to change this capability.
        /// </summary>
        public virtual bool CanBeDeleted => true;

        /// <summary>
        /// Determines if the asset can be created, override to change this capability.
        /// </summary>
        public virtual bool CanBeCreated => true;

        /// <summary>
        /// Determines if the asset can be saved, override to change this capability.
        /// </summary>
        public virtual bool CanBeSaved => true;

        public virtual string SaveLocation => Path.Join(Game.Profile.GenericAssetsPath, FileHelper.Clean(EditorFolder));

        /// <summary>
        /// Whether this file is saved relative do the save path.
        /// </summary>
        public virtual bool IsStoredInSaveData => false;

        /// <summary>
        /// Whether this file will be packed in the save, if <see cref="IsStoredInSaveData"/> is true.
        /// </summary>
        public virtual bool IsSavePacked => false;

        /// <summary>
        /// Whether this file should be stored following a database hierarchy of the files.
        /// True by default.
        /// </summary>
        public virtual bool StoreInDatabase => true;

        [JsonIgnore]
        public bool TaggedForDeletion = false;

        /// <summary>
        /// If set, this has a list of assets which should also be saved whenever this asset has been saved.
        /// For example: localization resources or sprite event manager.
        /// </summary>
        private List<Guid>? _saveAssetsOnSave = null;
        
        /// <summary>
        /// Called after the asset is deserialized, override to implement custom post-deserialization logic.
        /// </summary>
        public virtual void AfterDeserialized() { }

        public GameAsset() { }

        /// <summary>
        /// Generates and assigns a new globally unique identifier (GUID) to the object.
        /// </summary>
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

        /// <summary>
        /// Track an asset such that <paramref name="g"/> is also saved once this asset is saved.
        /// </summary>
        public void TrackAssetOnSave(Guid g)
        {
            _saveAssetsOnSave ??= new();
            _saveAssetsOnSave.Add(g);
        }

        /// <summary>
        /// Return the assets which will be saved with this (<see cref="_saveAssetsOnSave"/>). 
        /// Also clear the pending list.
        /// </summary>
        public List<Guid>? AssetsToBeSaved()
        {
            if (_saveAssetsOnSave is null) return null;

            List<Guid> result = _saveAssetsOnSave;
            _saveAssetsOnSave = null;

            return result;
        }
    }
}