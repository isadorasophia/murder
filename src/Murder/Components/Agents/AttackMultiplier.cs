using Bang.Components;

namespace Murder.Components
{
    public readonly struct AttackMultiplier : IComponent
    {
        public readonly float Multiplier = 0;

        public AttackMultiplier()
        {
        }

        public AttackMultiplier(float multiplier)
        {
            Multiplier = multiplier;
        }
    }
}
