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
    public FrameInfo Evaluate(float time, float lastFrameTime, bool animationLoop) => Evaluate(time, lastFrameTime, animationLoop, - 1);

    /// <summary>
    /// Evaluates the current frame of the animation, given a time value (in seconds)
    /// and an optional maximum animation duration (in seconds)
    /// </summary>
    public FrameInfo Evaluate(float time, float lastFrameTime, bool animationLoop, float forceAnimationDuration)
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
            return new FrameInfo(0, true, Events.ContainsKey(0) ? Events[0] : ReadOnlySpan<char>.Empty, this);

        if (FrameCount > 0)
        {
            int frame = -1;

            if (animationLoop)
            {
                // Use a switch statement to calculate the current frame index, which can improve performance for small frame counts
                switch (Frames.Length)
                {
                    case 1:
                        frame = 0;
                        break;
                    case 2:
                        {
                            if (time < 0)
                            {
                                time = (time % animationDuration + animationDuration) % animationDuration;
                            }
                            var delta = time % animationDuration;
                            frame = delta >= (FramesDuration[0] / 1000f) ? 1 : 0;
                            break;
                        }
                    default:
                        {
                            if (time < 0)
                            {
                                time = (time % animationDuration + animationDuration) % animationDuration;
                            }
                            var delta = time % animationDuration;
                            for (float current = 0; current <= delta; current += factor * FramesDuration[frame % FramesDuration.Length] / 1000f)
                            {
                                frame++;
                            }
                        }
                        break;
                }
            }
            else
            {
                for (float current = 0; current <= time; current += factor * FramesDuration[frame % FramesDuration.Length] / 1000f)
                {
                    frame++;
                    if (frame == FramesDuration.Length - 1)
                        break;
                }
                if (frame < 0)
                    frame = 0;
            }

            int previousFrame = EvaluatePreviousFrame(lastFrameTime, animationDuration, factor);
            int clampedFrame = Math.Clamp(frame, 0, Frames.Length - 1);
            if (previousFrame != frame)
            {
                var events = ImmutableArray.CreateBuilder<string>();
                for (int i = previousFrame; i < frame; i++)
                {
                    if (Events.ContainsKey(i))
                    {
                        events.Add(Events[i]);
                    }
                }
                return new FrameInfo(Frames[clampedFrame], time + Game.FixedDeltaTime * 2 >= animationDuration, events.ToImmutable(), this);
            }
            else
            {
                return new FrameInfo(Frames[clampedFrame], time + Game.FixedDeltaTime * 2 >= animationDuration, this);
            }
        }
        else
        {
            // Animation has no length, this shouldn't happen.
            GameLogger.Error("Animation with no frames found!");
            return new FrameInfo(0, true, this);
        }
    }

    public int EvaluatePreviousFrame(float time, float animationDuration, float factor)
    {
        // Handle a zero animation duration separately to avoid division by zero errors
        if (animationDuration == 0)
            return 0;

        if (FrameCount > 0)
        {
            int frame = -1;

            if (time < 0)
            {
                time = (time % animationDuration + animationDuration) % animationDuration;
            }
            var delta = time % animationDuration;
            for (float current = 0; current <= delta; current += factor * FramesDuration[frame % FramesDuration.Length] / 1000f)
            {
                frame++;
            }

            // Use TryGetValue to avoid exceptions when accessing the FramesDuration dictionary
            return frame % FramesDuration.Length;
        }
        else
        {
            // Animation has no length, this shouldn't happen.
            GameLogger.Error("Animation with no frames found!");
            return -1;
        }
    }
}
