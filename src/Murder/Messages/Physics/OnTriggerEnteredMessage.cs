using Bang.Components;
using Murder.Utilities;

namespace Murder.Messages.Physics
{
    /// <summary>
    /// Message sent to the ACTOR when touching a trigger area.
    /// </summary>
    public readonly struct OnTriggerEnteredMessage : IMessage
    {
        public readonly int EntityId;
        public readonly CollisionDirection Movement;

        /// <summary>
        /// Message sent to the ACTOR when touching a trigger area.
        /// </summary>
        public OnTriggerEnteredMessage(int triggerId, CollisionDirection movement)
        {
            EntityId = triggerId;
            Movement = movement;
        }
    }
}
