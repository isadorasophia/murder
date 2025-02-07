using Bang.Components;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Messages
{
    /// <summary>
    /// A message signaling that this entity should be killed
    /// </summary>
    public readonly struct FatalDamageMessage : IMessage
    {
        public readonly Vector2 FromPosition = Vector2.Zero;
        public readonly int Amount;
        public readonly int Source = -1;

        public FatalDamageMessage(Vector2 fromPosition, int damageAmount, int source)
        {
            FromPosition = fromPosition;
            Amount = damageAmount;
            Source = source;
        }
    }
}