using InstallWizard.Core.Graphics;
using InstallWizard.Util.Attributes;
using Editor.Gui;
using Editor.Reflection;
using ImGuiNET;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableDictionary<string, Animation>))]
    internal class DictionaryStringAnimationField : DictionaryField<string, Animation>
    {
        string _newKey = string.Empty;

        protected override bool Add(IList<string> candidates, [NotNullWhen(true)] out (string Key, Animation Value)? element)
        {
            if(ImGui.Button("Add Animation"))
            {
                _newKey = "New Animation";
                ImGui.OpenPopup("Add Animation");
            }

            if (ImGui.BeginPopup("Add Animation"))
            {
                ImGui.InputText("##AddAnim_new_name", ref _newKey, 128);
                if (ImGui.Button("Create"))
                {
                    ImGui.CloseCurrentPopup();
                    element = (_newKey, default);

                    ImGui.EndPopup();
                    return true;
                }

                ImGui.EndPopup();
            }

            element = (string.Empty, default);
            return false;
        }

        protected override List<string> GetCandidateKeys(EditorMember member, IDictionary<string, Animation> fieldValue)
        {
            return new List<string>(new string[] { "" });
        }
    }
}
