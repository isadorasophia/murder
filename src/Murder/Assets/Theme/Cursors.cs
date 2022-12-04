using Murder.Assets.Graphics;
using Murder.Attributes;

namespace Murder.Assets
{
    [Serializable]
    public class EditorAssets
    {
        [GameAssetId(typeof(AsepriteAsset))]
        public Guid Normal;

        [GameAssetId(typeof(AsepriteAsset))]
        public Guid Point;

        [GameAssetId(typeof(AsepriteAsset))]
        public Guid Hand;

        [GameAssetId(typeof(AsepriteAsset))]
        public Guid Eye;
        
        // Cutscenes
        [GameAssetId(typeof(AsepriteAsset))]
        public readonly Guid CutsceneImage = Guid.Empty;

        [GameAssetId(typeof(AsepriteAsset))]
        public readonly Guid AnchorImage = Guid.Empty;
    }
}
