
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Core;
/// <summary>
/// Struct for creating one-shot animations and other similar sprite effects.
/// </summary>
public readonly struct QuickSpriteInfo
{
    [GameAssetId<SpriteAsset>]
    public readonly Guid Sprite { get; init; } = new();
    public readonly ImmutableArray<string> Animations { get; init; } = [];
    public readonly Vector2 Offset { get; init; } = Vector2.Zero;
    public readonly int YSortOffset { get; init; } = 0;
    public readonly ImageFlip Flip { get; init; } = ImageFlip.None;

    [SpriteBatchReference]
    public readonly int TargetSpriteBatch { get; init; } = Batches2D.GameplayBatchId;

    public QuickSpriteInfo(Portrait portrait)
    {
        Sprite = portrait.Sprite;
        Animations = [portrait.AnimationId];
    }

    public QuickSpriteInfo(Guid sprite)
    {
        Sprite = sprite;
    }
}
