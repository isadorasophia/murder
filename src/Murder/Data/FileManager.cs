using Murder.Assets;
using Murder.Data;
using Murder.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text.Json;

namespace Murder.Serialization;

/// <summary>
/// FileHelper which will do OS operations. This is system-agnostic.
/// </summary>
public partial class FileManager
{
    /// <summary>
    /// Pack content into a zip format that will be compressed and reduce IO time.
    /// </summary>
    public void PackContent<T>(T data, string path)
    {
        using FileStream stream = File.Open(path, FileMode.OpenOrCreate);
        using GZipStream gzipStream = new(stream, CompressionMode.Compress);

        SerializeToJson(gzipStream, data);

        gzipStream.Close();
        stream.Close();
    }

    /// <summary>
    /// Unpack content from a zip format. This reduces the overhead on IO time.
    /// </summary>
    public T? UnpackContent<T>(string path)
    {
        using FileStream stream = File.OpenRead(path);
        using GZipStream gzipStream = new(stream, CompressionMode.Decompress);

#if false
        // Useful for debugging compressed data only!
        using StreamReader reader = new(gzipStream);
        string content = reader.ReadToEnd();
        T? data = DeserializeFromJson<T>(content);
#else
        T? data = DeserializeFromJson<T>(gzipStream);
#endif

        gzipStream.Close();
        stream.Close();

        return data;
    }

    public void SaveText(in string fullpath, in string content)
    {
        GameLogger.Verify(Path.IsPathRooted(fullpath));

        if (!FileExistsWithCaseInsensitive(fullpath))
        {
            string directoryName = Path.GetDirectoryName(fullpath)!;
            _ = GetOrCreateDirectory(directoryName);
        }

        File.WriteAllText(fullpath, content);
    }

