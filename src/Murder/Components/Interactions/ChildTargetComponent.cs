using Bang.Components;
using Murder.Utilities.Attributes;

namespace Murder.Components;

public readonly struct ChildTargetComponent : IComponent
{
    [ChildId]
    public readonly string Name = string.Empty;

    public ChildTargetComponent() { }
}
