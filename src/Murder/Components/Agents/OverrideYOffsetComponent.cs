using Bang.Components;
using Murder.Utilities.Attributes;

namespace Murder.Components;

[RuntimeOnly]
public readonly struct OverrideYOffsetComponent : IComponent
{
    public readonly float YOffset = 0;

    public OverrideYOffsetComponent() { }
}
