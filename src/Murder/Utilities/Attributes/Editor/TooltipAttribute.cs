namespace Murder.Attributes
{
    /// <summary>
    /// Tooltip that will show up when hovering over a field in the editor.
    /// </summary>
    public class TooltipAttribute : Attribute
    {
        /// <summary>
        /// The content of the tooltip.
        /// </summary>
        public string Text = string.Empty;

        /// <summary>
        /// Creates a new <see cref="TooltipAttribute"/>.
        /// </summary>
        /// <param name="text">The content of the tooltip.</param>
        public TooltipAttribute(string text) => Text = text;
    }
}