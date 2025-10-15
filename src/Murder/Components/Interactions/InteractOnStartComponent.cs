using Bang.Components;
using Murder.Core;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

public enum InteractOnStartFlags
{
    None = 0,
    OnlyOnce = 1
}

[Story]
public readonly struct InteractOnStartComponent : IComponent
{
    public readonly ImmutableArray<ICondition>? Conditions = null;
    public readonly InteractOnStartFlags Flags = InteractOnStartFlags.None;

    public InteractOnStartComponent() { }

    public InteractOnStartComponent(ImmutableArray<ICondition> conditions, InteractOnStartFlags flags) 
    {
        Conditions = conditions;
        Flags = flags;
    }
}