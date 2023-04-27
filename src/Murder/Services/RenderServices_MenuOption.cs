namespace Murder.Services
{
    public struct MenuOption
    {
        public readonly string? Text = null;
        public readonly bool Selectable = true;

        /// <summary>
        /// Length of the text option.
        /// </summary>
        public int Length => Text?.Length ?? 0;

        public MenuOption() { }

        public MenuOption(bool selectable) => Selectable = selectable;

        public MenuOption(string text, bool selectable = true) : this(selectable)
        {
            Text = text;
        }
    }
}