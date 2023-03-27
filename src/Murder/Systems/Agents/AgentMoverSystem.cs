using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder;
using Murder.Components;
using Murder.Components.Agents;
using Murder.Core.Geometry;
using Murder.Helpers;
using Murder.Utilities;

namespace Road.Systems
{
    /// <summary>
    /// System that looks for AgentImpulse systems and translated them into 'Velocity' for the physics system.
    /// </summary>
    [Filter(typeof(AgentComponent), typeof(AgentImpulseComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(DisableAgentComponent))]
    internal class AgentMoverSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                var agent = e.GetAgent();
                var impulse = e.GetAgentImpulse();

                Vector2 startVelocity = e.TryGetVelocity()?.Velocity ?? Vector2.Zero;
                
                if (!e.HasStrafing())
                    e.SetFacing(impulse.Direction);

                // Use friction on any axis that's not receiving impulse or is receiving it in an oposing direction
                var result = GetVelocity(e, agent, impulse, startVelocity);

                e.RemoveFriction();     // Remove friction to move
                e.SetVelocity(result); // Turn impulse into velocity
            }
        }

        private static Vector2 GetVelocity(Entity entity, AgentComponent agent, AgentImpulseComponent impulse, in Vector2 currentVelocity)
        {
            var velocity = currentVelocity;

            if (impulse.Impulse.HasValue)
            {
                if (impulse.Impulse.X == 0 || !Calculator.SameSignOrSimilar(impulse.Impulse.X, currentVelocity.X))
                {
                    velocity = new Vector2(currentVelocity.X * agent.Friction, velocity.Y);
                }
                if (impulse.Impulse.Y == 0 || !Calculator.SameSignOrSimilar(impulse.Impulse.Y, currentVelocity.Y))
                {
                    velocity = new Vector2(velocity.X, velocity.Y * agent.Friction);
                }
            }

            float multiplier = 1f;
            if (entity.TryGetAgentSpeedMultiplier() is AgentSpeedMultiplier speedMultiplier)
                multiplier = speedMultiplier.SpeedMultiplier;

            float speed, accel;
            if (entity.TryGetAgentSpeedOverride() is AgentSpeedOverride speedOverride)
            {
                speed = speedOverride.MaxSpeed;
                accel = speedOverride.Acceleration;
            }
            else
            {
                speed = agent.Speed;
                accel = agent.Acceleration;
            }


            return Calculator.Approach(velocity, impulse.Impulse * speed * multiplier, accel * multiplier * Game.FixedDeltaTime);
        }
    }
}
