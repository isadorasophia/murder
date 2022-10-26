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
        public override string? CustomPath => GameDataManager.GameProfileFileName;

        public override bool StoreInDatabase => false;

        /// <summary>
        /// Where our data .json files are stored
        /// </summary>
        public string GameAssetsResourcesPath = "Content/";

        /// <summary>
        /// Where our atlas .png and .json files are stored
        /// </summary>
        public string AtlasFolderName = "atlas/";

        /// <summary>
        /// Where our ecs assets are stored
        /// </summary>
        public string ContentAsepritePath = "aseprite/";

        /// <summary>
        /// Where our ecs assets are stored
        /// </summary>
        public string ContentECSPath = "ecs/";

        /// <summary>
        /// Where our generic assets are stored
        /// </summary>
        public string ContentDataPath = "data/";

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
    }
}
