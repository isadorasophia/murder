using Murder.Data;
using Murder.Attributes;
using Microsoft.Xna.Framework;

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
        /// Where our aseprite contents are stored.
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
        ///     shaders/
        /// </summary>
        [HideInEditor]
        public readonly string ShadersPath = "shaders/";

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
        public readonly int GameScale = 3;

        [HideInEditor]
        public bool Fullscreen = false;

        public readonly int TargetFps = 60;
        public readonly float FixedUpdateFactor = 2;
        public readonly bool IsVSyncEnabled = false;
        public readonly bool ShowUiDebug = true;
        public readonly float PushAwayInterval = 0.05f;

        [GameAssetId(typeof(WorldAsset))]
        public readonly Guid StartingScene;

        public readonly Theme Theme = new Theme();

        public readonly Exploration Exploration = new();
        public readonly EditorAssets EditorAssets = new();

        public Color BackColor = Color.Black;

        public GameProfile() =>
            FilePath = GameDataManager.GameProfileFileName;
    }
}
