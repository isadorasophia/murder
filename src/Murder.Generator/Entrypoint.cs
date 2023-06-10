using System.Reflection;
using System.Text;

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
            List<string> rawArguments = new();
            foreach (string arg in args)
            {
                rawArguments.AddRange(arg.Split(' '));
            }

            if (!TryParseArguments(rawArguments, out string[] arguments))
            {
                Warning("This expects the following arguments:\n" +
                    "\t.\\Entrypoint <-buildWithBinaries|-buildWithIntermediate> <targetSource> <binariesPath> <namespace>\n\n" +
                    "\t  - <targetSource> is the source path to the target project, relative to the executable or absolute.\n" +
                    "\t  - <binariesPath> is the path to the output directory, relative to the executable or absolute.\n" +
                    "\t  - <namespace> is the namespace of the target.");

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

        private static bool TryParseArguments(List<string> arguments, out string[] result)
        {
            result = new string[4];

            if (arguments.Count < 4)
            {
                Warning("Expected at least 4 arguments, see documentation for Generator.");

                // Invalid count of arguments.
                return false;
            }

            int argIndex = 0;

            // <-buildWithBinaries|-buildWithIntermediate>
            if (arguments[argIndex][0] != '-')
            {
                Warning("Expected switch for <-buildWithBinaries|-buildWithIntermediate> argument.");
                return false;
            }

            result[0] = arguments[argIndex];

            if (GetArgumentWithMaybeSpaces(arguments, startIndex: 1, out argIndex) is not string targetSource)
            {
                return false;
            }

            result[1] = targetSource;

            if (GetArgumentWithMaybeSpaces(arguments, startIndex: argIndex, out argIndex) is not string outputPath)
            {
                return false;
            }

            result[2] = outputPath;
            result[3] = arguments[argIndex];

            return true;
        }

        private static string? GetArgumentWithMaybeSpaces(List<string> arguments, int startIndex, out int lastIndex)
        {
            char firstSeparator = arguments[startIndex][0];
            if (firstSeparator != '\'' && firstSeparator != '"')
            {
                lastIndex = startIndex + 1;
                return arguments[startIndex];
            }

            StringBuilder result = new();

            int index = startIndex;
            while (index < arguments.Count)
            {
                int length = arguments[index].Length;

                bool isFirst = index == startIndex;
                bool isLast = arguments[index][length - 1] == firstSeparator;

                if (isLast) length--;
                if (isFirst) length--;

                result.Append(arguments[index], isFirst ? 1 : 0, length);
                if (isLast)
                {
                    lastIndex = index + 1;
                    return result.ToString();
                }

                result.Append(' ');
                index++;
            }

            Warning("Expected matching ' or \" char for argument.");

            lastIndex = -1;
            return null;
        }

        private static void Warning(in string warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(warning);
            Console.ResetColor();
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