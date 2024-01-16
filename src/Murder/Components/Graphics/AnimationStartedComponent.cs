using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Graphics;

[RuntimeOnly, DoNotPersistOnSave]
public readonly struct AnimationStartedComponent : IComponent
{
    public readonly float StartTime;

    public AnimationStartedComponent(float startTime)
    {
        StartTime = startTime;
    }
}