    public async Task SaveTextAsync(string fullpath, string content)
    {
        GameLogger.Verify(Path.IsPathRooted(fullpath));

        if (!FileExistsWithCaseInsensitive(fullpath))
        {
            string directoryName = Path.GetDirectoryName(fullpath)!;

            if (!string.IsNullOrWhiteSpace(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        await File.WriteAllTextAsync(fullpath, content);
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:Required members might get lost when trimming.", Justification = "Assembly is trimmed.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:JsonSerializer.Serialize with reflection may cause issues with trimmed assembly.", Justification = "We use source generators.")]
    public static string SerializeToJson<T>(T value)
    {
        GameLogger.Verify(value != null, $"Cannot serialize a null {typeof(T).Name}");

        return JsonSerializer.Serialize(value, Game.Data.SerializationOptions);
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:Required members might get lost when trimming.", Justification = "Assembly is trimmed.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:JsonSerializer.Serialize with reflection may cause issues with trimmed assembly.", Justification = "We use source generators.")]
    public static T? DeserializeFromJson<T>(string json)
    {
        T? asset = JsonSerializer.Deserialize<T>(json, Game.Data.SerializationOptions);
        return asset;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:Required members might get lost when trimming.", Justification = "Assembly is trimmed.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:JsonSerializer.Serialize with reflection may cause issues with trimmed assembly.", Justification = "We use source generators.")]
    public static void SerializeToJson<T>(Stream stream, T value)
    {
        GameLogger.Verify(value != null, $"Cannot serialize a null {typeof(T).Name}");
        JsonSerializer.Serialize(stream, value, Game.Data.SerializationOptions);
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:Required members might get lost when trimming.", Justification = "Assembly is trimmed.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:JsonSerializer.Serialize with reflection may cause issues with trimmed assembly.", Justification = "We use source generators.")]
    public static T? DeserializeFromJson<T>(Stream stream)
    {
        T? asset = JsonSerializer.Deserialize<T>(stream, Game.Data.SerializationOptions);
        return asset;
    }

    public string SaveSerialized<T>(T value, string path)
    {
        GameLogger.Verify(value != null, $"Cannot serialize a null {typeof(T).Name}");

        string json = SerializeToJson(value);
        SaveText(path, json);

        return json;
    }

    public async ValueTask<string> SaveSerializedAsync<T>(T value, string path)
    {
        GameLogger.Verify(value != null, $"Cannot serialize a null {typeof(T).Name}");

        string json = SerializeToJson(value);
        await SaveTextAsync(path, json);

        return json;
    }

    public T? DeserializeGeneric<T>(string path, bool warnOnErrors = true)
    {
        GameLogger.Verify(Path.IsPathRooted(path));

        if (!FileExistsWithCaseInsensitive(path))
        {
            if (warnOnErrors)
            {
                GameLogger.Warning($"Can't find {path} to deserialize");
            }

            return default;
        }

        string json = File.ReadAllText(path);
        return DeserializeFromJson<T>(json);
    }

    /// <summary>
    /// Deserialize and asset asynchronously. Assumes that <paramref name="path"/> is valid.
    /// </summary>
    public async Task<T?> DeserializeAssetAsync<T>(string path) where T : GameAsset
    {
        GameLogger.Verify(Path.IsPathRooted(path));

        try
        {
            string? json = await File.ReadAllTextAsync(path);

            T? asset = DeserializeFromJson<T>(json);
            asset?.AfterDeserialized();

            if (asset != null && asset.Guid == Guid.Empty)
            {
                asset.MakeGuid();
            }

            return asset;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public T? DeserializeAsset<T>(string path) where T : GameAsset
    {
        GameLogger.Verify(Path.IsPathRooted(path));

        if (!FileExistsWithCaseInsensitive(path))
        {
            GameLogger.Warning($"Can't find {path} to deserialize");
            return null;
        }

        var json = File.ReadAllText(path);

        try
        {
            T? asset = DeserializeFromJson<T>(json);
            asset?.AfterDeserialized();

            if (asset != null && asset.Guid == Guid.Empty)
            {
                asset.MakeGuid();
            }

            return asset;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static IEnumerable<string> ListAllDirectories(string path)
    {
        GameLogger.Verify(Path.IsPathRooted(path));

        if (!Directory.Exists(path))
        {
            return [];
        }

        return Directory.GetDirectories(path);
    }

    public static DirectoryInfo GetOrCreateDirectory(in string path)
    {
        GameLogger.Verify(Path.IsPathRooted(path));

        if (Directory.Exists(path))
        {
            return new DirectoryInfo(path);
        }

        return Directory.CreateDirectory(path);
    }

    public static bool DeleteDirectoryIfExists(in string path)
    {
        GameLogger.Verify(Path.IsPathRooted(path));

        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Delete all directories in <paramref name="fullpath"/>.
    /// </summary>
    /// <param name="fullpath">Complete rooted path.</param>
    /// <param name="deleteRootFiles">Whether we should also delete the files in the root path.</param>
    public static void DeleteContent(in string fullpath, bool deleteRootFiles)
    {
        GameLogger.Verify(Path.IsPathRooted(fullpath));

        DirectoryInfo di = new(fullpath);
        if (!di.Exists)
            return;

        if (deleteRootFiles)
        {
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    /// <summary>
    /// This is required since some systems may do a case sensitive search (and we don´t want that).
    /// This will be overriden by systems that would like to take this into account (e.g. editor manager).
    /// </summary>
    protected virtual bool FileExistsWithCaseInsensitive(in string path)
    {
        return File.Exists(path);
    }

    public bool DeleteFileIfExists(in string path)
    {
        GameLogger.Verify(Path.IsPathRooted(path));

        if (FileExistsWithCaseInsensitive(path))
        {
            File.Delete(path);
            return true;
        }

        return false;
    }

    public bool Exists(in string path)
    {
        GameLogger.Verify(Path.IsPathRooted(path));
        return FileExistsWithCaseInsensitive(path);
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
            _ = GetOrCreateDirectory(directoryPath);
        }
    }
}