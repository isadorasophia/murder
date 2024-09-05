using ImGuiNET;
using Murder.Attributes;
using Murder.Components;
using Murder.Editor.Attributes;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;

namespace Murder.Editor.CustomComponents
{
    [CustomComponentOf(typeof(SituationComponent))]
    public class EditorSituationComponent : CustomComponent
    {
        protected override bool DrawAllMembersWithTable(ref object target)
        {
            if (target is not SituationComponent situation)
            {
                throw new ArgumentException(nameof(situation));
            }

            bool modified = false;

            if (SituationComponentField.DrawSituationField(
                    situation.Character, situation.Situation, showFirstLinePreview: false, out string result))
            {
                EditorMember situationField = typeof(SituationComponent).
                    TryGetFieldForEditor(nameof(SituationComponent.Situation))!;

                situationField.SetValue(target, result);
                modified = true;
            }

            using TableMultipleColumns table = new($"situation_editor_component",
                flags: ImGuiTableFlags.SizingFixedSame | ImGuiTableFlags.BordersOuter,
                (-1, ImGuiTableColumnFlags.WidthFixed), (-1, ImGuiTableColumnFlags.WidthStretch));

            modified |= DrawAllMembers(
                target,
                exceptForMembers: [nameof(SituationComponent.Situation)]);

            return modified;
        }
    }
}