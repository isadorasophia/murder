using System.Reflection;

namespace Generator
{
    /// <summary>
    /// This will generate code for fast retrieval of components within the entities.
    /// </summary>
    internal class Entrypoint
    {
        /// <param name="args">Path to the target project, relative to the executable or absolute.</param>
        /// <exception cref="ArgumentException">Whenever the arguments mismatch the documentation.</exception>
        internal static async Task Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Unexpected arguments. Please refer to the documentation on how to use Generator.exe.");
                throw new ArgumentException(nameof(args));
            }

            string projectPath = args[0];

            if (!Path.IsPathRooted(projectPath))
            {
                projectPath = Path.GetFullPath(Path.Join(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location), projectPath));
            }

            string targetAssembliesPath = Path.Combine(projectPath, "bin");
            if (!Directory.Exists(targetAssembliesPath))
            {
                Console.WriteLine("Unable to find the binaries directory. Have you built the target project?");
                throw new InvalidOperationException();
            }

            IEnumerable<string> allAssemblies = GetAllLibrariesInPath(targetAssembliesPath);
            if (!allAssemblies.Any())
            {
                Console.WriteLine("Unable to find the any binaries. Have you built the target project?");
                throw new InvalidOperationException();
            }

            List<Assembly> targetAssemblies = new();
            foreach (string assembly in allAssemblies)
            {
                try
                {
                    targetAssemblies.Add(Assembly.LoadFrom(assembly));
                }
                catch (Exception e) when (e is FileLoadException || e is BadImageFormatException)
                {
                    // Ignore invalid (or native) assemblies.
                }
            }

            string outputDirectory = Path.Combine(projectPath, "Generated");
            CreateIfNotFound(outputDirectory);

            Generation g = new(targetAssemblies);
            await g.Generate(outputDirectory);

            Console.WriteLine("Finished generating components!");
        }

        /// <summary>
        /// Look recursively for all the files in <paramref name="path"/>.
        /// </summary>
        /// <param name="path">Rooted path to the binaries folder. This must be a valid directory.</param>
        private static IEnumerable<string> GetAllLibrariesInPath(in string path)
        {
            // TODO: Include linux.
            string[] targetExtensions = new string[] { "dll" };

            // 1. Filter all files that has an extension.
            // 2. Filter the target extensions.
            // 3. Distinguish the file names.
            return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => targetExtensions.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()))
                .GroupBy(s => Path.GetFileName(s))
                .Select(s => s.First());
        }

        /// <summary>
        /// Create a directory at <paramref name="path"/> if none is found.
        /// </summary>

        private static void CreateIfNotFound(in string path)
        {
            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }
        }
    }
}