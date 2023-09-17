﻿using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Editor.Data;
using Murder.Editor.Systems;
using Murder.Systems.Graphics;
using Murder.Systems;
using Newtonsoft.Json;
using System.Collections.Immutable;
using Murder.Assets.Graphics;
using Murder.Editor.Systems.Debug;
using System.ComponentModel;
using Bang.StateMachines;
using Murder.Diagnostics;

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

        /// <summary>
        /// The asset currently being shown in the editor scene.
        /// </summary>
        [HideInEditor]
        public Guid? LastOpenedAsset = null;

        [GameAssetId(typeof(WorldAsset)), Tooltip("Use Shift+F5 to start here")]
        public Guid QuickStartScene;

        public bool OnlyReloadAtlasWithChanges = true;

        public string IgnoredTexturePackingExtensions = ".clip,.psd,.gitkeep";

        [JsonProperty]
        private ImmutableArray<(Type systemType, bool isActive)> _editorSystems = ImmutableArray<(Type systemType, bool isActive)>.Empty;

        /// <summary>
        /// These are all the systems the editor currently supports.
        /// </summary>
        public ImmutableArray<(Type systemType, bool isActive)> EditorSystems => _editorSystems;

        public bool UseCustomShadersOnEditor = false;

        /// <summary>
        /// The default floor tiles to use when creating a new room.
        /// </summary>
        [GameAssetId(typeof(FloorAsset))]
        public Guid DefaultFloor;

        [JsonProperty, HideInEditor]
        internal float FontScale;

        [JsonProperty]
        internal string AsepritePath = "Aseprite";
        [JsonProperty]
        internal bool SaveAsepriteInfoOnSpriteAsset = false;

        [JsonProperty]
        [Tooltip("Path for the lua scripts relative to RawResourcesPath.")]
        internal string LuaScriptsPath = "lua";

        [JsonProperty, HideInEditor]
        internal readonly Dictionary<Guid, PersistStageInfo> CameraPositions = new();

        public void UpdateSystems(ImmutableArray<(Type systemType, bool isActive)> systems) => _editorSystems = systems;

        public EditorSettingsAsset(string name)
        {
            FilePath = EditorDataManager.EditorSettingsFileName;
            Name = EditorDataManager.EditorSettingsFileName;

            GameSourcePath = $"../../../../{name}";

            _editorSystems = ImmutableArray.Create<(Type systemType, bool isActive)>(
                (typeof(EditorStartOnCursorSystem), true),
                (typeof(EditorSystem), true),
                (typeof(TileEditorSystem), false),
                (typeof(EditorCameraControllerSystem), true),
                (typeof(SpriteRenderDebugSystem), true),
                (typeof(DebugColliderRenderSystem), true),
                (typeof(CursorSystem), true),
                (typeof(TilemapRenderSystem), true),
                (typeof(TextBoxRenderSystem), true),
                (typeof(RectangleRenderSystem), true),
                (typeof(RectPositionDebugRenderer), true),
                (typeof(UpdatePositionSystem), true),
                (typeof(UpdateColliderSystem), true),
                (typeof(StateMachineSystem), false),
                (typeof(CustomDrawRenderSystem), true),
                (typeof(UpdateTileGridSystem), false),
                (typeof(EntitiesPlacerSystem), true),
                (typeof(DebugShowInteractionsSystem), true),
                (typeof(DebugShowCameraBoundsSystem), true),
                (typeof(CutsceneEditorSystem), false),
                (typeof(UpdateAnchorSystem), false),
                (typeof(EditorFloorRenderSystem), true),
                (typeof(ParticleRendererSystem), true),
                (typeof(DebugParticlesSystem), true),
                (typeof(ParticleDisableTrackerSystem), true),
                (typeof(ParticleTrackerSystem), true),
                (typeof(SpriteThreeSliceRenderSystem), true),
                (typeof(DialogueNodeSystem), false),
                (typeof(StoryEditorSystem), false),
                (typeof(PolygonSpriteRenderSystem), true)
                );
        }

        // ===========================================
        // Editor properties for hooking start commands
        // ===========================================

        /// <summary>
        /// This is a property used when creating hooks within the editor to quickly test a scene.
        /// TODO: Move this to save, eventually? Especially if this is a in-game feature at some point.
        /// </summary>
        [JsonIgnore, HideInEditor]
        public Point? TestWorldPosition;

        [JsonProperty, HideInEditor]
        public float? TestStartTime;

        [JsonIgnore, HideInEditor]
        public bool UseCustomCutscene = false;

        [JsonProperty, HideInEditor]
        public (Guid Entity, IStateMachineComponent? Component)? TestStartWithEntityAndComponent;

        public override void AfterDeserialized()
        {
            bool changed = false;
            for (int i = 0; i < _editorSystems.Length; ++i)
            {
                if (_editorSystems[i].systemType is null)
                {
                    // Remove the problematic system from the editor.
                    _editorSystems = _editorSystems.RemoveAt(i);
                    i--;

                    changed = true;
                }
            }

            if (changed)
            {
                GameLogger.Warning("\uf0ad Fixed an issue found in editor profile! It shouldn't happen again.");
            }
        }
    }
}
