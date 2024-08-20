using Bang.Components;
using Murder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components;

[DoNotPersistOnSave]
public readonly struct MoveToMaxTimeComponent : IComponent
{
    public readonly float RemoveAt;

    public MoveToMaxTimeComponent(float removeAt)
    {
        RemoveAt = removeAt;
    }
}
