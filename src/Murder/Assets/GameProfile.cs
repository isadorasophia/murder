using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Data;
using Newtonsoft.Json;

namespace Murder.Assets
{
    public class GameProfile : GameAsset
    {
        public override char Icon => '\uf085';
        public override bool CanBeRenamed => false;
        public override bool CanBeDeleted => false;
        public override bool CanBeCreated => false;

        public override string SaveLocation => string.Empty;

        public override bool StoreInDatabase => false;

        public float Aspect
        {
            get
            {
                if (true || _aspect == 0)
                {
                    _aspect = (float)GameHeight / GameWidth;
                }

                return _aspect;
            }

        }

        private float _aspect = 0;

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
        /// Where our aseprite contents are stored.
        /// Under:
        ///   packed/ -> bin/resources/
        ///     dialogues/
        /// </summary>
        [HideInEditor]
        public readonly string DialoguesPath = "dialogues/";

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

        public readonly int GameWidth = 320;
        public readonly int GameHeight = 180;
        public readonly int GameScale = 2;

        [JsonProperty]
        internal bool _enforceResolution = false;

        public bool EnforceResolution => _enforceResolution;

        [JsonProperty]
        internal bool _scalingFilter = false;

        public bool ScalingFilter => _scalingFilter;

        [HideInEditor]
        public bool Fullscreen = false;

        public readonly int TargetFps = 60;
        public readonly float FixedUpdateFactor = 2;
        public readonly bool IsVSyncEnabled = false;
        public readonly bool ShowUiDebug = true;
        public readonly float PushAwayInterval = 0.05f;
        public readonly int DefaultGridCellSize = 24;

        [GameAssetId(typeof(WorldAsset))]
        public readonly Guid StartingScene;

        public readonly Theme Theme = new Theme();

        public readonly Exploration Exploration = new();
        public readonly EditorAssets EditorAssets = new();

        public Color BackColor = Color.Black;

        [SimpleTexture, JsonProperty]
        internal string DefaultPalette = "images/murder_palette";

        [GameAssetId(typeof(SpriteAsset))]
        public readonly Guid MissingImage = new("485a9a13-e62b-7215-dbc3-9e1df4bcba73");

        public GameProfile() =>
            FilePath = GameDataManager.GameProfileFileName;
    }
}