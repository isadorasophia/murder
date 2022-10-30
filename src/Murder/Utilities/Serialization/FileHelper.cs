using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Murder.Diagnostics;
using Murder.Assets;

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

            GameLogger.Verify(!Path.IsPathRooted(path));

            return Path.GetFullPath(Path.Join(Path.GetDirectoryName(AppContext.BaseDirectory), path));
        }

        internal readonly static JsonSerializerSettings _settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
            ContractResolver = new WritablePropertiesOnlyResolver(),
            MissingMemberHandling = MissingMemberHandling.Error
        };

        /// <summary>
        /// Settings when serializing compressed json files.
        /// </summary>
        private readonly static JsonSerializerSettings _compressedSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            ContractResolver = new WritablePropertiesOnlyResolver(),
            MissingMemberHandling = MissingMemberHandling.Error,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string EscapePath(this string path)
        {
            return path
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);
        }

        internal static ReadOnlySpan<char> Clean(string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9\\/\\\\_ -]");
            return rgx.Replace(str, "");
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
                if (!string.IsNullOrWhiteSpace(directoryName))
                    Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(fullpath, content);
            //GameDebugger.Log($"Saved file {relativePath}");
        }

        public static string SaveSerializedFromRelativePath<T>(T value, in string relativePath) =>
            SaveSerialized(value, GetPath(relativePath));

        public static string SaveSerialized<T>(T value, string path, bool isCompressed = false)
        {
            GameLogger.Verify(value != null, $"Cannot serialize a null {typeof(T).Name}");

            var json = JsonConvert.SerializeObject(value, isCompressed ? _compressedSettings : _settings);
            SaveText(path, json);

            return json;
        }

        public static T? DeserializeGeneric<T>(string path)
        {
            GameLogger.Verify(Path.IsPathRooted(path));

            if (!FileExists(path))
            {
                GameLogger.Warning($"Can't find {path} to deserialize");
                return default;
            }

            var json = File.ReadAllText(path);
            var asset = JsonConvert.DeserializeObject<T>(json, _settings);
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

            DirectoryInfo dir = GetOrCreateDirectory(path);
            return dir.EnumerateDirectories().Select(d => d.FullName);
        }

        public static IEnumerable<FileInfo> GetAllFilesInFolder(string path, string filter, bool recursive)
        {
            GameLogger.Verify(Path.IsPathRooted(path));

            DirectoryInfo dir = GetOrCreateDirectory(path);
            var files = dir.EnumerateFiles(filter, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            return files;
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
                // TODO: This won't work in Linux.
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = path,
                    FileName = "explorer.exe"
                };

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

        public static int DirectoryCopy(
            string sourceDirPath,
            string destDirPath,
            bool copySubDirs)
        {
            GameLogger.Verify(Path.IsPathRooted(sourceDirPath) && Path.IsPathRooted(destDirPath));

            DirectoryInfo dir = new DirectoryInfo(sourceDirPath);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirPath);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirPath))
            {
                Directory.CreateDirectory(destDirPath);
            }

            // Get the file contents of the directory to copy.
            var files = dir.EnumerateFiles("*.*", SearchOption.AllDirectories).ToArray();
            int counter = 0;
            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirPath, file.Name);

                // Copy the file.
                file.CopyTo(temppath, true);
                counter++;
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirPath, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }

            return counter;
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
    }
}
