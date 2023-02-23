using Bang.Components;

namespace Murder.Messages.Physics
{
    public readonly struct CollidedWithTriggerMessage : IMessage
    {
        public readonly int EntityId;

        public CollidedWithTriggerMessage(int entityId)
        {
            EntityId = entityId;
        }
    }
}
