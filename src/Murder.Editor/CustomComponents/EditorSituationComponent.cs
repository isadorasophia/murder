using ImGuiNET;
using Bang.Interactions;
using System.Collections.Immutable;
using Murder.Interactions;
using Murder.Core.Dialogs;
using Murder.Editor.Reflection;
using Murder.Editor.Attributes;
using Murder.Editor.Utilities;
using Murder.Editor.CustomEditors;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.CustomFields;
using Murder.Components;
using Murder.Core.Particles;

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

            using TableMultipleColumns table = new($"situation_editor_component",
                flags: ImGuiTableFlags.SizingFixedSame | ImGuiTableFlags.BordersOuter,
                (-1, ImGuiTableColumnFlags.WidthFixed), (-1, ImGuiTableColumnFlags.WidthStretch));

            bool modified = false;
            if (SituationComponentField.DrawSituationField(
                    situation.Character, situation.Situation, out int result))
            {
                EditorMember situationField = typeof(SituationComponent).
                    TryGetFieldForEditor(nameof(SituationComponent.Situation))!;

                situationField.SetValue(target, result);
                modified = true;
            }
            
            modified |= DrawAllMembers(
                target,
                exceptForMembers: new HashSet<string>() { nameof(SituationComponent.Situation) });

            return modified;
        }
    }
}
