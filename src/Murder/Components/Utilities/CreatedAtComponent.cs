using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components;

[RuntimeOnly, DoNotPersistOnSave]
public readonly struct CreatedAtComponent : IComponent
{
    [Tooltip("Time that this entity was created, in seconds")]
    public readonly float When { get; init; }

    public CreatedAtComponent(float when)
    {
        When = when;
    }
}
