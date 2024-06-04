using Bang.Components;
using Murder.Messages;
using Murder.Utilities.Attributes;

namespace Murder.Components;

public readonly struct ChangeSpriteBatchOnAnimationCompleteComponent : IComponent
{
    [SpriteBatchReference]
    public readonly int SpriteBatch = 0;

    public readonly AnimationCompleteStyle OnCompleteStyle = AnimationCompleteStyle.Sequence;

    public ChangeSpriteBatchOnAnimationCompleteComponent()
    {
        
    }

    public ChangeSpriteBatchOnAnimationCompleteComponent(int spriteBatch, AnimationCompleteStyle onCompleteStyle)
    {
        SpriteBatch = spriteBatch;
        OnCompleteStyle = onCompleteStyle;
    }
}
