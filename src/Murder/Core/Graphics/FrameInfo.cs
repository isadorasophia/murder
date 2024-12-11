using Murder.Core.Graphics;

namespace Murder.Core;
/// <summary>
/// A struct representing information about a single animation frame, such as its index in the list and a flag indicating whether the animation is complete
/// </summary>
public readonly struct FrameInfo
{
    public static FrameInfo Fail => new() { Failed = true };

    /// <summary>
    /// The index of the current frame
    /// </summary>
    public readonly int Frame { get; init; }

    /// <summary>
    /// The index of the current frame inside the current animation
    /// </summary>
    public readonly int InternalFrame;

    /// <summary>
    /// Whether the animation is complete
    /// </summary>
    public readonly bool Complete;

    public readonly bool Failed { get; init; }

    public readonly Animation Animation { get; init; }

    public FrameInfo(int frame, int internalFrame, bool animationComplete, Animation animation)
    {
        Frame = frame;
        InternalFrame = internalFrame;
        Complete = animationComplete;
        Animation = animation;
    }

    public FrameInfo(Animation animation)
    {
        Frame = 0;
        Complete = false;
        Animation = animation;
    }
    public FrameInfo()
    {
        Frame = 0;
        Complete = false;
    }
}