using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Messages;

namespace Murder.Systems.Physics
{
    [Filter(typeof(VerticalPositionComponent))]
    public class VerticalPhysicsSystem : IFixedUpdateSystem
    {
        public ValueTask FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                var verticalPosition = e.GetVerticalPosition().UpdatePosition(Game.FixedDeltaTime);
                if (verticalPosition.Z == 0)
                {
                    e.SendMessage(new TouchedGroundMessage());
                    
                    if (verticalPosition.ZVelocity == 0)
                    {
                        e.RemoveVerticalPosition();
                        continue;
                    }
                }

                e.SetVerticalPosition(verticalPosition);
            }

            return default;
        }
    }
}
