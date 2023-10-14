using Murder.Assets.Graphics;
using Murder.Attributes;

namespace Murder.Core
{
    public readonly struct Portrait
    {
        public bool HasValue => Sprite != Guid.Empty;

        [GameAssetId(typeof(SpriteAsset))]
        public readonly Guid Sprite = Guid.Empty;

        public readonly string AnimationId = string.Empty;

        public Portrait() { }

        public Portrait(Guid aseprite, string animationId) =>
            (Sprite, AnimationId) = (aseprite, animationId);

        public bool HasImage
        {
            get
            {
                if (Game.Data.TryGetAsset<SpriteAsset>(Sprite) is SpriteAsset sprite)
                {
                    if (sprite.Animations.ContainsKey(AnimationId))
                        return true;
                }
                return false;
            }
        }

        public Portrait WithAnimationId(string animationId) => new(Sprite, animationId);
    }
}