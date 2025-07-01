using Bang.Components;

namespace Murder.Components.Agents
{
    public readonly struct OverrideAgentSpeedComponent : IComponent
    {
        public readonly float MaxSpeed = 0;
        public readonly float Acceleration = -1;

        public OverrideAgentSpeedComponent() { }

        public OverrideAgentSpeedComponent(float maxSpeed)
        {
            MaxSpeed = maxSpeed;
        }

        public OverrideAgentSpeedComponent(float maxSpeed, float acceleration) : this(maxSpeed)
        {
            Acceleration = acceleration;
        }
    }
}