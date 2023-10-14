using ImGuiNET;
using Murder.Attributes;
using Murder.Core;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<TriggerEventOn>))]
    internal class ImmutableArrayTriggerEventOnField : ImmutableArrayField<TriggerEventOn>
    {
        private const string DefaultName = "Event Name";

        string _newEvent = DefaultName;

        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out TriggerEventOn element)
        {
            element = new();

            bool modified = false;

            if (ImGui.Button("Add new trigger!"))
            {
                ImGui.OpenPopup("choose_new_trigger");
            }

            if (ImGui.BeginPopup("choose_new_trigger"))
            {
                ImGui.Text("Choose a name:");
                ImGui.SameLine();
                ImGui.InputText("##AddWorldEvent_new_name", ref _newEvent, 128);

                if (ImGui.Button("Create"))
                {
                    element = new(_newEvent);
                    _newEvent = DefaultName;

                    modified = true;
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            return modified;
        }

        protected override bool DrawElement(ref TriggerEventOn element, EditorMember member, int index)
        {
            bool modified = false;

            if (TreeEntityGroupNode($"{element.Name}##{index}", Game.Profile.Theme.Yellow, icon: '\uf06b'))
            {
                modified = CustomComponent.ShowEditorOf(ref element);
                ImGui.TreePop();
            }

            return modified;
        }

        private bool TreeEntityGroupNode(string name, System.Numerics.Vector4 textColor, char icon = '\ue1b0', ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None) =>
            ImGuiHelpers.TreeNodeWithIconAndColor(
                icon: icon,
                label: name,
                flags: ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.FramePadding | flags,
                text: textColor,
                background: Game.Profile.Theme.BgFaded,
                active: Game.Profile.Theme.Bg);

    }
}