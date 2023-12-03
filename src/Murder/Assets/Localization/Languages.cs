namespace Murder.Assets.Localization
{
    public readonly struct LanguageId
    {
        public readonly int Id;
        public readonly string Identifier;

        public LanguageId(int id, string identifier) => (Id, Identifier) = (id, identifier);
    }

    public static class Languages
    {
        private static LanguageId[]? _all = null;

        public static LanguageId[] All => _all ??= [ English, Portuguese ];

        public static LanguageId Next(LanguageId id) =>
            All[(id.Id + 1) % All.Length];

        public static readonly LanguageId English = new(0, "en-US");
        public static readonly LanguageId Portuguese = new(1, "pt-BR");
    }
}
