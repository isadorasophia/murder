using Bang.Components;

namespace Murder.Messages
{
    /// <summary>
    /// A message signaling that this entity should be killed
    /// </summary>
    public readonly struct FatalDamageMessage : IMessage
    {
        /// <summary>
        /// This is the entity id that caused the damage.
        /// When environmental or caused by itself, either same value as <see cref="DamagedEntityId"/> or -1.
        /// </summary>
        public readonly int FromId = -1;

        /// <summary>
        /// This is the entity id that got attacked.
        /// If -1, assume it's a critical fatal (everything got the fatal damage).
        /// </summary>
        public readonly int DamagedEntityId = -1;

        public FatalDamageMessage() { }

        public FatalDamageMessage(int fromId, int damagedEntityId)
        {
            FromId = fromId;
            DamagedEntityId = damagedEntityId;
        }
    }
}