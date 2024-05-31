using Bang;
using Bang.Entities;
using Bang.Interactions;
namespace Murder.Interactions
{
    public readonly struct SendInteractMessageInteraction : IInteraction
    {
        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            interacted?.SendInteractMessage(interactor);
        }
    }
}
