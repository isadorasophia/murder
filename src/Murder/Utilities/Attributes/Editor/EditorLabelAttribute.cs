namespace Murder.Attributes
{
    /// <summary>
    /// Label that will show up for this field in the editor.
    /// </summary>
    public class EditorLabelAttribute : Attribute
    {
        /// <summary>
        /// The content of the tooltip.
        /// </summary>
        public readonly string Label1 = string.Empty;

        /// <summary>
        /// [Optional] Secondary label.
        /// </summary>
        public readonly string Label2 = string.Empty;

        public EditorLabelAttribute(string label1) => Label1 = label1;

        public EditorLabelAttribute(string label1, string label2) =>
            (Label1, Label2) = (label1, label2);
    }
}