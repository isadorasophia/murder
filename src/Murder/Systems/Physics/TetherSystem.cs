using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Utilities;
using System.Numerics;
using Murder.Diagnostics;
using Murder.Messages;
using Bang;

namespace Murder.Systems
{
    [Filter(ContextAccessorFilter.AllOf, typeof(TetheredComponent), typeof(PositionComponent))]
    internal class TetherSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                Vector2 position = e.GetGlobalTransform().Vector2;
                var tetherComponent = e.GetTethered();
                Vector2 velocity = e.TryGetVelocity()?.Velocity ?? Vector2.Zero;

                foreach (var tetherPoint in tetherComponent.TetherPoints)
                {
                    if (context.World.TryGetEntity(tetherPoint.Target) is not Entity target)
                    {
                        GameLogger.Warning($"Couldn't find target {tetherPoint.Target} for {e.EntityId}, skipping tether point");
                        continue;
                    }

                    Vector2 targetPosition = target.GetGlobalTransform().Vector2;
                    Vector2 direction = targetPosition - position;
                    float distance = direction.Length();

                    if (distance > tetherPoint.Length)
                    {
                        float adjustmentDistance = distance - tetherPoint.Length;

                        if (adjustmentDistance > tetherPoint.SnapDistance)
                        {
                            if (!TryAdjustPosition(context.World, e, target, tetherPoint.SnapDistance))
                            {
                                // Send a failure message if snapping fails
                                e.SendThetherSnapMessage(e.EntityId);
                                break;
                            }
                        }
                        else
                        {
                            Vector2 adjustment = direction.NormalizedWithSanity() * adjustmentDistance * 0.5f; // Split the adjustment between both entities

                            // Apply velocity adjustment
                            if (!target.HasStatic())
                            {
                                Vector2 targetVelocity = target.TryGetVelocity()?.Velocity ?? Vector2.Zero;
                                targetVelocity -= adjustment / Game.FixedDeltaTime;
                                target.SetVelocity(new VelocityComponent(targetVelocity));
                            }

                            if (!e.HasStatic())
                            {
                                velocity += 2 * adjustment / Game.FixedDeltaTime;
                            }
                        }
                    }
                }

                e.SetVelocity(new VelocityComponent(velocity));
            }
        }

        private bool TryAdjustPosition(World world, Entity e, Entity target, float snapDistance)
        {
            Vector2 position = e.GetGlobalTransform().Vector2;
            Vector2 targetPosition = target.GetGlobalTransform().Vector2;
            Vector2 direction = targetPosition - position;
            Vector2 snapAdjustment = direction.NormalizedWithSanity() * snapDistance;

            // Check if the target is attached to something else
            if (target.HasComponent<TetheredComponent>())
            {
                var targetTetherComponent = target.GetTethered();
                foreach (var tetherPoint in targetTetherComponent.TetherPoints)
                {
                    if (world.TryGetEntity(tetherPoint.Target) is not Entity nextTarget)
                    {
                        GameLogger.Warning($"Couldn't find target {tetherPoint.Target} for {target.EntityId}, skipping tether point");
                        continue;
                    }

                    if (!TryAdjustPosition(world, target, nextTarget, tetherPoint.SnapDistance))
                    {
                        // If adjustment fails, send a message and stop the entity
                        e.SendThetherSnapMessage(e.EntityId);
                        return false;
                    }
                }
            }

            // Apply the adjustment if the target is not static
            if (!target.HasStatic())
            {
                targetPosition -= snapAdjustment;
                target.SetGlobalPosition(targetPosition);
                target.SetVelocity(new VelocityComponent(Vector2.Zero)); // Stop the target's velocity
            }

            // Apply the adjustment to the entity
            if (!e.HasStatic())
            {
                position += snapAdjustment;
                e.SetGlobalPosition(position);
                e.SetVelocity(new VelocityComponent(Vector2.Zero)); // Stop the entity's velocity
            }

            return true;
        }
    }
}
