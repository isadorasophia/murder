using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

[CustomName($" AnimationEventBroadcaster")]
[RuntimeOnly]
[PersistOnSave]
public readonly struct AnimationEventBroadcasterComponent : IComponent
{
    public readonly ImmutableHashSet<int> BroadcastTo = ImmutableHashSet<int>.Empty;

    public AnimationEventBroadcasterComponent()
    {
    }

    public AnimationEventBroadcasterComponent(ImmutableHashSet<int> broadcastTo)
    {
        BroadcastTo = broadcastTo;
    }

    internal AnimationEventBroadcasterComponent Subscribe(int entityId)
    {
        return new AnimationEventBroadcasterComponent(BroadcastTo.Add(entityId));
    }
}