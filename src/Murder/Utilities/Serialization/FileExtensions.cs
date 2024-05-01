using Murder.Diagnostics;

namespace Murder.Serialization;

public static class FileExtensions
{
    public static string EscapePath(this string path)
    {
        return path
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
    }
}
