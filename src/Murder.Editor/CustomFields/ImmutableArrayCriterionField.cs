using ImGuiNET;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Murder.Core.Dialogs;
using Murder;
using Murder.Editor.Reflection;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.CustomEditors;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<CriterionNode>))]
    internal class ImmutableArrayCriterionField : ImmutableArrayField<CriterionNode>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out CriterionNode criterion)
        {
            criterion = default;
            
            if (ImGuiHelpers.IconButton('\uf055', $"{member.Name}_add", Game.Data.GameProfile.Theme.Accent))
            {
                criterion = new();
                return true;
            }
            
            return false;
        }

        protected override bool DrawElement(ref CriterionNode node, EditorMember member, int index)
        {
            bool changed = false;
            return changed;
        }
    }
}
