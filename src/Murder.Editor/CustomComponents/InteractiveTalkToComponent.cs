using Bang.Interactions;
using ImGuiNET;
using Murder.Core.Dialogs;
using Murder.Editor.Attributes;
using Murder.Editor.CustomEditors;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Interactions;
using System.Collections.Immutable;

namespace Murder.Editor.CustomComponents
{
    [CustomComponentOf(typeof(InteractiveComponent<TalkToInteraction>))]
    public class InteractiveTalkToComponent : InteractiveComponent
    {
        protected override bool DrawInteraction(object? interaction)
        {
            if (interaction is not TalkToInteraction talkToInteraction)
            {
                throw new ArgumentException(nameof(interaction));
            }

            using TableMultipleColumns table = new($"talk_to_interaction",
                flags: ImGuiTableFlags.SizingFixedSame | ImGuiTableFlags.BordersOuter,
                (-1, ImGuiTableColumnFlags.WidthFixed), (-1, ImGuiTableColumnFlags.WidthStretch));

            return DrawMembersForTarget(interaction, TalkToMembers());
        }

        private IList<(string, EditorMember)>? _cachedMembers = null;

        private IList<(string, EditorMember)> TalkToMembers()
        {
            if (_cachedMembers is null)
            {
                HashSet<string> skipFields = new string[] { "Situation" }.ToHashSet();

                _cachedMembers ??= typeof(TalkToInteraction).GetFieldsForEditor()
                    .Where(t => !skipFields.Contains(t.Name)).Select(f => (f.Name, f)).ToList();
            }

            return _cachedMembers;
        }
    }
}