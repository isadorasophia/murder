using ImGuiNET;
using Murder.Assets;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Editor.Reflection;
using Murder.Interactions;
using System.Collections.Immutable;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(SituationComponent), priority: 10)]
    internal class SituationComponentField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            SituationComponent situation = (SituationComponent)fieldValue!;

            if (DrawSituationField(situation.Character, situation.Situation, out int result))
            {
                situation = situation.WithSituation(result);
                modified = true;
            }

            modified |= DrawValue(ref situation, nameof(SituationComponent.Character));

            return (modified, situation);
        }

        /// <summary>
        /// Draw field for <see cref="TalkToInteraction.Situation"/> in <see cref="TalkToInteraction"/>.
        /// </summary>
        public static bool DrawSituationField(Guid character, int situation, out int result)
        {
            result = 0;

            return false;
        }
    }
}

