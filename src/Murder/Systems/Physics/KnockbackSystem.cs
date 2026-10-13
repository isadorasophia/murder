using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components.Agents;
using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Systems.Physics;

[Filter(ContextAccessorFilter.AllOf, typeof(KnockbackComponent))]
public class KnockbackSystem : IFixedUpdateSystem
{
    public void FixedUpdate(Context context)
    {
        foreach (Entity e in context.Entities)
        {
            KnockbackComponent knockback = e.GetKnockback();
            Vector2 velocity = e.TryGetVelocity()?.Velocity ?? Vector2.Zero;

            velocity = Calculator.LerpSmooth(velocity, Vector2.Zero, Game.FixedDeltaTime, knockback.HalfLife);

            if (Game.Now >= knockback.EndTime || velocity.LengthSquared() < 1f)
            {
                e.RemoveKnockback();
                e.RemoveVelocity();
                continue;
            }

            e.SetVelocity(velocity);
        }
    }
}
