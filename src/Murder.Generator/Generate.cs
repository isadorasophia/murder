using Bang.Components;
using Bang.Interactions;
using Bang.StateMachines;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Generator
{
    internal class Generation
    {
        private const string _template = "template.txt";

        private readonly string _targetNamespace;
        private readonly ImmutableArray<Assembly> _targetAssemblies;

        internal Generation(string targetAssembly, IEnumerable<Assembly> targetAssemblies)
        {
            _targetNamespace = targetAssembly;
            _targetAssemblies = targetAssemblies.ToImmutableArray();
        }

        internal async ValueTask Generate(string pathToOutputDirectory)
        {
            List<(int index, string name, Type type)> componentsDescriptions = 
                GetComponentsDescription(out var genericComponentsDescription, out int lastAvailableIndex);

            List<(int index, string name, Type type)> messagesDescriptions = GetMessagesDescription(lastAvailableIndex);

            string outputFilePath = Path.Combine(pathToOutputDirectory, "EntityExtensions.cs");
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _template);

            IEnumerable<Type> targetTypes =
                componentsDescriptions.Select(t => t.type)
                .Concat(genericComponentsDescription.Select(t => t.genericType))
                .Concat(genericComponentsDescription.Select(t => t.genericArgument))
                .Concat(messagesDescriptions.Select(t => t.type));

            Dictionary<string, string> parameters = new()
            {
                { "<target_assembly>", _targetNamespace },
                { "<using_namespaces>", GenerateNamespaces(targetTypes) },
                { "<components_enum>", GenerateEnums(componentsDescriptions) },
                { "<messages_enum>", GenerateEnums(messagesDescriptions) },
                { "<components_get>", GenerateComponentsGetter(componentsDescriptions) },
                { "<components_has>", GenerateComponentsHas(componentsDescriptions) },
                { "<components_tryget>", GenerateComponentsTryGet(componentsDescriptions) },
                { "<components_set>", GenerateComponentsSet(componentsDescriptions) },
                { "<components_remove>", GenerateComponentsRemove(componentsDescriptions) },
                { "<messages_has>", GenerateMessagesHas(messagesDescriptions) },
                { "<components_relative_set>", GenerateRelativeSet(componentsDescriptions) },
                { "<components_type_to_index>", GenerateTypesDictionary(componentsDescriptions, genericComponentsDescription) },
                { "<messages_type_to_index>", GenerateTypesDictionary(messagesDescriptions) }
            };

            string template = await File.ReadAllTextAsync(templatePath);
            string formatted = parameters.Aggregate(template, 
                (current, parameter) => current.Replace(parameter.Key, parameter.Value));

            await File.WriteAllTextAsync(outputFilePath, formatted);
        }

        private List<(int index, string name, Type t)> GetMessagesDescription(int startingIndex)
        {
            IEnumerable<Type> messages = ReflectionHelper.GetAllCandidateMessages(_targetAssemblies);

            int index = startingIndex;
            return messages.Select(m => (index++, Prettify(m), m)).ToList();
        }

        private List<(int index, string name, Type t)> GetComponentsDescription(
            out List<(int index, Type genericType, Type genericArgument)> generics,
            out int lastAvailableIndex)
        {
            IEnumerable<Type> components = ReflectionHelper.GetAllCandidateComponents(_targetAssemblies);

            Dictionary<Type, int> lookup = new();
            List<(int index, string name, Type t)> result = new();

            int index = 0;
            foreach (Type t in components)
            {
                lookup[t] = index++;
                result.Add((lookup[t], Prettify(t), t));
            }

            generics = new();

            Type genericStateMachineComponent = typeof(StateMachineComponent<>);

            Type stateMachineComponent = typeof(IStateMachineComponent);
            IEnumerable<Type> allStateMachines = ReflectionHelper.GetAllStateMachineComponents(_targetAssemblies);

            if (allStateMachines.Any())
            {
                if (!lookup.ContainsKey(stateMachineComponent))
                {
                    // Generic has not been added yet.
                    lookup[stateMachineComponent] = index++;
                    result.Add((lookup[stateMachineComponent], Prettify(stateMachineComponent), stateMachineComponent));
                }
            }

            foreach (Type t in allStateMachines)
            {
                generics.Add((lookup[stateMachineComponent], genericStateMachineComponent, t));
            }

            Type interactiveComponent = typeof(IInteractiveComponent);
            Type genericInteractiveComponent = typeof(InteractiveComponent<>);

            foreach (Type t in ReflectionHelper.GetAllInteractionComponents(_targetAssemblies))
            {
                if (!lookup.ContainsKey(interactiveComponent))
                {
                    // Generic has not been added yet.
                    lookup[interactiveComponent] = index++;
                    result.Add((lookup[interactiveComponent], Prettify(interactiveComponent), interactiveComponent));
                }

                generics.Add((lookup[interactiveComponent], genericInteractiveComponent, t));
            }

            lastAvailableIndex = index;
            return result;
        }
        
        private string GenerateNamespaces(IEnumerable<Type> types)
        {
            StringBuilder builder = new();

            HashSet<string> allNamespaces = new();

            foreach (Type t in types)
            {
                if (t.Namespace is string @namespace && !allNamespaces.Contains(@namespace))
                {
                    allNamespaces.Add(@namespace);
                }
            }

            foreach (string @namespace in allNamespaces)
            {
                builder.Append($"using {@namespace};\r\n");
            }

            // Trim last extra enter.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            return builder.ToString();
        }

        private string GenerateEnums(IEnumerable<(int index, string name, Type t)> descriptions)
        {
            StringBuilder builder = new();

            foreach (var (index, name, t) in descriptions)
            {
                if (builder.Length > 0)
                {
                    builder.Append("        ");
                }

                builder.Append($"{name} = {index},\r\n");
            }

            // Trim last extra comma and enter.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 3, 3);
            }

            return builder.ToString();
        }

        private string GenerateComponentsGetter(IEnumerable<(int index, string name, Type t)> descriptions)
        {
            StringBuilder builder = new();

            foreach (var (index, name, t) in descriptions)
            {
                if (builder.Length > 0)
                {
                    builder.Append("        ");
                }

                builder.AppendFormat($"internal static {t.Name} Get{name}(this Entity e)\r\n");
                builder.AppendFormat("        {{\r\n");
                builder.AppendFormat($"            return e.GetComponent<{t.Name}>({index});\r\n");
                builder.AppendFormat("        }}\r\n\r\n");
            }

            // Trim last enter.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            return builder.ToString();
        }

        private string GenerateComponentsHas(IEnumerable<(int index, string name, Type t)> descriptions)
        {
            StringBuilder builder = new();

            foreach (var (index, name, t) in descriptions)
            {
                if (name is null)
                {
                    // Skip syntax sugar if there is no name.
                    continue;
                }

                if (builder.Length > 0)
                {
                    builder.Append("        ");
                }

                builder.AppendFormat($"internal static bool Has{name}(this Entity e)\r\n");
                builder.AppendFormat("        {{\r\n");
                builder.AppendFormat($"            return e.HasComponent({index});\r\n");
                builder.AppendFormat("        }}\r\n\r\n");
            }

            // Trim last enter.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            return builder.ToString();
        }

        private string GenerateComponentsTryGet(IEnumerable<(int index, string name, Type t)> descriptions)
        {
            StringBuilder builder = new();

            foreach (var (index, name, t) in descriptions)
            {
                if (builder.Length > 0)
                {
                    builder.Append("        ");
                }

                builder.AppendFormat($"internal static {t.Name}? TryGet{name}(this Entity e)\r\n");
                builder.AppendFormat("        {{\r\n");
                builder.AppendFormat($"            if (!e.Has{name}())\r\n");
                builder.AppendFormat("            {{\r\n");
                builder.AppendFormat($"                return null;\r\n");
                builder.AppendFormat("            }}\r\n\r\n");
                builder.AppendFormat($"            return e.Get{name}();\r\n");
                builder.AppendFormat("        }}\r\n\r\n");
            }

            // Trim last enter.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            return builder.ToString();
        }
        
        private string GenerateComponentsSet(IEnumerable<(int index, string name, Type t)> descriptions)
        {
            StringBuilder builder = new();

            foreach (var (index, name, t) in descriptions)
            {
                if (builder.Length > 0)
                {
                    builder.Append("        ");
                }

                builder.AppendFormat($"internal static void Set{name}(this Entity e, {t.Name} component)\r\n");
                builder.AppendFormat("        {{\r\n");
                builder.AppendFormat($"            e.AddOrReplaceComponent(component, {index});\r\n");
                builder.AppendFormat("        }}\r\n\r\n");

                // Create fancy constructors based on the component!
                foreach (ConstructorInfo constructor in t.GetConstructors())
                {
                    ParameterInfo[] parameters = constructor.GetParameters();

                    builder.AppendFormat($"        internal static void Set{name}(this Entity e");
                    foreach (ParameterInfo p in parameters)
                    {
                        string parameterName = p.ParameterType.IsGenericType ?
                            FormatGenericName(p.ParameterType, p.ParameterType.GetGenericArguments()) :
                            FormatNonGenericTypeName(p.ParameterType);

                        builder.Append($", {parameterName} {p.Name}");
                    }

                    builder.AppendFormat(")\r\n");
                    builder.AppendFormat("        {{\r\n");
                    builder.AppendFormat($"            e.AddOrReplaceComponent(new {t.Name}(");
                    for (int c = 0; c < parameters.Length; c++)
                    {
                        builder.Append($"{parameters[c].Name}");

                        if (c != parameters.Length - 1)
                        {
                            builder.Append($", ");
                        }
                    }

                    builder.AppendFormat($"), {index});\r\n");
                    builder.AppendFormat("        }}\r\n\r\n");
                }
            }

            // Trim last enter.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            return builder.ToString();
        }

        private string GenerateComponentsRemove(IEnumerable<(int index, string name, Type t)> descriptions)
        {
            StringBuilder builder = new();

            foreach (var (index, name, t) in descriptions)
            {
                if (builder.Length > 0)
                {
                    builder.Append("        ");
                }

                builder.AppendFormat($"internal static bool Remove{name}(this Entity e)\r\n");
                builder.AppendFormat("        {{\r\n");
                builder.AppendFormat($"            return e.RemoveComponent({index});\r\n");
                builder.AppendFormat("        }}\r\n\r\n");
            }

            // Trim last enter.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            return builder.ToString();
        }

        private string GenerateMessagesHas(IEnumerable<(int index, string name, Type t)> descriptions)
        {
            StringBuilder builder = new();

            foreach (var (index, name, t) in descriptions)
            {
                if (name is null)
                {
                    // Skip syntax sugar if there is no name.
                    continue;
                }

                if (builder.Length > 0)
                {
                    builder.Append("        ");
                }

                builder.AppendFormat($"internal static bool Has{name}Message(this Entity e)\r\n");
                builder.AppendFormat("        {{\r\n");
                builder.AppendFormat($"            return e.HasMessage({index});\r\n");
                builder.AppendFormat("        }}\r\n\r\n");
            }

            // Trim last enter.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            return builder.ToString();
        }

        private string GenerateRelativeSet(IEnumerable<(int index, string name, Type t)> descriptions)
        {
            StringBuilder builder = new();

            foreach (var (index, _, t) in descriptions)
            {
                if (!typeof(IParentRelativeComponent).IsAssignableFrom(t))
                {
                    // Not a relative component.
                    continue;
                }

                if (builder.Length > 0)
                {
                    builder.Append("            ");
                }

                builder.AppendFormat($"{index},\r\n");
            }

            // Trim last comma+enter.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 3, 3);
            }

            return builder.ToString();
        }

        private string GenerateTypesDictionary(
            IEnumerable<(int index, string name, Type t)> descriptions,
            IEnumerable<(int index, Type genericType, Type genericArgument)>? generics = default)
        {
            StringBuilder builder = new();

            foreach (var (index, _, t) in descriptions)
            {
                if (builder.Length > 0)
                {
                    builder.Append("            ");
                }

                builder.Append("{ ");
                builder.AppendFormat($"typeof({t.Name}), {index}");
                builder.Append(" },\r\n");
            }

            if (generics is not null)
            {
                foreach (var (index, genericType, genericArgument) in generics)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append("            ");
                    }

                    builder.Append("{ ");
                    builder.AppendFormat("typeof({0}<{1}>), {2}",
                        genericType.Name.Substring(0, genericType.Name.LastIndexOf("`", StringComparison.InvariantCulture)),
                        genericArgument.Name, 
                        index);
                    builder.Append(" },\r\n");
                }
            }

            // Trim last comma+enter.
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 3, 3);
            }

            return builder.ToString();
        }

        private string FormatNonGenericTypeName(Type type)
        {
            StringBuilder builder = new();

            string name = type.FullName!;
            if (name.Contains("&"))
            {
                builder.Append($"in {name.Substring(0, name.LastIndexOf("&", StringComparison.InvariantCulture))}");
            }
            else
            {
                builder.Append(name);
            }

            return builder.ToString();
        }

        private string FormatGenericName(Type genericType, params Type[] genericArguments)
        {
            StringBuilder builder = new();

            builder.AppendFormat("{0}<",
                genericType.FullName!.Substring(0, genericType.FullName.LastIndexOf("`", StringComparison.InvariantCulture)));
            for (int a = 0; a < genericArguments.Length; a++)
            {
                builder.AppendFormat($"{genericArguments[a].FullName}");

                if (a != genericArguments.Length - 1)
                {
                    builder.Append(", ");
                }
            }

            builder.Append(">");

            return builder.ToString();
        }

        /// <summary>
        /// Prettify the name of <paramref name="t"/>.
        /// </summary>
        private string Prettify(Type t)
        {
            StringBuilder builder = new(t.Name);
            if (t.IsInterface && builder[0] == 'I')
            {
                // If this is an interface, skip the "I" character.
                builder.Remove(0, 1);
            }

            string name = builder.ToString();

            // Remove "Component" of the name.
            Regex re = new Regex(@"(.*)(?=Component|Message)");
            Match m = re.Match(name);
            if (m.Success)
            {
                name = m.Groups[0].Value;
            }

            return name;
        }
    }
}