using Bang.Components;

namespace Murder.Components
{
    public readonly struct FrictionComponent : IComponent
    {
        public readonly float Amount;

        public FrictionComponent(float amount) : this()
        {
            Amount = amount;
        }
    }
}
