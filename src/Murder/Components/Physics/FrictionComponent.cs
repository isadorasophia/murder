using Bang.Components;

namespace Murder.Components
{
    public readonly struct FrictionComponent : IComponent
    {
        public readonly float Amount;
        /// <summary>
        /// High friction means stopping fast
        /// </summary>
        public FrictionComponent(float amount) : this()
        {
            Amount = amount;
        }
    }
}