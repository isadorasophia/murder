using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Physics;

namespace Murder.Systems.Physics
{
    [Filter(typeof(VerticalPositionComponent))]
    public class VerticalPhysicsSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                float bounciness = 0.6f;
                float gravity = 1f;
                if (e.TryGetBounceAmount() is BounceAmountComponent bounceOverride)
                {
                    bounciness = bounceOverride.Bounciness;
                    gravity = bounceOverride.GravityMod;
                }

                float multiplier = 1;
                if (e.TryGetGravityMultiplier() is GravityMultiplierComponent gravityMultiplier)
                {
                    multiplier = gravityMultiplier.Multiply;
                    bounciness *= multiplier;
                }

                var verticalPosition = e.GetVerticalPosition().UpdatePosition(Game.FixedDeltaTime * gravity, bounciness, multiplier);

                if (verticalPosition.Z == 0)
                {
                    e.SendTouchedGroundMessage();

                    if (verticalPosition.ZVelocity == 0)
                    {
                        e.RemoveVerticalPosition();
                        continue;
                    }
                }

                e.SetVerticalPosition(verticalPosition);
            }
        }
    }
}