using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    public readonly struct FrictionComponent : IComponent
    {
        [Tooltip("The ammount of friction to apply, 0 never stops, o.9 stops very fast")]
        public readonly float Amount;

        [Tooltip("The ammount of friction to apply while off ground, 0 never stops, o.9 stops very fast. No value will use the regular friction amount.")]
        public readonly float? AirFriction;
        /// <summary>
        /// High friction means stopping fast
        /// </summary>
        public FrictionComponent(float amount) : this()
        {
            Amount = amount;
        }
    }
}