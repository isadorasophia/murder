using Bang.Components;

namespace Murder.Components.Physics
{
    public readonly struct GravityMultiplierComponent : IComponent
    {
        public readonly float Multiply = 1f;

        public GravityMultiplierComponent()
        {
        }

        public GravityMultiplierComponent(float multiply)
        {
            Multiply = multiply;
        }
    }
}
