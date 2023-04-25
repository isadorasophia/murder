using ImGuiNET;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Murder.Core.Dialogs;
using Murder;
using Murder.Editor.Reflection;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.CustomEditors;
using Murder.Utilities;

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
            using TableMultipleColumns table = new($"criteria_{member.Name}", flags: ImGuiTableFlags.SizingStretchSame, 150, 70, 140);

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
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
            }

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
            if (DrawCriteriaCombo($"criteria_kind_{member.Name}", ref criterion))
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
                string targetFieldName = GetTargetFieldForFact(criterion.Fact.Kind);
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

        internal static bool DrawCriteriaCombo(string id, ref Criterion criterion)
        {
            CriterionKind[] allKinds = criterion.FetchValidCriteriaKind();
            if (allKinds.Length == 0)
            {
                // This may be empty if the criterion has not been initialized yet.
                ImGui.TextColored(Game.Profile.Theme.Warning, "???");
                return false;
            }

            int index = Array.IndexOf(allKinds, criterion.Kind);
            if (index == -1)
            {
                criterion = criterion.WithKind(allKinds[0]);
                return true;
            }

            string[] kindToString = allKinds.Select(k => k.ToCustomString()).ToArray();

            if (ImGui.Combo($"##{id}", ref index, kindToString, kindToString.Length))
            {
                criterion = criterion.WithKind(allKinds[index]);
                return true;
            }

            return false;
        }

        internal static string GetTargetFieldForFact(FactKind kind)
        {
            switch (kind)
            {
                case FactKind.Int:
                    return nameof(Criterion.IntValue);

                case FactKind.String:
                    return nameof(Criterion.StrValue);

                case FactKind.Weight:
                    return nameof(Criterion.IntValue);

                case FactKind.Bool:
                default:
                    return nameof(Criterion.BoolValue);
            }
        }
    }
}
