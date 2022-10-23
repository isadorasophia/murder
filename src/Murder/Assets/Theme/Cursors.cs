using Murder.Assets.Graphics;
using Murder.Attributes;

namespace Murder.Assets
{
    [Serializable]
    public class Cursors
    {
        [GameAssetId(typeof(AsepriteAsset))]
        public Guid Normal;

        [GameAssetId(typeof(AsepriteAsset))]
        public Guid Point;

        [GameAssetId(typeof(AsepriteAsset))]
        public Guid Hand;

        [GameAssetId(typeof(AsepriteAsset))]
        public Guid Eye;
    }
}
