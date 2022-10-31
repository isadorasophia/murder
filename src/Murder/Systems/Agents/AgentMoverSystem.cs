using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Helpers;
using Murder.Utilities;

namespace Road.Systems
{
    [Filter(typeof(AgentComponent), typeof(AgentImpulseComponent))]
    internal class AgentMoverSystem : IFixedUpdateSystem
    {
        public ValueTask FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                var agent = e.GetAgent();
                var impulse = e.GetAgentImpulse();

                Vector2 startVelocity = Vector2.Zero;
                if (e.TryGetVelocity() is VelocityComponent velocity)
                {
                    startVelocity = velocity.Velocity;
                }

                if (e.TryGetFacing() is FacingComponent facing)
                    e.SetFacing(new FacingComponent(facing.Direction));
                else
                    e.SetFacing(new FacingComponent(impulse.Impulse));

                var result = Calculator.Approach(startVelocity, impulse.Impulse * agent.Speed, agent.Acceleration * Game.FixedDeltaTime);

                if (impulse.Impulse.HasValue)
                {
                    e.RemoveFriction();         // Remove friction to move
                }
                e.SetVelocity(result); // Turn impulse into velocity
            }

            return default;
        }
    }
}
