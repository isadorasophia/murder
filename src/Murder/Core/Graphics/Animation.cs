using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Core.Graphics
{
    public readonly struct Animation
    {

        /// <summary>
        /// A struct representing information about a single animation frame, such as its index in the list and a flag indicating whether the animation is complete
        /// </summary>
        public ref struct FrameInfo
        {
            /// <summary>
            /// The index of the current frame
            /// </summary>
            public readonly int Frame;
            /// <summary>
            /// Whether the animation is complete
            /// </summary>
            public readonly bool AnimationComplete;
            /// <summary>
            /// A string ID representing the event associated with the current frame (if any). Usually set in Aseprite
            /// </summary>
            public readonly ReadOnlySpan<char> Event;

            public FrameInfo(int frame, bool animationComplete, ReadOnlySpan<char> @event)
            {
                Frame = frame;
                AnimationComplete = animationComplete;
                Event = @event;
            }

            public FrameInfo(int frame, bool animationComplete) : this()
            {
                Frame = frame;
                AnimationComplete = animationComplete;
            }
        }
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

        public Animation()
        {
            Frames = ImmutableArray<int>.Empty;
            FramesDuration = ImmutableArray<float>.Empty;
            Events = ImmutableDictionary<int, string>.Empty;
        }
        public Animation(int[] frames, float[] framesDuration)
        {
            (Frames, FramesDuration) = (frames.ToImmutableArray(), framesDuration.ToImmutableArray());

            var duration = 0f;
            foreach (var d in framesDuration)
            {
                duration += d;
            }
            AnimationDuration = duration / 1000f;
        }
        /// <summary>
        ///  A property representing the number of frames in the animation
        /// </summary>
        public int FrameCount => Frames.Length;

        /// <summary>
        /// Evaluates the current frame of the animation, given a time value (in seconds)
        /// and an optional maximum animation duration (in seconds)
        /// </summary>
        public FrameInfo Evaluate(float time) => Evaluate(time, -1);

        /// <summary>
        /// Evaluates the current frame of the animation, given a time value (in seconds)
        /// and an optional maximum animation duration (in seconds)
        /// </summary>
        public FrameInfo Evaluate(float time, float forceAnimationDuration)
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
                return new FrameInfo(0, true);

            if (time < 0)
            {
                time = (time % animationDuration + animationDuration) % animationDuration;
            }

            var delta = time % animationDuration;

            if (FrameCount > 0)
            {
                int frame = -1;

                // Use a switch statement to calculate the current frame index, which can improve performance for small frame counts
                switch (Frames.Length)
                {
                    case 1:
                        frame = 0;
                        break;
                    case 2:
                        frame = delta >= FramesDuration[0] ? 1 : 0;
                        break;
                    default:
                        for (float current = 0; current <= delta; current += factor * FramesDuration[frame % FramesDuration.Length] / 1000f)
                        {
                            frame++;
                        }
                        break;
                }

                // Use TryGetValue to avoid exceptions when accessing the FramesDuration dictionary
                if (FramesDuration.TryGet(frame % FramesDuration.Length) is float frameDuration)
                {
                    return new FrameInfo(Frames[frame % FramesDuration.Length], time + Game.FixedDeltaTime * 2 >= animationDuration, Events.ContainsKey(frame) ? Events[frame] : ReadOnlySpan<char>.Empty);
                }
                else
                {
                    return new FrameInfo(0, true);
                }
            }
            else
            {
                return new FrameInfo(0, true);
            }
        }
    }
}
