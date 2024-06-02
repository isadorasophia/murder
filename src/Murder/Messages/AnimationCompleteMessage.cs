using Bang.Components;

namespace Murder.Messages;

public enum AnimationCompleteStyle
{
    Single,
    Sequence
}

public readonly struct AnimationCompleteMessage : IMessage
{
    public readonly AnimationCompleteStyle CompleteStyle;
    public AnimationCompleteMessage(AnimationCompleteStyle style)
    {
        CompleteStyle = style;
    }
}