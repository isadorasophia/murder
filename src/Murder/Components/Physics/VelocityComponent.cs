using Bang.Components;
using System.Numerics;

namespace Murder.Components
{
    public readonly struct VelocityComponent : IComponent
    {
        public readonly Vector2 Velocity;

        public VelocityComponent(Vector2 velocity) =>
            Velocity = velocity;

        public VelocityComponent(float x, float y) =>
            Velocity = new Vector2(x, y);
    }
}
