namespace Murder.Services
{
    public struct MenuOption
    {
        public readonly string Text { get; init; } = string.Empty;
        public readonly bool Enabled { get; init; } = true;
        public readonly bool AllowHoverOnDisabled { get; init; } = false;

        /// <summary>
        /// Whether this option can be hovered.
        /// </summary>
        public bool CanHover => Enabled || AllowHoverOnDisabled;

        /// <summary>
        /// Length of the text option.
        /// </summary>
        public int Length => Text?.Length ?? 0;

        public bool SoundOnClick = true;

        public MenuOption() { }

        public MenuOption(bool selectable) => Enabled = selectable;

        public MenuOption(string text, bool selectable = true) : this(selectable)
        {
            Text = text;
        }
    }
}