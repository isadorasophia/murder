namespace Murder.Services
{
    public struct MenuOption
    {
        public readonly string Text;
        public readonly bool Selectable;

        /// <summary>
        /// Length of the text option.
        /// </summary>
        public int Length => Text.Length;

        public MenuOption(string text, bool selectable = true)
        {
            Text = text;
            Selectable = selectable;
        }
    }
}