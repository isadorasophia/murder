using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Messages;

namespace Murder.Systems
{
    [Filter(typeof(PersistInteractionComponent))]
    [Messager(typeof(InteractMessage))]
    internal class PersistInteractSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            entity.SetInteractionMessageReceived();
        }
    }
}
