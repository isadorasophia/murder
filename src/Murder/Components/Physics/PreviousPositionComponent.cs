using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Physics;

[RuntimeOnly, DoNotPersistOnSave]
public readonly struct PreviousPositionComponent : IComponent, IDoNotCheckOnReplaceTag
{
    public readonly Vector2 PreviousPosition;

    public PreviousPositionComponent(Vector2 previousPosition)
    {
        this.PreviousPosition = previousPosition;
    }
}
