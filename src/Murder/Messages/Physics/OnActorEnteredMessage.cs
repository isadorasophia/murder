using Bang.Components;
using Murder.Utilities;

namespace Murder.Messages.Physics
{
    public readonly struct OnActorEnteredOrExitedMessage : IMessage
    {
        public readonly int EntityId;
        public readonly CollisionDirection Movement;
        /// <summary>
        /// Message sent to the TRIGGER when touching an actor touches it.
        /// </summary>
        public OnActorEnteredOrExitedMessage(int actorId, CollisionDirection movement)
        {
            EntityId = actorId;
            Movement = movement;
        }
    }
}
