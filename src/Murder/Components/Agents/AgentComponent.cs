using Bang.Components;
using Murder.Attributes;
using Murder.Core.Physics;
using Murder.Utilities.Attributes;
using System.Diagnostics;

namespace Murder.Components
{
    [DebuggerDisplay("[AgentComponent] Speed: {Speed}, Acceleration: {Acceleration}")]
    public readonly struct AgentComponent : IComponent
    {
        /// <summary>
        /// Maximum speed of this agent
        /// </summary>
        [Tooltip("Maximum speed of this agent")]
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
