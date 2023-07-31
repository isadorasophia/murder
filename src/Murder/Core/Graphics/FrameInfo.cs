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
    
    /// <summary>
    /// A string ID representing the events played since the last played frame (if any). Usually set in Aseprite.
    /// </summary>
    public readonly ImmutableArray<string> Event = ImmutableArray<string>.Empty;

    public readonly Animation Animation;

    public FrameInfo(int frame, bool animationComplete, ReadOnlySpan<char> @event, Animation animation)
    {
        Frame = frame;
        Complete = animationComplete;
        Event = ImmutableArray.Create(@event.ToString());
        Animation = animation;
    }

    public FrameInfo(int frame, bool animationComplete, ImmutableArray<string> @event, Animation animation)
    {
        Frame = frame;
        Complete = animationComplete;
        Debug.Assert(@event != null);
        Event = @event;
        Animation = animation;
    }

    public FrameInfo(int frame, bool animationComplete, Animation animation) : this(animation)
    {
        Frame = frame;
        Complete = animationComplete;
        Event = ImmutableArray<string>.Empty;
        Animation = animation;
    }

    public FrameInfo(Animation animation)
    {
        Frame = 0;
        Complete = false;
        Event = ImmutableArray<string>.Empty;
        Animation = animation;
    }
    public FrameInfo()
    {
        Frame = 0;
        Complete = false;
        Event = ImmutableArray<string>.Empty;
    }
}
