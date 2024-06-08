using Bang.Components;
using Murder.Core.Graphics;

namespace Murder.Components;

public readonly struct FadeScreenWithSolidColorComponent : IComponent
{
    public readonly Color Color;
    public readonly FadeType FadeType;
    public readonly float Duration;
    public readonly float Sort = .05f;

    public FadeScreenWithSolidColorComponent(Color color, FadeType fade, float duration)
    {
        Color = color;
        FadeType = fade;
        Duration = duration;
    }

    public FadeScreenWithSolidColorComponent(Color color, FadeType fade, float duration, float sort)
        : this(color, fade, duration)
    {
        Sort = sort;
    }
}
