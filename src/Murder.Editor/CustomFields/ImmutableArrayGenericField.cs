using ImGuiNET;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<>), priority: -1)]
    public class ImmutableArrayGenericField<T> : ImmutableArrayField<T> where T : new()
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out T? element)
        {
            element = default!;

            if (ImGui.Button("Add item"))
            {
                element = CreateNewInstance()!;
                return true;
            }

            return false;
        }

        private T CreateNewInstance()
        {
            Type t = typeof(T);
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ImmutableArray<>))
            {
                MethodInfo? create = typeof(ImmutableArray).GetMethod("Create", BindingFlags.Public | BindingFlags.Static, [])?
                    .MakeGenericMethod(t.GenericTypeArguments[0]);

                return (T)create?.Invoke(null, null)!;
            }
            else
            {
                return new();
            }
        }
    }
}