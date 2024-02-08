using Bang.Components;
using Murder.Components;
using Murder.Core.Graphics;

namespace Road.Components;

public readonly struct FadeScreenWithSolidColorComponent : IComponent
{
    public readonly Color Color;
    public readonly FadeType FadeType;
    public readonly float Duration;

    public FadeScreenWithSolidColorComponent(Color color, FadeType fade, float duration)
    {
        Color = color;
        FadeType = fade;
        Duration = duration;
    }
}
