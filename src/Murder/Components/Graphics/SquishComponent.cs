using Bang.Components;
using Murder.Attributes;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components;

[RuntimeOnly, DoNotPersistOnSave]
public readonly struct SquishComponent : IComponent
{
    public readonly EaseKind EaseIn;
    public readonly EaseKind EaseOut;
    public readonly float Start;
    public readonly float Duration;
    public readonly float Amount;
    public readonly bool ScaledTime { get; init; }

    public SquishComponent(EaseKind easeIn, EaseKind easeOut, float start, float duration, float amount)
    {
        EaseIn = easeIn;
        EaseOut = easeOut;
        Start = start;
        Duration = duration;
        Amount = amount;
    }
}
