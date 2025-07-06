using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Geometry;

namespace Murder.Core;

public readonly struct Portrait : IEquatable<Portrait>
{
    public bool HasValue => Sprite != Guid.Empty;

    [GameAssetId(typeof(SpriteAsset))]
    public readonly Guid Sprite { get; init; } = Guid.Empty;

    public readonly string AnimationId { get; init; } = string.Empty;

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

    public bool Equals(Portrait other)
    {
        return Sprite == other.Sprite && AnimationId == other.AnimationId;
    }

    public Point GetSize()
    {
        if (Game.Data.TryGetAsset<SpriteAsset>(Sprite) is SpriteAsset sprite)
        {
            return sprite.Size;
        }

        return Point.Zero;
    }
}