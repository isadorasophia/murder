using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Utilities;
using System.Numerics;
using Murder.Diagnostics;
using Bang;
using System.Collections.Immutable;
using Murder.Services;
using System.Runtime.CompilerServices;
using Murder.Helpers;

namespace Murder.Systems
{
    [Filter(ContextAccessorFilter.AllOf, typeof(TetheredComponent), typeof(PositionComponent))]
    internal class TetherSystem : IFixedUpdateSystem
    {
        private const int MaxIterations = 5; // Maximum number of iterations to resolve constraints

        public void FixedUpdate(Context context)
        {
            for (int iteration = 0; iteration < MaxIterations; iteration++)
            {
                foreach (var entity in context.Entities)
                {
                    AdjustPosition(context.World, entity);
                }
            }
        }

        private void AdjustPosition(World world, Entity e)
        {
            var tetherComponent = e.GetTethered();
            Vector2 position = e.GetGlobalPosition();
            bool isStatic = e.HasStatic();

            foreach (var tetherPoint in tetherComponent.TetherPoints)
            {
                if (world.TryGetEntity(tetherPoint.Target) is not Entity target)
                {
                    GameLogger.Warning($"Couldn't find target {tetherPoint.Target} for {e.EntityId}, skipping tether point");
                    continue;
                }

                Vector2 targetPosition = target.GetGlobalPosition();
                Vector2 direction = targetPosition - position;
                float distance = direction.Length();

                if (distance > tetherPoint.Length)
                {
                    float adjustmentDistance = distance - tetherPoint.Length;
                    Vector2 adjustment = direction.NormalizedWithSanity() * adjustmentDistance * 0.5f;

                    if (!isStatic)
                    {
                        e.SetFacing(DirectionHelper.FromVector(direction));
                        if (target.TryGetVelocity()?.Velocity is Vector2 velocity)
                        {
                            e.AddVelocity(velocity * 0.5f / (float)MaxIterations);
                        }

                        position += adjustment;
                    }

                    if (!target.HasStatic())
                    {
                        targetPosition -= adjustment;
                        target.SetGlobalPosition(targetPosition);
                    }

                    e.SetGlobalPosition(position);
                }
            }
        }
    }
}
