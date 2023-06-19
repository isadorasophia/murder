namespace Murder.Attributes
{
    /// <summary>
    /// Tooltip that will show up when hovering over a field in the editor.
    /// </summary>
    public class EditorTupleTooltipAttribute : Attribute
    {
        /// <summary>
        /// The content of the tooltip.
        /// </summary>
        public string Tooltip1 = string.Empty;

        /// <summary>
        /// The content of the tooltip.
        /// </summary>
        public string? Tooltip2 = string.Empty;

        public EditorTupleTooltipAttribute(string tooltip1, string? tooltip2 = null) => 
            (Tooltip1, Tooltip2) = (tooltip1, tooltip2);
    }
}
