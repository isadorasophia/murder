using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Bang.Systems;
using Murder.Messages;

namespace Murder.Systems
{
    [Filter(typeof(IInteractiveComponent))]
    [Messager(typeof(InteractMessage))]
    internal class InteractSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            var interactor = (InteractMessage)message;

            if (entity.TryGetInteractive() is IInteractiveComponent interacted)
            {
                interacted.Interact(world, interactor.Interactor ?? entity, entity);
            }
        }
    }
}