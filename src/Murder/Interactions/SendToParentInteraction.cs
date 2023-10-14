using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Messages;

namespace Murder.Interactions
{
    public readonly struct SendToParentInteraction : IInteraction
    {
        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (interacted == null)
            {
                return;
            }

            interacted.TryFetchParent()?.SendMessage(new InteractMessage(interacted));
        }
    }
}