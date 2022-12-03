using ImGuiNET;
using Murder.Core.Dialogs;
using Murder.Editor.Reflection;
using Murder.ImGuiExtended;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableDictionary<,>), priority: -1)]
    internal class DictionaryGenericField<T, V> : DictionaryField<T, V> where T : notnull
    {
        private (T Key, V Value) _new = default!;

        protected override bool Add(IList<T> candidates, [NotNullWhen(true)] out (T Key, V Value)? element)
        {
            if (ImGui.Button("Add Item"))
            {
                _new = default!;
                ImGui.OpenPopup("Add Item##dictionary");
            }

            if (ImGui.BeginPopup("Add Item##dictionary"))
            {
                DrawValue(ref _new, "Item1");
                
                if (_new.Key == null)
                {
                    ImGuiHelpers.SelectedButton("Create");
                }
                else if (ImGui.Button("Create"))
                {
                    ImGui.CloseCurrentPopup();
                    element = (_new.Key, default!);

                    ImGui.EndPopup();
                    return true;
                }

                ImGui.EndPopup();
            }

            element = (default!, default!);
            return false;
        }

        protected override List<T> GetCandidateKeys(EditorMember member, IDictionary<T, V> fieldValue) =>
            new() { default! };

        protected override bool CanModifyKeys() => true;
    }
}
