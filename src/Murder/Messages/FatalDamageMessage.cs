using Bang.Components;
using Murder.Core.Geometry;

namespace Road.Messages
{
    /// <summary>
    /// A message signaling that this entity should be killed
    /// </summary>
    public readonly struct FatalDamageMessage : IMessage
    {
        public readonly Vector2 FromPosition = Vector2.Zero;
        public readonly int Amount;

        public FatalDamageMessage(Vector2 fromPosition, int damageAmount)
        {
            FromPosition = fromPosition;
            Amount = damageAmount;
        }
    }
}
