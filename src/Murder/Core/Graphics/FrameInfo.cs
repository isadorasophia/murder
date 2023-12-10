using Murder.Core.Graphics;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Murder.Core;
/// <summary>
/// A struct representing information about a single animation frame, such as its index in the list and a flag indicating whether the animation is complete
/// </summary>
public readonly struct FrameInfo
{
    internal static FrameInfo Fail => new() { Failed = true };

    /// <summary>
    /// The index of the current frame
    /// </summary>
    public readonly int Frame;

    /// <summary>
    /// Whether the animation is complete
    /// </summary>
    public readonly bool Complete;

    public readonly bool Failed { get; init; }

    public readonly Animation Animation { get; init; }

    public FrameInfo(int frame, bool animationComplete, Animation animation)
    {
        Frame = frame;
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