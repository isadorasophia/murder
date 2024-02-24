using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Messages;

namespace Murder.Interactions
{
    public readonly struct SendToParentInteraction : IInteraction
    {
        [Default("Send a specific message")]
        public readonly IMessage? CustomMessage;

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (interacted == null)
            {
                return;
            }

            interacted.TryFetchParent()?.SendMessage(CustomMessage ?? new InteractMessage(interacted));
        }
    }
}