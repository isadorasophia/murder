using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components;


[Flags]
public enum ChangeFrictionFlags
{
    None = 0,
    Reduce = 1 << 0,
    Increase = 1 << 1,
}

[RuntimeOnly, DoNotPersistOnSave]
public readonly struct ChangeAgentFrictionUntil : IComponent
{
    public readonly ChangeFrictionFlags FrictionFlags;
    public readonly float Amount;
    public readonly float StartTime;
    public readonly float UntilTime;

    public ChangeAgentFrictionUntil(ChangeFrictionFlags frictionFlags, float amount, float startTime, float duration)
    {
        FrictionFlags = frictionFlags;
        Amount = amount;
        StartTime = startTime;
        UntilTime = startTime + duration;
    }
}
