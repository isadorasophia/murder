using Bang.Components;
using Bang.Interactions;
using System.Collections.Immutable;

namespace Murder.Components;

public readonly struct FireOnActivatedComponent : IComponent
{
    public readonly ImmutableArray<IInteractiveComponent> Actions = [];

    public FireOnActivatedComponent() { }
}
