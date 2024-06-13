using Bang.Components;
using Murder.Utilities.Attributes;

namespace Murder.Components;

public readonly struct BroadcastEventMessageComponent : IComponent
{
    [ChildId]
    public readonly string Target = string.Empty;

    public BroadcastEventMessageComponent() { }
}
