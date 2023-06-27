using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components.Effects;
using Murder.Messages.Physics;
using Murder.Utilities.Attributes;

namespace Murder.Systems
{
    [Filter(typeof(OnEnterOnExitComponent))]
    [Messager(typeof(OnActorEnteredOrExitedMessage))]
    internal class OnEnterOnExitInteractionSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            if (message is not OnActorEnteredOrExitedMessage actorMessage)
            {
                return;
            }

            OnEnterOnExitComponent onEnterOnExit = entity.GetOnEnterOnExit();
            if (actorMessage.Movement == Murder.Utilities.CollisionDirection.Enter)
            {
                onEnterOnExit.OnEnter?.Interact(world, entity, interacted: null);
            }
            else
            {
                onEnterOnExit.OnExit?.Interact(world, entity, interacted: null);
            }
        }
    }
}
