namespace Murder.Assets.Localization
{
    public readonly struct LanguageIdData
    {
        public readonly LanguageId Id;
        public readonly string Identifier;

        public LanguageIdData(LanguageId id, string identifier) => (Id, Identifier) = (id, identifier);
    }

    public static class Languages
    {
        private static LanguageIdData[]? _all = null;

        public static LanguageIdData[] All => _all ??= [ English, Portuguese, Japanese];

        public static LanguageIdData Get(LanguageId id) =>
            All[(int)id];

        public static LanguageIdData Next(LanguageId id) =>
            All[((int)id + 1) % All.Length];

        public static readonly LanguageIdData English = new(LanguageId.English, "en-US");
        public static readonly LanguageIdData Portuguese = new(LanguageId.Portuguese, "pt-BR");
        public static readonly LanguageIdData Japanese = new(LanguageId.Japanese, "ja");
    }

    public enum LanguageId
    {
        English = 0,
        Portuguese = 1,
        Japanese = 2
    }
}
