namespace Murder.Core.Dialogs
{
    public readonly struct DialogLine
    {
        public readonly Line? Line = null;

        public readonly ChoiceLine? Choice = null;

        public DialogLine(Line line) => Line = line;

        public DialogLine(ChoiceLine choice) => Choice = choice;
    }
}
