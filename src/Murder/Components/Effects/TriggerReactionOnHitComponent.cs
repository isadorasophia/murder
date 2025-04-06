using Bang.Components;
using Murder.Assets;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

public readonly struct TriggerReactionOnHitComponent : IComponent
{
    [Tooltip("Play these on hit")]
    public readonly ImmutableArray<string> Animations = [];

    public readonly bool DestroySolid = true;
    public readonly bool OnlyOnce = true;

    [SpriteBatchReference]
    public readonly int? ChangeSpriteBatchOnComplete { get; init; } = null;

    public readonly int? ChangeYSort { get; init; } = null;

    [GameAssetId<PrefabAsset>]
    public readonly Guid? ReplaceWithPrefab = null;

    public TriggerReactionOnHitComponent() { }
}
