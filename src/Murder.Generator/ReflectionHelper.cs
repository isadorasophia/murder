using Bang.Components;
using Bang.Interactions;
using Bang.StateMachines;
using System.Reflection;

namespace Generator
{
    internal class ReflectionHelper
    {
        private static readonly Type _componentType = typeof(IComponent);
        private static readonly Type _messageType = typeof(IMessage);
        
        private static readonly Type[] _interfaceComponents = new Type[] { typeof(ITransformComponent) };

        public static IEnumerable<Type> GetAllCandidateComponents(IEnumerable<Assembly> targetAssemblies)
        {
            // Order by name to guarantee consistency across different runs.
            return targetAssemblies.SelectMany(s => s.GetTypes())
                .Where(t => IsCandidateComponent(t))
                .OrderBy(t => t.Name);
        }

        public static IEnumerable<Type> GetAllStateMachineComponents(IEnumerable<Assembly> targetAssemblies)
        {
            // Order by name to guarantee consistency across different runs.
            return targetAssemblies.SelectMany(s => s.GetTypes())
                .Where(t => typeof(StateMachine).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface && !t.IsGenericType)
                .OrderBy(t => t.Name);
        }

        public static IEnumerable<Type> GetAllInteractionComponents(IEnumerable<Assembly> targetAssemblies)
        {
            // Order by name to guarantee consistency across different runs.
            return targetAssemblies.SelectMany(s => s.GetTypes())
                .Where(t => typeof(Interaction).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface && !t.IsGenericType)
                .OrderBy(t => t.Name);
        }

        /// <summary>
        /// This will return whatever engine specific transform interface is available within the engine.
        /// </summary>
        internal static Type FindTransformInterfaceComponent(IEnumerable<Assembly> targetAssemblies)
        {
            // Order by name to guarantee consistency across different runs.
            return targetAssemblies.SelectMany(s => s.GetTypes())
                .Where(t => typeof(ITransformComponent).IsAssignableFrom(t) && t.IsInterface && t != typeof(ITransformComponent))
                .OrderBy(t => t.Name)
                .First();
        }

        public static IEnumerable<Type> GetAllTransformComponents(IEnumerable<Assembly> targetAssemblies)
        {
            // Order by name to guarantee consistency across different runs.
            return targetAssemblies.SelectMany(s => s.GetTypes())
                .Where(t => typeof(ITransformComponent).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface && !t.IsGenericType)
                .OrderBy(t => t.Name);
        }

        /// <summary>
        /// Returns whether <paramref name="t"/> is a valid component for the generated code.
        /// </summary>
        private static bool IsCandidateComponent(Type t)
        {
            // Interfaces, abstract or generics are filtered out.
            // We also filter out components that inherit from a transform.
            if (t.IsInterface || t.IsAbstract || t.IsGenericType || _interfaceComponents.Any(tt => tt.IsAssignableFrom(t)))
            {
                return false;
            }

            return _componentType.IsAssignableFrom(t);
        }

        public static IEnumerable<Type> GetAllCandidateMessages(IEnumerable<Assembly> targetAssemblies)
        {
            IEnumerable<Type> messages = targetAssemblies.SelectMany(s => s.GetTypes())
                .Where(t => IsCandidateMessage(t));

            // Order by name to guarantee consistency across different runs.
            return messages.OrderBy(t => t.Name);
        }


        /// <summary>
        /// Returns whether <paramref name="t"/> is a valid component for the generated code.
        /// </summary>
        private static bool IsCandidateMessage(Type t)
        {
            // Interfaces, abstract or generics are filtered out.
            if (t.IsInterface || t.IsAbstract || t.IsGenericType)
            {
                return false;
            }

            return _messageType.IsAssignableFrom(t);
        }

        /// <summary>
        /// Get the modifier for <paramref name="t"/> when generating source code.
        /// </summary>
        public static string GetAccessModifier(Type t)
        {
            return t.IsPublic ? "public" : "internal";
        }
    }
}
