using Microsoft.Xna.Framework;
using Murder.Assets;
using Murder.Diagnostics;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Murder.Serialization
{
    /// <summary>
    /// FileHelper which will do OS operations. This is system-agnostic.
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// Gets the rooted path from a relative one
        /// </summary>
        /// <param name="paths">List of paths which will be joined.</param>
        /// <returns></returns>
        public static string GetPath(params string[] paths)
        {
            var path = Path.Join(paths);

            if (Path.IsPathRooted(path))
            {
                // Already rooted, so yay?
                return path;
            }

            return Path.GetFullPath(Path.Join(TitleLocation.Path, path));
        }

        public static ReadOnlySpan<char> Clean(string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9\\/\\\\_ -]");
            return rgx.Replace(str, "").EscapePath();
        }

        public static string GetPathWithoutExtension(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path));
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
        public static string GetSerializedJson<T>(T value)
        {
            GameLogger.Verify(value != null, $"Cannot serialize a null {typeof(T).Name}");

            return JsonSerializer.Serialize(value, Game.Data.SerializationOptions);
        }
        
        [UnconditionalSuppressMessage("Trimming", "IL2026:Required members might get lost when trimming.", Justification = "Assembly is trimmed.")]
        [UnconditionalSuppressMessage("AOT", "IL3050:JsonSerializer.Serialize with reflection may cause issues with trimmed assembly.", Justification = "We use source generators.")]
        public static T? GetDeserialized<T>(string json)
        {
            T? asset = JsonSerializer.Deserialize<T>(json, Game.Data.SerializationOptions);
            return asset;
        }

        public string SaveSerialized<T>(T value, string path)
        {
            GameLogger.Verify(value != null, $"Cannot serialize a null {typeof(T).Name}");

            string json = GetSerializedJson(value);
            SaveText(path, json);

            return json;
        }

        public async ValueTask<string> SaveSerializedAsync<T>(T value, string path)
        {
            GameLogger.Verify(value != null, $"Cannot serialize a null {typeof(T).Name}");

            string json = GetSerializedJson(value);
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
            return GetDeserialized<T>(json);
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

                T? asset = GetDeserialized<T>(json);
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
                T? asset = GetDeserialized<T>(json);
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

        public static IEnumerable<string> GetAllFilesInFolder(string path, string filter, bool recursive)
        {
            GameLogger.Verify(Path.IsPathRooted(path));
            if (!Directory.Exists(path))
            {
                return [];
            }

            return Directory.EnumerateFiles(path, filter, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).AsParallel();
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
        /// This is required since some systems may do a case sensitive search (and we don´t want that)
        /// </summary>
        protected virtual bool FileExistsWithCaseInsensitive(in string path)
        {
            if (File.Exists(path))
            {
                return true;
            }

            string directory = Path.GetDirectoryName(path) ?? string.Empty;
            string? file = Path.GetFileName(path);

            if (!Directory.Exists(directory))
            {
                return false;
            }

            var files = Directory.GetFiles(directory, file, new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
            return files.Length > 0;
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

        /// <summary>
        /// Returns the assembly extension for the target operating system.
        /// For example, if targeting Windows, this returns ".dll".
        /// </summary>
        /// <returns>A dot followed by the extension string.</returns>
        /// <exception cref="PlatformNotSupportedException">
        /// If the current operating has not been implemented yet.
        /// </exception>
        public static string ExtensionForOperatingSystem()
        {
            if (OperatingSystem.IsWindows())
            {
                return ".dll";
            }
            else if (OperatingSystem.IsLinux())
            {
                return ".so";
            }
            else if (OperatingSystem.IsMacOS())
            {
                return ".dylib";
            }

            throw new PlatformNotSupportedException("Consoles are not supported yet?");
        }
    }
}