namespace Murder.Attributes
{
    /// <summary>
    /// A slider attribute used when setting values in the editor.
    /// </summary>
    public class SliderAttribute : Attribute
    {
        /// <summary>
        /// Minimum value.
        /// </summary>
        public readonly float Minimum = 0;

        /// <summary>
        /// Maximum value.
        /// </summary>
        public readonly float Maximum = 1;

        /// <summary>
        /// Creates a new <see cref="SliderAttribute"/>.
        /// </summary>
        /// <param name="minimum">Minimum value.</param>
        /// <param name="maximum">Maximum value.</param>
        public SliderAttribute(float minimum = 0, float maximum = 1) => (Minimum, Maximum) = (minimum, maximum);
    }
}
