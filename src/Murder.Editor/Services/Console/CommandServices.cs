using Bang;
using Murder.Services.Console;
using System.Reflection;
using System.Text;

namespace Murder.Editor.Services.Console
{
    public static class CommandServices
    {
        private static readonly Lazy<Dictionary<string, Command>> _commands;
        private static readonly Lazy<string> _help;

        static CommandServices()
        {
            _commands = new(FetchAllCommands());
            _help = new(HelpCommand());
        }

        public static string Parse(World world, string input)
        {
            const string InvalidCommand = "Unable to recognize command! (Type 'help' for a list of supported commands)";

            if (string.IsNullOrEmpty(input))
            {
                return InvalidCommand;
            }

            string[] tokens = input.Split(' ');
            string name = tokens[0];

            switch (name)
            {
                case "help":
                    return _help.Value;

                default:
                    if (_commands.Value.TryGetValue(name, out Command command))
                    {
                        return ExecuteCommand(world, command, tokens[1..]);
                    }
                    else
                    {
                        return InvalidCommand;
                    }
            }
        }

        private static string ExecuteCommand(World world, Command command, string[] args)
        {
            if (command.Arguments.Length - 1 != args.Length)
            {
                StringBuilder builder = new();
                builder.AppendLine("Unexpected arguments! See usage:");
                builder.AppendLine($"\t{UsageInstructions(command)}");

                return builder.ToString();
            }

            object[] objectArgs = new object[args.Length + 1];

            // First argument will always be 'world'
            objectArgs[0] = world;

            for (int i = 0; i < args.Length; ++i)
            {
                Type t = command.Arguments[i + 1].Type;
                try
                {
                    objectArgs[i + 1] = Convert.ChangeType(args[i], t);
                }
                catch
                {
                    return $"Unexpected type for {command.Arguments[i + 1].Name}, expected an {t.Name}.";
                }
            }

            return command.Method.Invoke(null, objectArgs) as string ?? string.Empty;
        }

        private readonly struct Command
        {
            public MethodInfo Method { get; init; }
            public (Type Type, string Name)[] Arguments { get; init; }
            public string Help { get; init; }

            public string Name => ToName(Method);
        }

        private static Dictionary<string, Command> FetchAllCommands()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            return assembly.GetTypes()
                .Where(t => string.Equals(t.Namespace, "InstallWizard.Services.Console", StringComparison.OrdinalIgnoreCase))
                .SelectMany(t => t.GetMethods())
                .Where(m => Attribute.IsDefined(m, typeof(CommandAttribute)) && !string.IsNullOrEmpty(m.Name))
                .ToDictionary(
                    m => ToName(m),
                    m => ToCommand(m),
                    StringComparer.OrdinalIgnoreCase);
        }

        private static string ToName(MethodInfo m)
        {
            return char.ToLower(m.Name[0]) + m.Name[1..];
        }

        private static Command ToCommand(MethodInfo m)
        {
            return new Command
            {
                Method = m,
                Arguments = m.GetParameters().Select(p => (p.ParameterType, p.Name ?? string.Empty)).ToArray(),
                Help = ((CommandAttribute)m.GetCustomAttribute(typeof(CommandAttribute))!).Help
            };
        }

        private static string HelpCommand()
        {
            StringBuilder builder = new();
            builder.AppendLine("Our supported commands are:");

            int c = 0;
            foreach (var (_, command) in _commands.Value)
            {
                builder.Append($"\t{UsageInstructions(command)}");

                if (c < _commands.Value.Count - 1)
                {
                    builder.Append('\n');
                }
            }

            return builder.ToString();
        }

        private static string UsageInstructions(Command c)
        {
            StringBuilder builder = new();

            StringBuilder arguments = new();
            foreach (var arg in c.Arguments[1..])
            {
                arguments.Append($" {arg.Name}");
            }

            builder.Append($"'{c.Name}{arguments}' - {c.Help}");

            return builder.ToString();
        }
    }
}
