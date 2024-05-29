using Bang.Components;

namespace Murder.Components;

public readonly struct TimeScaleComponent(float scale) : IComponent
{
    public readonly float Value = scale;
}
