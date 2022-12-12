using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Agents
{
    public readonly struct AgentSpeedMultiplier : IComponent
    {
        public readonly float SpeedMultiplier = 0;

        public AgentSpeedMultiplier()
        {
        }

        public AgentSpeedMultiplier(float speedMultiplier)
        {
            SpeedMultiplier = speedMultiplier;
        }
    }
}
