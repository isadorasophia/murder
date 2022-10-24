using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Geometry;

namespace Murder.Systems
{
    [Filter(typeof(AgentComponent), typeof(AgentImpulseComponent))]
    internal class AgentMoverSystem : IFixedUpdateSystem
    {
        public ValueTask FixedUpdate(Context context)
        {
            // TODO: Generate extesions
            //foreach (var e in context.Entities)
            //{
            //    var agent = e.GetAgent();
            //    var impulse = e.GetAgentImpulse();

            //    Vector2 startVelocity = Vector2.Zero;
            //    if (e.TryGetVelocity() is VelocityComponent velocity)
            //    {
            //        startVelocity = velocity.Velocity;
            //    }

            //    if (e.TryGetFacing() is FacingComponent facing)
            //        e.SetFacing(DirectionHelper.CreateFacing(impulse.Impulse, facing));
            //    else
            //        e.SetFacing(DirectionHelper.CreateFacing(impulse.Impulse, false, false));

            //    var result = Calculator.Approach(startVelocity, impulse.Impulse * agent.Speed, agent.Acceleration * Game.FixedDeltaTime);

            //    if (impulse.Impulse.HasValue)
            //    {
            //        e.RemoveFriction();         // Remove friction to move
            //    }
            //    e.SetVelocity(result); // Turn impulse into velocity
            //}

            return default;
        }
    }
}
