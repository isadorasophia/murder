using System.Security.Cryptography;
using System.Text;

namespace Murder.Serialization;

public static class EditorFileExtensions
{
    public static Guid GuidFromName(string name)
    {
        using var md5 = MD5.Create();
        Guid guid = new Guid(md5.ComputeHash(Encoding.Default.GetBytes(name)));
        return guid;
    }

    /// <summary>
    /// This will create a directory on the root of this <paramref name="filePath"/>,
    /// if the directory is not available.
    /// </summary>
    public static void CreateDirectoryPathIfNotExists(string filePath)
    {
        string? directoryPath = Path.GetDirectoryName(filePath);
        if (directoryPath != null && !Directory.Exists(directoryPath))
        {
            _ = FileHelper.GetOrCreateDirectory(directoryPath);
        }
    }

    /// <summary>
    /// Used to normalize file paths from different OS into the same output.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string Normalize(string source)
    {
        return source.ToLowerInvariant().Replace('\\', '/');
    }
}
