using Bang.Components;

namespace Murder.Messages.Physics
{
    public readonly struct OnTriggerEnteredMessage : IMessage
    {
        public readonly int EntityId;

        public OnTriggerEnteredMessage(int entityId)
        {
            EntityId = entityId;
        }
    }
}
