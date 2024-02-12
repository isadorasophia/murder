using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Systems;

[Filter(typeof(FacingComponent), typeof(VelocityTowardsFacingComponent))]
[Watch(typeof(VelocityTowardsFacingComponent))]
public class VelocityTowardsFacingSystem : IReactiveSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        foreach (var e in entities)
        {
            if (e.TryGetVelocityTowardsFacing() is VelocityTowardsFacingComponent velocityTowards)
            {
                e.SetVelocity(velocityTowards.Velocity * new Vector2(MathF.Sin(e.GetFacing().Angle), MathF.Cos(e.GetFacing().Angle)));
            }
            e.RemoveVelocityTowardsFacing();
        }
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        
    }
}
