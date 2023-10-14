using Bang.Components;
using System.Numerics;

namespace Murder.Components.Graphics
{
    public readonly struct ReflectionComponent : IComponent
    {
        public readonly float Alpha = 0.3f;
        public readonly Vector2 Offset = Vector2.Zero;
        public readonly bool BlockReflection = false;

        public ReflectionComponent()
        {
        }
    }
}