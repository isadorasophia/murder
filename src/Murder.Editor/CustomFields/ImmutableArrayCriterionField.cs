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
            using TableMultipleColumns table = new($"criteria_{member.Name}", flags: ImGuiTableFlags.SizingFixedFit, 100, 200, 150, 200);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            bool changed = false;

            // If this is not the first element, draw the separating node kind.
            if (index != 0)
            {
                ImGui.PushItemWidth(-1);
                ImGui.PushID($"edit_node_kind");

                // Draw criterion kind
                if (DrawValue(ref node, nameof(CriterionNode.Kind)))
                {
                    changed = true;
                }

                ImGui.PopID();
                ImGui.PopItemWidth();
            }
            else
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "<Matches>");
            }
            
            ImGui.TableNextColumn();

            Criterion criterion = node.Criterion;
            
            // -- Facts across all blackboards --
            if (SearchBox.SearchFacts($"{member.Name}_fact_search", criterion.Fact) is Fact newFact)
            {
                node = node.WithCriterion(criterion.WithFact(newFact));
                changed = true;
            }
            
            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            // -- Select match kind --
            if (CharacterEditor.DrawCriteriaCombo($"criteria_kind_{member.Name}", ref criterion))
            {
                node = node.WithCriterion(criterion);
                changed = true;
            }

            ImGui.PopItemWidth();
            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);
            
            // -- Select requirement value --
            if (criterion.Fact.Kind is not FactKind.Invalid)
            {
                ImGui.PushID($"edit_requirement_{member.Name}");

                // Draw criterion kind
                string targetFieldName = CharacterEditor.GetTargetFieldForFact(criterion.Fact.Kind);
                if (DrawValue(ref criterion, targetFieldName))
                {
                    node = node.WithCriterion(criterion);
                    changed = true;
                }

                ImGui.PopID();
            }
            
            ImGui.PopItemWidth();

            return changed;
        }
    }
}
