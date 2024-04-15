using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using System.Collections.Immutable;

namespace Murder.Interactions
{
    public readonly struct AdvancedBlackboardInteraction : IInteraction
    {
        [Bang.Serialize, ShowInEditor]
        private readonly ImmutableArray<BlackboardAction> _actions = ImmutableArray<BlackboardAction>.Empty;

        public AdvancedBlackboardInteraction()
        {
        }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            foreach (var action in _actions)
            {
                action.Invoke(world, interactor, interacted);
            }
        }
    }
}