using Murder.Attributes;
using Murder.Utilities;

namespace Murder.Components
{
    /// <summary>
    /// Range of float values.
    /// </summary>
    public readonly struct FloatRange
    {
        [Slider(0, 1)]
        public readonly float Start;

        [Slider(0, 1)]
        public readonly float End;

        public FloatRange(float start, float end) => (Start, End) = (start, end);

        public bool Contains(float v) => v >= Start && v <= End;

        public float GetRandom() => GetRandom(Game.Random);
        public float GetRandom(Random random)
        {
            return random.NextFloat(Start, End);
        }

        public float Get(float progress)
        {
            return Calculator.Lerp(Start, End, progress);
        }
    }
}