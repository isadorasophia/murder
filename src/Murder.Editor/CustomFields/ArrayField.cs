using ImGuiNET;
using Murder.Editor.Reflection;
using Murder.ImGuiExtended;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(int[]))]
    internal class ArrayField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            IList<int> array = (IList<int>)fieldValue!;
            for (int i = 0; i < array.Count; i++)
            {
                if (ImGuiHelpers.DeleteButton($"delete_{i}"))
                {
                    List<int> newArray = new(array);
                    newArray.RemoveAt(i);

                    return (true, newArray.ToArray());
                }

                ImGui.SameLine();

                int value = array[i];
                if (ImGui.InputInt($"##field_{i}", ref value, 1))
                {
                    array[i] = value;
                    modified = true;
                }
            }

            if (ImGuiHelpers.IconButton('\uf055', "add_new"))
            {
                List<int> newArray = new(array);
                newArray.Add(default);

                return (true, newArray.ToArray());
            }

            return (modified, array);
        }
    }
}
