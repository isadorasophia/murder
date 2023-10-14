using Murder.Attributes;

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
    }
}