using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

public readonly struct PlayAnimationOnHitComponent : IComponent
{
    [Tooltip("Play these on hit")]
    public readonly ImmutableArray<string> Animations = [];

    public readonly bool DestroySolid = true;

    [SpriteBatchReference]
    public readonly int? ChangeSpriteBatchOnComplete { get; init; } = Batches2D.GameplayBatchId;

    public PlayAnimationOnHitComponent() { }
}
