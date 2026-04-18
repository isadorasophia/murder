using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Agents;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems
{
    [Filter(typeof(VelocityComponent), typeof(FrictionComponent))]
    internal class FrictionSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                var velocity = e.GetVelocity();
                var friction = e.GetFriction();

                if (friction.AirFriction != null)
                {
                    if (e.TryGetVerticalPosition() is VerticalPositionComponent verticalPosition && verticalPosition.Z > 0)
                    {
                        e.SetVelocity(velocity.Velocity * (1 - friction.AirFriction.Value));
                        continue;
                    }
                }

                // Check if we are inside a gravitywell, if so, we do not reduce the velocity towards it.
                if (e.TryGetInsideGravityWell() is InsideGravityWellComponent gravityWell
                    && context.World.TryGetEntity(gravityWell.GravityWellId) is Entity gravityWellEntity)
                {
                    Vector2 toWell = gravityWellEntity.GetGlobalPosition() - e.GetGlobalPosition();
                    float lengthSq = toWell.LengthSquared();

                    if (lengthSq > 0.0001f)
                    {
                        Vector2 axis = toWell / MathF.Sqrt(lengthSq);

                        float alongAxis = Vector2.Dot(velocity.Velocity, axis);
                        Vector2 alongComponent = axis * alongAxis;
                        Vector2 perpComponent = velocity.Velocity - alongComponent;

                        // Perpendicular (orbit) gets normal friction
                        perpComponent *= (1 - friction.Amount);

                        // Only apply friction if moving AWAY from the well, use dot for that
                        if (alongAxis < 0)
                        {
                            alongComponent *= (1 - friction.Amount);
                        }

                        e.SetVelocity((alongComponent + perpComponent) * (1 - friction.Amount * 0.03f)); // we still a bit
                        continue;
                    }
                }

                e.SetVelocity(velocity.Velocity * (1 - friction.Amount));
            }
        }
    }
}