using Bang.Components;
using Murder.Core.Geometry;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components;

[RuntimeOnly]
public readonly struct EntityBoundsCacheComponent : IComponent
{
    public readonly Rectangle Bounds;

    public EntityBoundsCacheComponent(Rectangle bounds)
    {
        Bounds = bounds;
    }
}
