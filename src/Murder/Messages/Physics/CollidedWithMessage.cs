using Bang.Components;
using Murder.Core.Geometry;

namespace Murder.Messages
{
    
    public readonly struct CollidedWithMessage : IMessage
    {
        public readonly Vector2 Pushout;
        public readonly int EntityId;

        /// <summary>
        /// Signals a collision with another entity
        /// </summary>
        public CollidedWithMessage(int entityId, Core.Geometry.Vector2 pushout)
        {
            EntityId = entityId;
            Pushout = pushout;
        }
    }
}
