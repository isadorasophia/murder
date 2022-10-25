using System.Reflection;

namespace Generator
{
    /// <summary>
    /// This will generate code for fast retrieval of components within the entities.
    /// </summary>
    internal class Entrypoint
    {
        /// <param name="args">
        /// This expects the following arguments:
        ///   `.\Entrypoint <-buildWithBinaries|-buildWithIntermediate> <targetSource> <binariesPath> <namespace>`
        ///     <targetSource> is the source path to the target project, relative to the executable or absolute.
        ///     <binariesPath> is the path to the output directory, relative to the executable or absolute.
        ///     <namespace> is the namespace of the target.
        /// .</param>
        /// <exception cref="ArgumentException">Whenever the arguments mismatch the documentation.</exception>
        internal static async Task Main(string[] args)
        {
            List<string> arguments = new();
            foreach (string arg in args)
            {
                arguments.AddRange(arg.Split(' '));
            }

            if (arguments.Count != 4)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("This expects the following arguments:\n" +
                    "\t.\\Entrypoint <-buildWithBinaries|-buildWithIntermediate> <targetSource> <binariesPath> <namespace>\n\n" +
                    "\t  - <targetSource> is the source path to the target project, relative to the executable or absolute.\n" +
                    "\t  - <binariesPath> is the path to the output directory, relative to the executable or absolute.\n" +
                    "\t  - <namespace> is the namespace of the target.");
                Console.ResetColor();

                throw new ArgumentException(nameof(args));
            }

            string ToRootPath(string s) => 
                Path.IsPathRooted(s) ? s : Path.GetFullPath(Path.Join(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location), s));

            bool buildIntermediate = !string.Equals(arguments[0], "-buildWithIntermediate", StringComparison.InvariantCultureIgnoreCase);
            string projectPath = ToRootPath(arguments[1]);
            string outputPath = ToRootPath(arguments[2]);
            string targetNamespace = arguments[3];

            IEnumerable<string> allAssemblies = GetAllLibrariesInPath(outputPath);
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

            string generatedFileDirectory = Path.Combine(projectPath, "Generated");
            CreateIfNotFound(generatedFileDirectory);

            Generation g = new(targetNamespace, targetAssemblies);

            if (buildIntermediate)
            {
                await g.GenerateIntermediate(generatedFileDirectory, outputPath);
            }

            await g.Generate(generatedFileDirectory);

            Console.WriteLine($"Finished generating components for {targetNamespace}!");
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