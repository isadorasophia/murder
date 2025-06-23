using Bang;
using Bang.Entities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Murder.Diagnostics
{
    public static class CommandServices
    {
        public static ImmutableDictionary<string, Command> AllCommands => _commands.Value;
        private static readonly Lazy<ImmutableDictionary<string, Command>> _commands;
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
                    string arg = args[i];

                    if (t == typeof(float))
                        objectArgs[i + 1] = Convert.ChangeType(arg.Trim('f'), t);
                    else
                        objectArgs[i + 1] = Convert.ChangeType(arg, t);
                }
                catch
                {
                    return $"Unexpected type for {command.Arguments[i + 1].Name}, expected an {t.Name}.";
                }
            }

            return command.Method.Invoke(null, objectArgs) as string ?? string.Empty;
        }

        public readonly struct Command
        {
            public MethodInfo Method { get; init; }
            public (Type Type, string Name)[] Arguments { get; init; }
            public string Help { get; init; }

            [UnconditionalSuppressMessage("Trimming", "IL2026:Method name might have been trimmed.", Justification = "Assembly is not trimmed.")]
            public string Name => ToName(Method);
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:GetTypes() might not get all the types if they are trimmed.", Justification = "Assembly is not trimmed.")]
        [UnconditionalSuppressMessage("AOT", "IL2075:Calling non-public fields with reflection.", Justification = "Assembly is not trimmed.")]
        private static ImmutableDictionary<string, Command> FetchAllCommands()
        {
            var builder = ImmutableDictionary.CreateBuilder<string, Command>();

            Type tCommand = typeof(ICommands);

            // Find all implementations of ICommand and use them.
            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly s in allAssemblies)
            {
                try
                {
                    foreach (Type t in s.GetTypes())
                    {
                        if (!t.IsAssignableTo(tCommand))
                        {
                            continue;
                        }

                        builder.AddRange(t
                            .GetMethods()
                            .Where(m => Attribute.IsDefined(m, typeof(CommandAttribute)) && !string.IsNullOrEmpty(m.Name))
                            .ToImmutableDictionary(
                                ToName,
                                ToCommand,
                                StringComparer.OrdinalIgnoreCase
                            ));
                    }
                }
                catch { } // ignore type errors
            }

            return builder.ToImmutable();
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