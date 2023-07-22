namespace Murder.Attributes
{
    /// <summary>
    /// Attributes for fields that should always show up in the editor.
    /// Commonly used for private fields.
    /// </summary>
    public class DefaultEditorSystemAttribute : Attribute
    {
        public readonly bool StartActive = true;

        public DefaultEditorSystemAttribute() { }

        public DefaultEditorSystemAttribute(bool startActive) => StartActive = startActive;
    }
}
