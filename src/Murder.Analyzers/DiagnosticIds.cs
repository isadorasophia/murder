namespace Murder.Analyzers;

public static class Diagnostics
{
    public static class Attributes
    {
        public static class ImporterSettingsAttribute
        {
            public const string Id = "MRDR0001";
            public const string Message = "ResourceImporter requires ImporterSettingsAttribute";
        }

        public static class RuntimeOnlyAttributeOnNonComponent
        {
            public const string Id = "MRDR0002";
            public const string Message = "RuntimeOnly attribute must be used only on components";
        }
    }
}