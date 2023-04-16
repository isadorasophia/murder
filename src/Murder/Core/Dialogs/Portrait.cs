using Murder.Assets.Graphics;
using Murder.Attributes;

namespace Murder.Core
{
    public readonly struct Portrait
    {
        [GameAssetId(typeof(SpriteAsset))]
        public readonly Guid Aseprite;

        public readonly string AnimationId;

        public Portrait(Guid aseprite, string animationId) =>
            (Aseprite, AnimationId) = (aseprite, animationId);

        public Portrait WithAnimationId(string animationId) => new(Aseprite, animationId);
    }
}
