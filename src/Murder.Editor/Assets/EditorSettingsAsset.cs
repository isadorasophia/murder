using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Editor.Data;
using Murder.Editor.Systems;
using Murder.Systems.Graphics;
using Murder.Systems;
using Newtonsoft.Json;
using System.Collections.Immutable;
using Road.Editor.Systems;
using Murder.Assets.Graphics;

namespace Murder.Editor.Assets
{
    public class EditorSettingsAsset : GameAsset
    {
        public override char Icon => '\uf085';
        public override bool CanBeRenamed => false;
        public override bool CanBeDeleted => false;
        public override bool CanBeCreated => false;

        public override bool IsStoredInSaveData => true;

        public override string SaveLocation => string.Empty;

        public override bool StoreInDatabase => false;

        public string AssetNamePattern = " ({0})";
        public string NewAssetDefaultName = "New {0}";

        public bool StartOnEditor = true;

        /// <summary>
        /// This points to the directory in the bin path.
        /// </summary>
        [Tooltip("This is the path to the resources in the bin directory. Usually it is in the same folter as the executable.")]
        public string BinResourcesPath = "resources";

        /// <summary>
        /// This points to the packed directory which will be synchronized in source.
        /// </summary>
        [Tooltip("This is the path to the source game path. This expects a raw resource (../../resource), a resource (resource) and packed (packed) directory.")]
        public string GameSourcePath;

        /// <summary>
        /// This points to the packed directory which will be synchronized in source.
        /// </summary>
        public string SourcePackedPath => Path.Join(GameSourcePath, "packed");

        /// <summary>
        /// This points to the resources which will be synchronized in source.
        /// </summary>
        public string SourceResourcesPath => Path.Join(GameSourcePath, "resources");

        /// <summary>
        /// This points to the resources raw path, before we get to process the contents to <see cref="ResourcesPathPrefix"/>.
        /// </summary>
        public string RawResourcesPath => Path.Join(GameSourcePath, "../../resources");

        [HideInEditor]
        public bool StartMaximized = false;

        [HideInEditor]
        public Point WindowStartPosition = new(-1, -1);

        [HideInEditor]
        public Point WindowSize = new(-1, -1);

        [HideInEditor]
        public int Monitor = 0;

        [HideInEditor]
        public Guid[] OpenedTabs = new Guid[0];

        [HideInEditor]
        public int SelectedTab = 0;

        [GameAssetId(typeof(WorldAsset)), Tooltip("Use Shift+F5 to start here")]
        public Guid QuickStartScene;

        public bool OnlyReloadAtlasWithChanges = true;

        [JsonProperty]
        private ImmutableArray<(Type systemType, bool isActive)> _editorSystems = ImmutableArray<(Type systemType, bool isActive)>.Empty;

        /// <summary>
        /// These are all the systems the editor currently supports.
        /// </summary>
        public ImmutableArray<(Type systemType, bool isActive)> EditorSystems => _editorSystems;
        
        /// <summary>
        /// This is a property used when creating hooks within the editor to quickly test a scene.
        /// TODO: Move this to save, eventually? Especially if this is a in-game feature at some point.
        /// </summary>
        [JsonIgnore, HideInEditor]
        public Point? TestWorldPosition;

        /// <summary>
        /// The default floor tiles to use when creating a new room.
        /// </summary>
        [GameAssetId(typeof(AsepriteAsset))]
        public Guid DefaultFloor;

        [JsonProperty, HideInEditor]
        internal float FontScale;

        public void UpdateSystems(ImmutableArray<(Type systemType, bool isActive)> systems) => _editorSystems = systems;

        public EditorSettingsAsset(string name)
        {
            FilePath = EditorDataManager.EditorSettingsFileName;

            GameSourcePath = $"../../../../{name}";

            _editorSystems = ImmutableArray.Create<(Type systemType, bool isActive)>(
                (typeof(EditorSystem), true),
                (typeof(TileEditorSystem), false),
                (typeof(EditorCameraControllerSystem), true),
                (typeof(AsepriteRenderDebugSystem), true),
                (typeof(DebugColliderRenderSystem), true),
                (typeof(CursorSystem), true),
                (typeof(TextureRenderSystem), true),
                (typeof(TilemapRenderSystem), true),
                (typeof(TextBoxRenderSystem), true),
                (typeof(RectangleRenderSystem), true),
                (typeof(RectPositionDebugRenderer), true),
                (typeof(UpdatePositionSystem), true),
                (typeof(UpdateColliderSystem), true),
                (typeof(StateMachineSystem), false),
                (typeof(CustomDrawRenderSystem), true),
                (typeof(UpdateTileGridSystem), false),
                (typeof(EntitiesSelectorSystem), true),
                (typeof(EntitiesPlacerSystem), true),
                (typeof(DebugShowInteractionsSystem), true),
                (typeof(CutsceneEditorSystem), false),
                (typeof(UpdateAnchorSystem), false),
                (typeof(EditorFloorRenderSystem), true),
                (typeof(EditorStartOnCursorSystem), true));
        }
    }
}
