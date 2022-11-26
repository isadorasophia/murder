using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems.Physics
{
    [Filter(typeof(VerticalPositionComponent))]
    public class VerticalPhysicsSystem : IFixedUpdateSystem
    {
        public ValueTask FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                e.SetVerticalPosition(e.GetVerticalPosition().UpdatePosition(Game.FixedDeltaTime));
            }

            return default;
        }
    }
}
