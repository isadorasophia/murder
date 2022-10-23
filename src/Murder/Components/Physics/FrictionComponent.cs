using Bang.Components;

namespace Murder.Components
{
    internal readonly struct FrictionComponent : IComponent
    {
        public readonly float Amount;

        public FrictionComponent(float amount) : this()
        {
            Amount = amount;
        }
    }
}
