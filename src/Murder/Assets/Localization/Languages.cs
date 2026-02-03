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

        public static LanguageIdData[] All => _all ??= [
            English, 
            Portuguese, 
            Japanese, 
            Chinese, 
            TraditionalChinese, 
            French, 
            Italian,
            German, 
            SpanishSpain, 
            SpanishLatam, 
            Korean, 
            Russian, 
            Turkish, 
            Ukrainian, 
            Hungarian];

        public static LanguageIdData? TryGet(LanguageId id)
        {
            int index = (int)id;
            if (index >= All.Length)
            {
                return null;
            }

            return All[index];
        }

        public static readonly LanguageIdData English = new(LanguageId.English, "en-US");
        public static readonly LanguageIdData Portuguese = new(LanguageId.Portuguese, "pt-BR");
        public static readonly LanguageIdData Japanese = new(LanguageId.Japanese, "ja");
        public static readonly LanguageIdData Chinese = new(LanguageId.Chinese, "zh-Hans");
        public static readonly LanguageIdData TraditionalChinese = new(LanguageId.TraditionalChinese, "zh-Hant");
        public static readonly LanguageIdData French = new(LanguageId.French, "fr");
        public static readonly LanguageIdData Italian = new(LanguageId.Italian, "it");
        public static readonly LanguageIdData German = new(LanguageId.German, "de");
        public static readonly LanguageIdData SpanishSpain = new(LanguageId.SpanishSpain, "es-ES");
        public static readonly LanguageIdData SpanishLatam = new(LanguageId.SpanishLatam, "es-LATAM");
        public static readonly LanguageIdData Korean = new(LanguageId.Korean, "ko");
        public static readonly LanguageIdData Russian = new(LanguageId.Russian, "ru");
        public static readonly LanguageIdData Turkish = new(LanguageId.Turkish, "tr");
        public static readonly LanguageIdData Ukrainian = new(LanguageId.Ukrainian, "uk");
        public static readonly LanguageIdData Hungarian = new(LanguageId.Hungarian, "hu");
    }

    public enum LanguageId
    {
        English = 0,
        Portuguese = 1,
        Japanese = 2,
        Chinese = 3,
        TraditionalChinese = 4,
        French = 5,
        Italian = 6,
        German = 7,
        SpanishSpain = 8,
        SpanishLatam = 9,
        Korean = 10,
        Russian = 11,
        Turkish = 12,
        Ukrainian = 13,
        Hungarian = 14
    }
}
