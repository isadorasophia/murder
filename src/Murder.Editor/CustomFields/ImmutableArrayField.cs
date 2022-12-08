using ImGuiNET;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    public abstract class ImmutableArrayField<T> : CustomField
    {
        protected abstract bool Add(in EditorMember member, [NotNullWhen(true)] out T? element);

        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            ImmutableArray<T> elements = (ImmutableArray<T>)fieldValue!;
            if (elements.IsDefault) elements = ImmutableArray<T>.Empty;

            ImGui.PushID($"Add ${member.Member.ReflectedType}");

            if (Add(member, out T? element))
            {
                elements = elements.Add(element);
                modified = true;
            }

            ImGui.PopID();

            if (modified || elements.Length == 0)
            {
                return (modified, elements);
            }

            using (new RectangleBox())
            {
                for (int index = 0; index < elements.Length; index++)
                {
                    ImGui.PushID($"{member.Member.ReflectedType}_{index}");
                    element = elements[index];

                    if (ImGuiHelpers.DeleteButton($"delete_{index}"))
                    {
                        ImGui.PopID();

                        return (true, elements.Remove(element));
                    }

                    ImGui.SameLine();

                    if (DrawElement(ref element, member, index))
                    {
                        elements = elements.SetItem(index, element!);
                        modified = true;
                    }

                    ImGui.PopID();
                }
            }
            return (modified, elements);
        }

        protected virtual bool DrawElement(ref T? element, EditorMember member, int index)
        {
            object? boxed = element;
            if (CustomComponent.ShowEditorOf(boxed))
            {
                element = (T?)boxed;
                return true;
            }

            return false;
        }
    }
}
