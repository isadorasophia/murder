using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Core.Graphics;

public readonly struct AnimationSequence
{
    public readonly string Next;
    public readonly float Chance;

    public AnimationSequence(string nextAnimation, float chance) : this()
    {
        Next = nextAnimation;
        Chance = chance;
    }

    public static AnimationSequence? CreateIfPossible(string userData)
    {
        if (string.IsNullOrWhiteSpace(userData))
        {
            return null;
        }

        var split = userData.Split(':');
        if (split.Length == 2)
        {
            float.TryParse(split[1], out float chance);
            return new AnimationSequence(split[0], chance);
        }
        else
        {
            return new AnimationSequence(userData, 1);
        }
    }
}
public readonly struct Animation
{
    public static Animation Empty = new Animation(
        [1],
        [0f],
        ImmutableDictionary<int, string>.Empty,
        null);

    /// <summary>
    /// An array of integers representing the indices of the frames in the animation
    /// </summary>
    public readonly ImmutableArray<int> Frames = ImmutableArray<int>.Empty;

    /// <summary>
    /// An array of floats representing the duration of each frame in the animation, in milliseconds
    /// </summary>
    public readonly ImmutableArray<float> FramesDuration = ImmutableArray<float>.Empty;

    /// <summary>
    /// A dictionary associating integer indices with event strings
    /// </summary>
    public readonly ImmutableDictionary<int, string> Events = ImmutableDictionary<int, string>.Empty;

    /// <summary>
    /// The total duration of the animation, in seconds
    /// </summary>
    public readonly float AnimationDuration = 1;

    public readonly AnimationSequence? NextAnimation;
    public Animation()
    {
        Frames = ImmutableArray<int>.Empty;
        FramesDuration = ImmutableArray<float>.Empty;
        Events = ImmutableDictionary<int, string>.Empty;
        NextAnimation = null;
    }

    public Animation(ImmutableArray<int> frames, ImmutableArray<float> framesDuration, ImmutableDictionary<int, string> events, AnimationSequence? sequence)
    {
        Frames = frames;
        FramesDuration = framesDuration;
        Events = events;
        AnimationDuration = FramesDuration.Sum() / 1000f;
        NextAnimation = sequence;
    }
    public Animation(int[] frames, float[] framesDuration, Dictionary<int, string> events, AnimationSequence? sequence)
    {
        Frames = frames.ToImmutableArray();
        FramesDuration = framesDuration.ToImmutableArray();
        Events = events.ToImmutableDictionary();
        AnimationDuration = FramesDuration.Sum() / 1000f;
        NextAnimation = sequence;
    }

    /// <summary>
    ///  A property representing the number of frames in the animation
    /// </summary>
    public int FrameCount => Frames.Length;

    /// <summary>
    /// Evaluates the current frame of the animation, given a time value (in seconds)
    /// and an optional maximum animation duration (in seconds)
    /// </summary>
    public FrameInfo Evaluate(float time, bool animationLoop) => Evaluate(time, animationLoop, -1);

    /// <summary>
    /// Evaluates the current frame of the animation, given a time value (in seconds)
    /// and an optional maximum animation duration (in seconds)
    /// </summary>
    public FrameInfo Evaluate(in float time, bool animationLoop, float forceAnimationDuration)
    {
        var animationDuration = AnimationDuration;
        var factor = 1f;

        if (forceAnimationDuration > 0)
        {
            factor = forceAnimationDuration / AnimationDuration;
            animationDuration = forceAnimationDuration;
        }

        // Handle a zero animation duration separately to avoid division by zero errors
        if (animationDuration == 0)
            return new FrameInfo(0, 0, true, this);

        if (FrameCount > 0)
        {
            int frame = GetCurrentFrame(time, animationLoop, FramesDuration, animationDuration, factor);
            int clampedFrame = Math.Clamp(frame, 0, Frames.Length - 1);

            return new FrameInfo(Frames[clampedFrame], clampedFrame, time >= animationDuration, this);
        }
        else
        {
            // Animation has no length, this shouldn't happen.
            GameLogger.Error("Animation with no frames found!");
            return new FrameInfo(0, 0, true, this);
        }
    }

    /// <summary>
    /// Gets the current frame of an animation at a given time.
    /// </summary>
    /// <param name="currentTime">Current time in seconds</param>
    /// <param name="animationLoops">If the animation should loop or stop at the last frame</param>
    /// <param name="FramesDuration">List durations for each frame in miliseconds</param>
    /// <param name="cachedAnimationDuration">Total sum of frames duration, cached for performance</param>
    /// <param name="factor">Time scale factor</param>
    /// <returns></returns>
    private static int GetCurrentFrame(float currentTime, bool animationLoops, ImmutableArray<float> FramesDuration, float cachedAnimationDuration, float factor)
    {
        // Scale the current time by the factor
        float scaledTime = currentTime * factor;

        // If the animation loops, modulate the scaled time with the total animation duration
        if (animationLoops)
        {
            scaledTime %= cachedAnimationDuration;
        }
        // If the animation does not loop and the time exceeds the duration, set to the last frame
        else if (scaledTime >= cachedAnimationDuration)
        {
            return FramesDuration.Length - 1;
        }
        
        // Animation frames are stored in milisseconds
        scaledTime *= 1000;

        // Iterate over each frame duration to find the current frame
        float accumulatedTime = 0;
        for (int i = 0; i < FramesDuration.Length; i++)
        {
            accumulatedTime += FramesDuration[i];
            if (accumulatedTime > scaledTime)
            {
                return i; // Return the current frame index
            }
        }

        // If for some reason the frame is not found, return the last frame
        return FramesDuration.Length - 1;
    }

    /// <summary>
    /// Get the current total frames at a given time while looping
    /// </summary>
    /// <param name="currentTime">Current time in seconds</param>
    /// <param name="FramesDuration">List durations for each frame in miliseconds</param>
    /// <param name="cachedAnimationDuration">Total sum of frames duration, cached for performance</param>
    /// <param name="factor">Time scale factor</param>
    /// <returns></returns>
    private static int GetTotalFrameCount(float currentTime, ImmutableArray<float> FramesDuration, float cachedAnimationDuration, float factor)
    {
        int sign = Math.Sign(currentTime);
        currentTime = Math.Abs(currentTime);

        // Scale the current time by the factor
        float scaledTime = currentTime * factor;

        // Determine how many complete cycles have elapsed
        int completeCycles = Calculator.FloorToInt(scaledTime / cachedAnimationDuration);
        float remainingTime = (scaledTime % cachedAnimationDuration) * 1000f;

        // Initialize variables
        float accumulatedTime = 0;
        int frameCount = 0;

        // Iterate over each frame duration for the remaining time
        foreach (var frameDuration in FramesDuration)
        {
            accumulatedTime += frameDuration;

            // Check if the accumulated time exceeds the remaining time
            if (accumulatedTime > remainingTime)
            {
                break; // Exit loop as we have reached the current time
            }

            frameCount++;
        }

        // Add the frames from complete cycles
        frameCount += completeCycles * FramesDuration.Length;

        return frameCount * sign;
    }
}