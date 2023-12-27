namespace Murder.Analyzers;

public static class CodeFixes
{
    public static class AddAttribute
    {
        public static string Title(string attributeName) => $"Add {attributeName} attribute";
    }

    public static class AddInterface
    {
        public static string Title(string interfaceName) => $"Add {interfaceName} interface";
    }
}