using System.Collections.Immutable;

namespace Murder.Core.Graphics
{
    public readonly struct Animation
    {
        public readonly ImmutableArray<int> Frames = ImmutableArray<int>.Empty;
        public readonly ImmutableArray<float> FramesDuration = ImmutableArray<float>.Empty;
        public readonly float AnimationDuration = 1;
        
        public Animation()
        {
            Frames = ImmutableArray<int>.Empty;
            FramesDuration = ImmutableArray<float>.Empty;
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

        public int FrameCount => Frames.Length;
        
        
        public (int animationFrame, bool complete) Evaluate(float time) => Evaluate(time, -1);
        public (int animationFrame, bool complete) Evaluate(float time, float forceAnimationDuration)
        {
            var animationDuration = AnimationDuration;
            var factor = 1f;

            if (forceAnimationDuration > 0)
            {
                factor = forceAnimationDuration / AnimationDuration;
                animationDuration = forceAnimationDuration;
            }

            if (animationDuration == 0)
                return (0, true);

            if (time < 0)
            {
                time = (time % animationDuration + animationDuration) % animationDuration;
            }

            var delta = time % animationDuration;

            if (FrameCount > 0)
            {
                int frame = -1;
                for (float current = 0; current <= delta; current += factor * FramesDuration[frame % FramesDuration.Length] / 1000f) 
                {
                    frame++;
                }
                return (Frames[frame % FramesDuration.Length], time + Game.FixedDeltaTime * 2 >= animationDuration);
            }
            else
                return (0, true);
        }
    }
}
