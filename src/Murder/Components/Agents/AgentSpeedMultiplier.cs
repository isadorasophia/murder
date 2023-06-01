using Bang.Components;

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

        /// <summary>
        /// Increases the current multiplier by <paramref name="multiplier"/>.
        /// </summary>
        public AgentSpeedMultiplier WithMultiplier(float multiplier) => new(SpeedMultiplier + multiplier);
    }
}
