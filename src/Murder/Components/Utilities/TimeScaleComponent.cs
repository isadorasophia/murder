using Bang.Components;
using Murder.Attributes;

namespace Murder.Components;

[DoNotPersistOnSave]
public readonly struct TimeScaleComponent(float scale) : IComponent
{
    public readonly float Value = scale;
}
