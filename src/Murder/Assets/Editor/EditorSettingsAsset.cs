using Bang;
using Bang.StateMachines;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Editor.Assets;

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
    /// This points to the resources raw path, before we get to process the contents to <see cref="SourceResourcesPath"/>.
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

    /// <summary>
    /// The time of the last resource import.
    /// </summary>
    [HideInEditor]
    public DateTime LastImported = DateTime.MinValue;

    /// <summary>
    /// The time of the last resource import with hot reload.
    /// </summary>
    [HideInEditor]
    public DateTime LastHotReloadImport = DateTime.MinValue;

    /// <summary>
    /// The time of the last resource import with event manager.
    /// </summary>
    [HideInEditor]
    public DateTime LastMetadataImported = DateTime.MinValue;

    [GameAssetId(typeof(WorldAsset)), Tooltip("Use Shift+F5 to start here")]
    public Guid QuickStartScene;

    public bool AlwaysBuildAtlasOnStartup = false;

    public string IgnoredTexturePackingExtensions = ".clip,.psd,.gitkeep";

    /// <summary>
    /// The default floor tiles to use when creating a new room.
    /// </summary>
    [GameAssetId(typeof(FloorAsset))]
    public Guid DefaultFloor;

    [Serialize, HideInEditor]
    public float FontScale = 1;

    [Serialize, Slider(1,2000)]
    public float WasdCameraSpeed = 100;

    [Serialize]
    public string AsepritePath = "Aseprite";

    [Serialize]
    public bool SaveAsepriteInfoOnSpriteAsset = false;

    [Serialize]
    [Tooltip("Path for the lua scripts relative to RawResourcesPath.")]
    public string LuaScriptsPath = "lua";

    [Serialize]
    [Tooltip("Custom path for fxc.exe, if applicable")]
    public string? FxcPath = null;

    [Serialize, HideInEditor]
    public readonly Dictionary<Guid, PersistStageInfo> StageInfo = new();

    [Serialize, HideInEditor]
    public readonly Dictionary<Guid, PersistWorldStageInfo> WorldAssetInfo = new();

    [Serialize, HideInEditor]
    private readonly HashSet<Guid> _favoriteAssets = new();

    public ImmutableHashSet<Guid> FavoriteAssets => [.. _favoriteAssets];

    private readonly List<GameAsset> _cachedFavoriteAssets = new();

    public List<GameAsset> CachedFavoriteAssets
    {
        get
        {
            if (_cachedFavoriteAssets.Count != FavoriteAssets.Count)
            {
                _cachedFavoriteAssets.Clear();
                foreach (Guid guid in FavoriteAssets)
                {
                    GameAsset? asset = Game.Data.TryGetAsset(guid);
                    if (asset is not null)
                    {
                        _cachedFavoriteAssets.Add(asset);
                    }
                    else
                    {
                        //Invalid asset, remove from favorites
                        FavoriteAssets.Remove(guid);
                    }
                }
            }

            return _cachedFavoriteAssets;
        }
    }

    [Tooltip("Whether an asset should be overriden (by a save) after an error loading it")]
    [Serialize]
    public bool SaveDeserializedAssetOnError = false;

    [Tooltip("Whether we will automatically apply any chances made to shaders")]
    public bool AutomaticallyHotReloadShaderChanges = false;

    [Tooltip("Whether we will automatically apply any chances made to dialogues")]
    public bool EnableDialogueHotReload = false;

    [JsonConstructor]
    public EditorSettingsAsset(string name, string gameSourcePath)
    {
        Name = name;
        FilePath = name;

        GameSourcePath = gameSourcePath;
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

    [Serialize, HideInEditor]
    public float? TestStartTime;

    [JsonIgnore, HideInEditor]
    public bool UseCustomCutscene = false;

    [Serialize, HideInEditor]
    public (Guid Entity, IStateMachineComponent? Component)? TestStartWithEntityAndComponent;

    public float DpiScale = 1f;

    public void FavoriteAsset(Guid guid)
    {
        _favoriteAssets.Add(guid);
        _cachedFavoriteAssets.Clear();
    }

    public void UnfavoriteAsset(Guid guid)
    {
        _favoriteAssets.Remove(guid);
        _cachedFavoriteAssets.Clear();
    }
}