using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Data;

namespace Murder.Editor.Assets
{
    public class EditorSettingsAsset : GameAsset
    {
        public float DPI = 100;

        [Slider()]
        public float Downsample = 1;
        public override char Icon => '\uf085';
        public override bool CanBeRenamed => false;
        public override bool CanBeDeleted => false;
        public override bool CanBeCreated => false;
        public override string? CustomPath => GameDataManager.EditorSettingsFileName;

        public override bool StoreInDatabase => false;

        public string AssetNamePattern = " ({0})";
        public string NewAssetDefaultName = "New Asset";

        public bool StartOnEditor = true;

        public string AssetPathPrefix = "../../../../Murder/";

        /// <summary>
        /// Where most images and sounds sources are placed before being imported by the Content Pipeline
        /// </summary>
        public string ContentSourcesPath = "../../../../../ContentSources/";

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
    }
}
