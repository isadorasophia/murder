using Bang.Interactions;
using Murder.Attributes;
using System.Collections.Immutable;

namespace Murder.Core.MurderActions
{
    public readonly struct MurderTargetedAction
    {
        [InstanceId]
        public readonly Guid Target = Guid.Empty;
        
        public readonly ImmutableArray<IInteractiveComponent> Interaction = ImmutableArray<IInteractiveComponent>.Empty;

        public MurderTargetedAction()
        {
        }

        internal MurderTargetedRuntimeAction Bake(int actionId)
        {
            return new MurderTargetedRuntimeAction(actionId, Interaction);
        }
    }

}
