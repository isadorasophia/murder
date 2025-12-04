using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;

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
                        e.ReplaceComponent(new VelocityComponent(velocity.Velocity * (1 - friction.AirFriction.Value)));
                        continue;
                    }
                }

                e.ReplaceComponent(new VelocityComponent(velocity.Velocity * (1 - friction.Amount)));
            }
        }
    }
}