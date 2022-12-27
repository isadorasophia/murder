using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Components;
using Murder.Utilities;

namespace Murder.Interactions
{
    public readonly struct RemoveEntityOnInteraction : Interaction
    {
        public RemoveEntityOnInteraction() { }

        public void Interact(World world, Entity interactor, Entity interacted)
        {
            if (interacted.TryGetIdTarget()?.Target is int targetId &&
                world.TryGetEntity(targetId) is Entity targetEntity)
            {
                targetEntity.Destroy();
            }
        }
    }
}
