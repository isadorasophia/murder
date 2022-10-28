using Murder.Data;
using Murder.Attributes;

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

        /// <summary>
        /// Where our atlas .png and .json files are stored.
        /// Under: 
        ///   packed/ -> bin/resources/
        ///     atlas/
        /// </summary>
        [HideInEditor]
        public string AtlasFolderName = "atlas/";

        /// <summary>
        /// Where our aseprite contents are stored.
        /// Under:
        ///   packed/ -> bin/resources/
        ///     aseprite/
        /// </summary>
        [HideInEditor]
        public string ContentAsepritePath = "aseprite/";

        /// <summary>
        /// Where our font contents are stored.
        /// Under:
        ///   packed/ -> bin/resources/
        ///     fonts/
        /// </summary>
        [HideInEditor]
        public string FontPath = "shaders/";

        /// <summary>
        /// Where our aseprite contents are stored.
        /// Under:
        ///   packed/ -> bin/resources/
        ///     sounds/
        /// </summary>
        [HideInEditor]
        public string SoundsPath = "sounds/";

        /// <summary>
        /// Where our aseprite contents are stored.
        /// Under:
        ///   packed/ -> bin/resources/
        ///     shaders/
        /// </summary>
        [HideInEditor]
        public string ShadersPath = "shaders/";

        /// <summary>
        /// Where our high resolution contents are stored.
        /// Under:
        ///   packed/ -> bin/resources
        ///     shaders/
        /// </summary>
        [HideInEditor]
        public string HiResPath = "hires_images/";

        /// <summary>
        /// Root path where our data .json files are stored.
        /// </summary>
        public string AssetResourcesPath = "assets/";

        /// <summary>
        /// Where our ecs assets are stored.
        /// Under:
        ///   resources/
        ///     assets/
        ///       ecs/
        /// </summary>
        [HideInEditor]
        public string ContentECSPath = "ecs/";

        /// <summary>
        /// Where our generic assets are stored.
        /// Under:
        ///   resources/
        ///     assets/
        ///       data/
        /// </summary>
        [HideInEditor]
        public string GenericAssetsPath = "data/";

        public int GameWidth = 320;
        public int GameHeight = 180;
        public int GameScale = 3;

        [HideInEditor]
        public bool Fullscreen = false;

        public int TargetFps = 60;
        public float FixedUpdateFactor = 2;
        public bool IsVSyncEnabled = false;
        public bool ShowUiDebug = true;
        public float PushAwayInterval = 0.05f;

        [GameAssetId(typeof(WorldAsset))]
        public Guid StartingScene;

        public Theme Theme = new Theme();

        public Exploration Exploration = new();
        public Cursors Cursors = new();

        public GameProfile() =>
            FilePath = GameDataManager.GameProfileFileName;
    }
}
