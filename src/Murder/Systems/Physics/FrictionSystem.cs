using Bang.Contexts;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems
{
    [Filter(typeof(VelocityComponent), typeof(FrictionComponent))]
    internal class FrictionSystem : IFixedUpdateSystem
    {
        public ValueTask FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                var velocity = e.GetComponent<VelocityComponent>();
                var friction = e.GetComponent<FrictionComponent>();

                e.ReplaceComponent(new VelocityComponent(velocity.Velocity * (1 - friction.Amount)));
            }

            return default;
        }
    }
}
