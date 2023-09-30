using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Murder.Diagnostics;
using Murder.Assets;
using Murder.Utilities;
using System.Security.Cryptography;
using System.Text;

namespace Murder.Serialization
{
    /// <summary>
    /// FileHelper which will do OS operations. This is system-agnostic.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Gets the rooted path from a relative one
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string GetPath(params string[] paths)
        {
            var path = Path.Join(paths);

            if (Path.IsPathRooted(path))
            {
                // Already rooted, so yay?
                return path;
            }

            return Path.GetFullPath(Path.Join(Path.GetDirectoryName(AppContext.BaseDirectory), path));
        }

        /// <summary>
        /// Computes a file Hash to watch it for changes
        /// </summary>
        public static string ComputeHash(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                var sha = System.Security.Cryptography.SHA256.Create();
                var hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }

        internal readonly static JsonSerializerSettings _settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
            ContractResolver = new WritablePropertiesOnlyResolver(),
            MissingMemberHandling = MissingMemberHandling.Error,
            Error = SerializationHelper.HandleSerializationError,
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// Settings when serializing compressed json files.
        /// </summary>
        private readonly static JsonSerializerSettings _compressedSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new WritablePropertiesOnlyResolver(),
            MissingMemberHandling = MissingMemberHandling.Error,
            Error = SerializationHelper.HandleSerializationError,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string EscapePath(this string path)
        {
            return path
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);
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

        public static void SaveTextFromRelativePath(in string relativePath, in string content) =>
            SaveText(GetPath(relativePath), content);

        public static void SaveText(in string fullpath, in string content)
        {
            GameLogger.Verify(Path.IsPathRooted(fullpath));

            if (!FileExists(fullpath))
            {
                string directoryName = Path.GetDirectoryName(fullpath)!;
                _ = GetOrCreateDirectory(directoryName);
            }

            File.WriteAllText(fullpath, content);
        }

