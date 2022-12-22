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

                e.ReplaceComponent(new VelocityComponent(velocity.Velocity * (1 - friction.Amount)));
            }
        }
    }
}
