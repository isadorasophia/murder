using Bang.Interactions;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Core.MurderActions
{
    [RuntimeOnly]
    public readonly struct MurderTargetedRuntimeAction
    {
        public readonly int EntityId = -1;

        public readonly ImmutableArray<IInteractiveComponent> Interaction = ImmutableArray<IInteractiveComponent>.Empty;

        public MurderTargetedRuntimeAction(int id, ImmutableArray<IInteractiveComponent> interaction)
        {
            EntityId = id;
            Interaction = interaction;
        }
    }
}