        public static async Task SaveTextAsync(string fullpath, string content)
        {
            GameLogger.Verify(Path.IsPathRooted(fullpath));

            if (!FileExists(fullpath))
            {
                string directoryName = Path.GetDirectoryName(fullpath)!;

                if (!string.IsNullOrWhiteSpace(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }

            await File.WriteAllTextAsync(fullpath, content);
        }

        public static string SaveSerializedFromRelativePath<T>(T value, in string relativePath) =>
            SaveSerialized(value, GetPath(relativePath));

        public static string SaveSerialized<T>(T value, string path, bool isCompressed = false)
        {
            GameLogger.Verify(value != null, $"Cannot serialize a null {typeof(T).Name}");

            string json = JsonConvert.SerializeObject(value, isCompressed ? _compressedSettings : _settings);
            SaveText(path, json);

            return json;
        }

        public static async ValueTask<string> SaveSerializedAsync<T>(T value, string path, bool isCompressed = false)
        {
            GameLogger.Verify(value != null, $"Cannot serialize a null {typeof(T).Name}");

            string json = JsonConvert.SerializeObject(value, isCompressed ? _compressedSettings : _settings);
            await SaveTextAsync(path, json);

            return json;
        }

        public static T? DeserializeGeneric<T>(string path, bool warnOnErrors = true)
        {
            GameLogger.Verify(Path.IsPathRooted(path));

            if (!FileExists(path))
            {
                if (warnOnErrors)
                {
                    GameLogger.Warning($"Can't find {path} to deserialize");
                }

                return default;
            }

            string json = File.ReadAllText(path);
            T? asset = JsonConvert.DeserializeObject<T>(json, _settings);
            return asset;
        }

        internal static dynamic GetJson(string path)
        {
            GameLogger.Verify(Path.IsPathRooted(path));
            if (!FileExists(path))
            {
                GameLogger.Warning($"Can't find {path} to deserialize");

                throw new InvalidOperationException("Unable to deserialize specified path.");
            }

            var txt = File.ReadAllText(path);
            dynamic jsonDoc = JsonConvert.DeserializeObject(txt)!;

            return jsonDoc;
        }

        public static T? DeserializeAsset<T>(string path) where T : GameAsset
        {
            GameLogger.Verify(Path.IsPathRooted(path));

            if (!FileExists(path))
            {
                GameLogger.Warning($"Can't find {path} to deserialize");
                return null;
            }

            var json = File.ReadAllText(path);

            try
            {
                T? asset = JsonConvert.DeserializeObject<T>(value: json, settings: _settings);
                asset?.AfterDeserialized();

                if (asset != null && asset.Guid == Guid.Empty)
                {
                    asset.MakeGuid();
                }
                
                return asset;
            }
            catch (JsonSerializationException)
            {
                return null;
            }
        }

        public static IEnumerable<string> ListAllDirectories(string path)
        {
            GameLogger.Verify(Path.IsPathRooted(path));

            if (!Directory.Exists(path))
            {
                return Enumerable.Empty<string>();
            }
            
            return Directory.GetDirectories(path);
        }

        public static IEnumerable<FileInfo> GetAllFilesInFolder(string path, string filter, bool recursive)
        {
            GameLogger.Verify(Path.IsPathRooted(path));

            if (!Directory.Exists(path))
            {
                return Enumerable.Empty<FileInfo>();
            }

            DirectoryInfo dir = new(path);
            return dir.EnumerateFiles(filter, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<FileInfo> GetAllFilesInFolder(string path, bool recursive, params string[] filters)
        {
            List<FileInfo> result = new();
            foreach (string filter in filters)
            {
                result.AddRange(GetAllFilesInFolder(path, filter, recursive));
            }

            return result;
        }

        public static void OpenFolder(string path)
        {
            GameLogger.Verify(Path.IsPathRooted(path));

            if (Directory.Exists(path))
            {
                ProcessStartInfo startInfo;

                if (OperatingSystem.IsWindows())
                {
                    startInfo = new ProcessStartInfo
                    {
                        Arguments = path,
                        FileName = "explorer.exe"
                    };
                }
                else if (OperatingSystem.IsMacOS())
                {
                    startInfo = new ProcessStartInfo
                    {
                        Arguments = $"-R {path}",
                        FileName = "open"
                    };
                }
                else
                {
                    GameLogger.Error("Open a folder in Linux has not been implemented yet (I need to test it).");
                    return;
                }

                Process.Start(startInfo);
            }
            else
            {
                GameLogger.Error(string.Format("{0} Directory does not exist!", path));
            }
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

        /// This is required since some systems may do a case sensitive search (and we don´t want that)
        public static bool FileExists(in string path)
        {
            var directory = Path.GetDirectoryName(path) ?? string.Empty;
            var file = Path.GetFileName(path);

            if (!Directory.Exists(directory))
                return false;

            var files = Directory.GetFiles(directory, file, new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });

            return files.Length > 0;
        }

        public static bool DeleteFileIfExists(in string path)
        {
            GameLogger.Verify(Path.IsPathRooted(path));

            if (FileExists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }

        public static bool ExistsFromRelativePath(in string relativePath) => Exists(GetPath(relativePath));

        public static bool Exists(in string path)
        {
            GameLogger.Verify(Path.IsPathRooted(path));
            return FileExists(path);
        }

        /// <summary>
        /// Copies all files from <paramref name="sourceDirectoryPath"/> to <paramref name="destDirectoryPath"/>.
        /// Do not delete existing files.
        /// </summary>
        /// <param name="sourceDirectoryPath">Full path to the source.</param>
        /// <param name="destDirectoryPath">Full path to the destination.</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static int DirectoryDeepCopy(
            string sourceDirectoryPath,
            string destDirectoryPath)
        {
            GameLogger.Verify(Path.IsPathRooted(sourceDirectoryPath) && Path.IsPathRooted(destDirectoryPath));
            
            // If the source directory does not exist, throw an exception.
            if (!Directory.Exists(sourceDirectoryPath))
            {
                throw new DirectoryNotFoundException(
                    $"Source directory does not exist or could not be found: {sourceDirectoryPath}");
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirectoryPath))
            {
                Directory.CreateDirectory(destDirectoryPath);
            }
            
            // Create all of the directories
            foreach (string directoryPath in Directory.GetDirectories(sourceDirectoryPath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(directoryPath.Replace(sourceDirectoryPath, destDirectoryPath));
            }

            int totalOfFiles = 0;
            
            // Copy all the files.
            foreach (string newPath in Directory.GetFiles(sourceDirectoryPath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourceDirectoryPath, destDirectoryPath), true);
                totalOfFiles++;
            }

            return totalOfFiles;
        }

        /// <summary>
        /// This will iterate recursively over all files in <paramref name="path"/> and return
        /// the latest modified date.
        /// </summary>
        public static DateTime? TryGetLastWrite(string path)
        {
            if (!Directory.Exists(path))
            {
                return default;
            }

            DirectoryInfo dir = new DirectoryInfo(path);
            DateTime last = dir.LastWriteTime;

            foreach (FileInfo file in dir.EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                if (file.LastWriteTime > last)
                {
                    last = file.LastWriteTime;
                }
            }

            return last;
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
        /// Remove extension from a string.
        /// </summary>
        public static string RemoveExtension(string filePath)
        {
            return filePath[..filePath.IndexOf('.')];
        }

        /// <summary>
        /// Used to normalize file paths from different OS into the same output.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static object Normalize(string source)
        {
            return source.ToLowerInvariant().Replace('\\', '/');
        }

        public static bool IsPathInsideOf(string path, string[] filterFolders)
        {
            // Normalize the folder path to remove any inconsistencies
            string normalizedFolder = Path.GetFullPath(path);

            foreach (string filterFolder in filterFolders)
            {
                // Normalize the filter folder path
                string normalizedFilterFolder = Path.GetFullPath(filterFolder);

                // Check if the folder starts with the filter folder path
                if (normalizedFolder.StartsWith(normalizedFilterFolder, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static Guid GuidFromName(string name)
        {
            using var md5 = MD5.Create();
            Guid guid = new Guid(md5.ComputeHash(Encoding.Default.GetBytes(name)));
            return guid;
        }
    }
}
