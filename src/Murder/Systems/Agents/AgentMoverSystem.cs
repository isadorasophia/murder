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

                e.SetFacing(new FacingComponent(DirectionHelper.FromVector(impulse.Impulse)));

                var result = startVelocity;

                // Use friction on any axis that's not receiving impulse or is receiving it in an oposing direction
                if (impulse.Impulse.HasValue && e.TryGetFriction() is FrictionComponent friction)
                {
                    if (impulse.Impulse.X == 0 || Calculator.SameSign(impulse.Impulse.X, result.X)) { result.X *= friction.Amount; }
                    if (impulse.Impulse.Y == 0 || Calculator.SameSign(impulse.Impulse.Y, result.Y)) { result.Y *= friction.Amount; }
                    e.RemoveFriction();         // Remove friction to move
                }

                result = Calculator.Approach(startVelocity, impulse.Impulse * agent.Speed, agent.Acceleration * Game.FixedDeltaTime);

                e.SetVelocity(result); // Turn impulse into velocity
            }

            return default;
        }
    }
}
