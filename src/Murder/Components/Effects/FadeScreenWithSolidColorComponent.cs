using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;

namespace Murder.Components;

[DoNotPersistOnSave]
public readonly struct FadeScreenWithSolidColorComponent : IComponent
{
    public readonly Color Color;
    public readonly FadeType FadeType;

    public readonly float Duration;
    public readonly float Sort = .05f;

    public readonly int BatchId = Batches2D.UiBatchId;

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

    public FadeScreenWithSolidColorComponent(int batchId, Color color, FadeType fade, float duration, float sort)
        : this(color, fade, duration)
    {
        BatchId = batchId;
        Sort = sort;
    }
}
