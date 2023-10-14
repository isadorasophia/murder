using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Agents
{
    public readonly struct AgentSpeedOverride : IComponent
    {
        public readonly float MaxSpeed = 0;
        public readonly float Acceleration = 0;

        public AgentSpeedOverride() { }

        public AgentSpeedOverride(float maxSpeed, float acceleration)
        {
            MaxSpeed = maxSpeed;
            Acceleration = acceleration;
        }
    }
}