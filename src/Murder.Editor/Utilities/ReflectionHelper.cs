using Bang.StateMachines;
using Murder.Attributes;
using Murder.Editor.Reflection;
using Murder.Utilities.Attributes;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Murder.Editor.Utilities
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Find the first abstract class from the type <see cref="T"/>.
        /// </summary>
        public static Type? TryFindFirstAbstractOf<T>() => TryFindFirstAbstractOf(typeof(T));

        public static Type? TryFindFirstAbstractOf(Type type)
        {
            if (type.BaseType is Type @base)
            {
                if (@base.IsAbstract)
                {
                    return @base;
                }

                return TryFindFirstAbstractOf(@base);
            }

            return default;
        }
        public static IEnumerable<Type> GetEnumsWithAttribute(Type attribute)
        {
            var types = SafeGetAllTypesInAllAssemblies()
                .Where(p => p.IsEnum && Attribute.IsDefined(p, attribute));

            return types;
        }

        /// <summary>
        /// Gets all types that implement  <typeparamref name="T"/>, including <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllImplementationsOf<T>()
        {
            return GetAllImplementationsOf(typeof(T));
        }

        public static IEnumerable<Type> GetAllImplementationsOf(Type type)
        {
            var types = SafeGetAllTypesInAllAssemblies()
                .Where(p => !p.IsInterface && !p.IsAbstract && type.IsAssignableFrom(p))
                .OrderBy(o => o.Name);

            return types;
        }

        public static IEnumerable<Type> GetAllTypesWithAttributeDefined<T>() =>
            GetAllTypesWithAttributeDefined(typeof(T));

        public static IEnumerable<Type> GetAllTypesWithAttributeDefined(Type t)
        {
            return SafeGetAllTypesInAllAssemblies()
                .Where(p => Attribute.IsDefined(p, t));
        }

        public static IEnumerable<Type> GetAllTypesWithAttributeDefinedOfType<T>(Type ofType)
        {
            var type = typeof(T);

            return SafeGetAllTypesInAllAssemblies()
                .Where(p => Attribute.IsDefined(p, type) && ofType.IsAssignableFrom(p));
        }

        public static Type? TryFindType(string name)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(s => s.GetType(name))
                .FirstOrDefault(t => t is not null);
        }

        /// <summary>
        /// Get visible fields in editor.
        /// This returns all fields that are either public or has the [ShowInEditor] attribute.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<EditorMember> GetFieldsForEditor(this Type type, params string[] filters)
        {
            HashSet<string> filterSet = filters.ToHashSet();

            FieldInfo[] allFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // TODO: For now, we whitelist interaction fields. We might want to change that if we do that for other types...
            // We might declare custom editors for types instead of fields.
            var targetFields = allFields
                .Where(f => f.IsPublic || Attribute.IsDefined(f, typeof(ShowInEditorAttribute)) || Attribute.IsDefined(f, typeof(JsonPropertyAttribute)) || filterSet.Contains(f.Name))
                .Select(f => EditorMember.Create(f));

            PropertyInfo[] allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // TODO: We want to whitelist all the types that we want to show its properties in the editor and bypass this filter.
            var targetProperties = allProperties
                .Where(f => Attribute.IsDefined(f, typeof(ShowInEditorAttribute)) || typeof(IStateMachineComponent).IsAssignableFrom(type) || filterSet.Contains(f.Name) ||
                (f.CanWrite && f.CanRead && (f.GetMethod is MethodInfo method && method.GetParameters().Length == 0)))
                .Select(f => EditorMember.Create(f));

            var result = targetFields
                .Concat(targetProperties)
                .Where(f => !AttributeExtensions.IsDefined(f, typeof(Newtonsoft.Json.JsonIgnoreAttribute))
                    && !AttributeExtensions.IsDefined(f, typeof(HideInEditorAttribute)));

            return result;
        }

        /// <summary>
        /// Rertieve a field of <paramref name="type"/> that has the name of <paramref name="name"/>.
        /// </summary>
        public static EditorMember? TryGetFieldForEditor(this Type type, string name)
        {
            FieldInfo[] allFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (allFields.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase)) is FieldInfo f)
            {
                return EditorMember.Create(f);
            }

            PropertyInfo[] allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (allProperties.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase)) is PropertyInfo p)
            {
                return EditorMember.Create(p);
            }

            return default;
        }

        public static string GetGenericName(Type t)
        {
            if (t.IsGenericType)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(t.Name.Substring(0, t.Name.LastIndexOf("`")));
                sb.Append(t.GetGenericArguments().Aggregate("<", (string aggregate, Type type) =>
                    {
                        return aggregate + (aggregate == "<" ? "" : ",") + GetGenericName(type);
                    }
                ));
                sb.Append(">");

                return sb.ToString();
            }

            return t.Name;
        }

        public static string? FindTooltip(Type t, string field)
        {
            return t.GetField(field, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                ?.GetCustomAttribute<TooltipAttribute>()?.Text;
        }

        private static IEnumerable<Type> SafeGetAllTypesInAllAssemblies()
        {
            // TODO: Can this me memoized? Probably.
            List<Type> result = [];

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    result.AddRange(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException)
                {
                    // Can be safely ignored.
                }
            }

            return result;
        }
    }
}