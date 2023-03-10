using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Diagnostics;
using System.Collections.Immutable;

namespace Murder.Interactions
{
    public readonly struct RemoveEntityOnInteraction : Interaction
    {
        public readonly bool DestroySelf = false;
        public RemoveEntityOnInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            GameLogger.Verify(interacted is not null);

            if (DestroySelf)
            {
                interacted?.Destroy();
                return;
            }

            if (interacted.TryGetIdTarget()?.Target is int targetId &&
                world.TryGetEntity(targetId) is Entity targetEntity)
            {
                targetEntity.Destroy();
            }

            // Also delete all entities defined in a collection.
            if (interacted.TryGetIdTargetCollection()?.Targets is ImmutableDictionary<string, int> targets)
            {
                foreach (int entityId in targets.Values)
                {
                    if (world.TryGetEntity(entityId) is Entity otherTarget)
                    {
                        otherTarget.Destroy();
                    }
                }
            }
        }
    }
}
