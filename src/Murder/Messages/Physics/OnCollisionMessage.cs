using Bang.Components;
using Murder.Utilities;

namespace Murder.Messages.Physics
{
    /// <summary>
    /// Message sent to the ACTOR when touching a trigger area.
    /// </summary>
    public readonly struct OnCollisionMessage : IMessage
    {
        /// <summary>
        /// The other colliding entity, please be aware that the entity may no longer exist or be active.
        /// </summary>
        public readonly int EntityId;
        public readonly CollisionDirection Movement;

        /// <summary>
        /// Message sent to the ACTOR when touching a trigger area.
        /// </summary>
        public OnCollisionMessage(int triggerId, CollisionDirection movement)
        {
            EntityId = triggerId;
            Movement = movement;
        }
    }
}