using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Utilities;
using Bang.Components;

namespace Murder.Systems.Agents
{
    /// <summary>
    /// Simple system for moving agents to another position. Looks for 'MoveTo' components and adds agent inpulses to it.
    /// </summary>
    [Filter(typeof(ITransformComponent), typeof(MoveToComponent))]
    public class AgentMoveToSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                MoveToComponent move = e.GetMoveTo();
                IMurderTransformComponent position = e.GetGlobalTransform();

                Vector2 targetPosition = move.Target;
                Vector2 delta = targetPosition - position.ToVector2();

                var distanceSq = delta.LengthSquared();
                if (distanceSq < MathF.Pow(move.MinDistance, 2))
                {
                    // No impulse, I'm too close
                    e.RemoveMoveTo();
                }
                else if (distanceSq < MathF.Pow(move.SlowDownDistance, 2))
                {
                    // Start slowing down
                    var distance = MathF.Sqrt(distanceSq);
                    var ratio = Calculator.Remap(distance, move.MinDistance, move.SlowDownDistance, 0.5f, 1);
                    e.SetAgentImpulse(delta.Normalized() * ratio);
                }
                else
                {
                    // Regular move
                    var direction = delta.Normalized();
                    e.SetAgentImpulse(direction);
                }
            }
        }
    }
}
