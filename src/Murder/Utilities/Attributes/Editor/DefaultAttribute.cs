namespace Murder.Attributes
{
    /// <summary>
    /// Text which will be displayed when a field has a default value.
    /// </summary>
    public class DefaultAttribute : Attribute
    {
        /// <summary>
        /// The content which will be displayed in the button to create a new value of the default field.
        /// </summary>
        public string Text = string.Empty;

        /// <summary>
        /// Creates a new <see cref="DefaultAttribute"/>.
        /// </summary>
        /// <param name="text">The content which will be displayed in the button to create a new value of the default field.</param>
        public DefaultAttribute(string text) => Text = text;
    }
}