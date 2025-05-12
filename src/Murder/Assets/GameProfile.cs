using Bang;
using Murder.Assets.Graphics;
using Murder.Assets.Input;
using Murder.Assets.Localization;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Data;
using System.Collections.Immutable;

namespace Murder.Assets
{
    /// <summary>
    /// Represents the game profile asset containing configuration and resource paths.
    /// </summary>
    public partial class GameProfile : GameAsset
    {
        public override char Icon => '\uf085';
        public override bool CanBeRenamed => false;
        public override bool CanBeDeleted => false;
        public override bool CanBeCreated => false;

        public override string SaveLocation => string.Empty;

        public override bool StoreInDatabase => false;

        /// <summary>Gets the game's intended aspect ratio</summary>
        public float Aspect => GameHeight / (float)GameWidth;
                 

        /// <summary>
        /// Where our atlas .png and .json files are stored.
        /// Under: 
        ///   packed/ -> bin/resources/
        ///     atlas/
        /// </summary>
        [HideInEditor]
        public readonly string AtlasFolderName = "atlas/";

        /// <summary>
        /// Where our aseprite contents are stored.
        /// Under:
        ///   packed/ -> bin/resources/
        ///     aseprite/
        /// </summary>
        [HideInEditor]
        public readonly string ContentAsepritePath = "aseprite/";

        /// <summary>
        /// Where our font contents are stored.
        /// Under:
        ///   packed/ -> bin/resources/
        ///     fonts/
        /// </summary>
        [HideInEditor]
        public readonly string FontPath = "shaders/";

        /// <summary>
        /// Where our sound contents are stored.
        /// Under:
        ///   packed/ -> bin/resources/
        ///     sounds/
        /// </summary>
        [HideInEditor]
        public readonly string SoundsPath = "sounds/";

        /// <summary>
        /// Where our dialogues contents are stored.
        /// Under:
        ///   packed/ -> bin/resources/
        ///     dialogues/
        /// </summary>
        [HideInEditor]
        public readonly string DialoguesPath = "dialogues/";

        /// <summary>
        /// Where our localization assets are stored.
        /// Under:
        ///   root/resources
        /// </summary>
        [HideInEditor]
        public readonly string LocalizationPath = "loc/";

        /// <summary>
        /// Where our aseprite contents are stored.
        /// Under:
        ///   packed/ -> bin/resources/
        ///     shaders/
        /// </summary>
        [HideInEditor]
        public readonly string ShadersPath = "shaders/";

        /// <summary>
        /// Where our sound contents are stored.
        /// Under:
        ///   packed/ -> bin/resources/
        ///     fonts/
        /// </summary>
        [HideInEditor]
        public readonly string FontsPath = "fonts/";

        /// <summary>
        /// Where our high resolution contents are stored.
        /// Under:
        ///   packed/ -> bin/resources
        ///     shaders/
        /// </summary>
        [HideInEditor]
        public readonly string HiResPath = "hires_images/";

        /// <summary>
        /// Root path where our data .json files are stored.
        /// </summary>
        public readonly string AssetResourcesPath = "assets/";

        /// <summary>
        /// Where our ecs assets are stored.
        /// Under:
        ///   resources/
        ///     assets/
        ///       ecs/
        /// </summary>
        [HideInEditor]
        public readonly string ContentECSPath = "ecs/";

        /// <summary>
        /// Where our generic assets are stored.
        /// Under:
        ///   resources/
        ///     assets/
        ///       data/
        /// </summary>
        [HideInEditor]
        public readonly string GenericAssetsPath = "data/";

        /// <summary>Game desired display width. Use <see cref="RenderContext.Camera"/> size for the runtime value.</summary>
        public readonly int GameWidth = 320;
        /// <summary>Game desired display height. Use <see cref="RenderContext.Camera"/> size for the runtime value.</summary>
        public readonly int GameHeight = 180;
        /// <summary>Game scaling factor.</summary>
        public readonly float GameScale = 2;

        /// <summary>
        /// Used for reporting bugs and feedback.
        /// </summary>
        public readonly string FeedbackUrl = string.Empty;
        public readonly string FeedbackKey = "changeme";

        public readonly ViewportResizeStyle ResizeStyle = new();

        [Bang.Serialize]
        internal bool _scalingFilter = false;

        /// <summary>
        /// Texture scaling smoothing
        /// </summary>
        [Tooltip("Texture scaling smoothing")]
        public bool ScalingFilter => _scalingFilter;

        public readonly int TargetFps = 60;
        public readonly float FixedUpdateFactor = 2;
        public readonly bool IsVSyncEnabled = false;
        public readonly bool ShowUiDebug = true;
        public readonly float PushAwayInterval = 0.05f;
        public readonly int DefaultGridCellSize = 24;

        [GameAssetId(typeof(WorldAsset))]
        public readonly Guid StartingScene;

        /// <summary>
        /// Whether textures (e.g. fonts) should be preloaded. If true, startup time might be slower but
        /// actual game will be faster.
        /// </summary>
        [Tooltip("Load textures on start, slower startup but no delays in game")]
        [Serialize]
        public bool PreloadTextures = true;

        public readonly Theme Theme = new Theme();

        public readonly EditorAssets EditorAssets = new();

        /// <summary>
        /// Background color for the game.
        /// </summary>
        public Color BackColor = Color.Black;

        [SimpleTexture, Serialize]
        internal string DefaultPalette = "images/murder_palette";

        /// <summary>
        /// ID of the default image used when an image is missing.
        /// </summary>
        [Tooltip("ID of the default image used when an image is missing.")]
        [GameAssetId(typeof(SpriteAsset))]
        public readonly Guid MissingImage = new("485a9a13-e62b-7215-dbc3-9e1df4bcba73");

        /// <summary>
        /// Dictionary mapping languages to the appropriate localization asset resource IDs.
        /// </summary>
        [Tooltip("Dictionary mapping languages to the appropriate localization asset resource IDs.")]
        [GameAssetId(typeof(LocalizationAsset))]
        public ImmutableDictionary<LanguageId, Guid> LocalizationResources = ImmutableDictionary<LanguageId, Guid>.Empty;

        [Tooltip("Aditional collision checks will be done if an entity moves more than X pixels per frame")]
        public int MinimumVelocityForSweep = 6;

        [GameAssetId<InputGraphicsAsset>]
        public Guid InputGraphics = Guid.Empty;

        [GameAssetId<InputProfileAsset>]
        public Guid InputProfile = Guid.Empty;

        public GameProfile() =>
            FilePath = GameDataManager.GameProfileFileName;
    }
}