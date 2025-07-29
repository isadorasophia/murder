using Bang.Components;
using Bang.Interactions;
using Murder.Core;
using System.Collections.Immutable;

namespace Murder.Component;

public readonly struct OnlyTriggerInteractionOnComponent : IComponent
{
    public readonly ImmutableArray<ICondition> Conditions { get; init; } = [];

    public readonly IInteractiveComponent? OnFailure { get; init; } = null;

    public OnlyTriggerInteractionOnComponent() { }
}
