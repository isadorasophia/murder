using Bang.Components;
using Murder.Utilities;

namespace Murder.Messages.Physics
{
    public readonly struct OnActorEnteredMessage : IMessage
    {
        public readonly int EntityId;
        public readonly CollisionDirection Movement;
        /// <summary>
        /// Message sent to the TRIGGER when touching an actor touches it.
        /// </summary>
        public OnActorEnteredMessage(int actorId, CollisionDirection movement)
        {
            EntityId = actorId;
            Movement = movement;
        }
    }
}
