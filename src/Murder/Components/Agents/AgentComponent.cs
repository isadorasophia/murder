using Bang.Components;
using Murder.Attributes;
using System.Diagnostics;

namespace Murder.Components
{
    [DebuggerDisplay("[AgentComponent] Speed: {Speed}, Acceleration: {Acceleration}")]
    public readonly struct AgentComponent : IComponent
    {
        public readonly float Speed;
        public readonly float Acceleration;

        [Slider]
        public readonly float Friction;


        public AgentComponent(float speed, float acceleration, float friction)
        {
            Speed = speed;
            Acceleration = acceleration;
            Friction = friction;
        }
    }
}
