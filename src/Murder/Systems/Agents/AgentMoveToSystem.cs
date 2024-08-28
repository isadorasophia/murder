using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems.Agents
{
    /// <summary>
    /// Simple system for moving agents to another position. Looks for 'MoveTo' components and adds agent inpulses to it.
    /// </summary>
    [Filter(typeof(ITransformComponent))]
    [Filter(ContextAccessorFilter.AnyOf, typeof(MoveToComponent), typeof(MoveToTargetComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(AgentPauseComponent))]
    public class AgentMoveToSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                if (e.TryGetMoveToMaxTime() is MoveToMaxTimeComponent moveToMaxTime &&
                    Game.Now > moveToMaxTime.RemoveAt)
                {
                    e.RemoveMoveTo();
                    e.RemoveMoveToTarget();
                }

                IMurderTransformComponent position = e.GetGlobalTransform();
                
                if (e.TryGetMoveTo() is MoveToComponent move)
                {
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

                        AgentImpulseFlags flags = distanceSq < (MathF.Pow(move.SlowDownDistance, 2) / 4f) ?
                            AgentImpulseFlags.IgnoreFacing : AgentImpulseFlags.None;

                        e.SetAgentImpulse(delta.Normalized() * ratio, flags);
                    }
                    else
                    {
                        // Regular move
                        var direction = delta.Normalized();
                        e.SetAgentImpulse(direction);
                    }
                }
                else if (e.TryGetMoveToTarget() is MoveToTargetComponent moveToTarget)
                {
                    if (context.World.TryGetEntity(moveToTarget.Target) is Entity target)
                    {
                        Vector2 targetPosition = target.GetGlobalTransform().ToVector2() + moveToTarget.Offset;
                        Vector2 delta = targetPosition - position.ToVector2();

                        var distanceSq = delta.LengthSquared();
                        if (distanceSq < MathF.Pow(moveToTarget.MinDistance, 2))
                        {
                            // No impulse, I'm too close
                        }
                        else if (distanceSq < MathF.Pow(moveToTarget.SlowDownDistance, 2))
                        {
                            // Start slowing down
                            var distance = MathF.Sqrt(distanceSq);
                            var ratio = Calculator.Remap(distance, moveToTarget.MinDistance, moveToTarget.SlowDownDistance, 0.5f, 1);
                            e.SetAgentImpulse(delta.Normalized() * ratio);
                        }
                        else
                        {
                            // Regular move
                            var direction = delta.Normalized();
                            e.SetAgentImpulse(direction);
                        }
                    }
                    else
                    {
                        // Target is gone, remove component
                        e.RemoveMoveToTarget();
                    }
                }
            }
        }
    }
}