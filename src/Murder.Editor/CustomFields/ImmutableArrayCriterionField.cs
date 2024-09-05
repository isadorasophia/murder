using ImGuiNET;
using Murder;
using Murder.Core.Dialogs;
using Murder.Core.Sounds;
using Murder.Editor.CustomEditors;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

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

            BlackboardKind kind = BlackboardKind.All;
            if (AttributeExtensions.TryGetAttribute(member, out BlackboardFieldAttribute? blackboardFieldAttribute))
            {
                kind = blackboardFieldAttribute.Kind;
            }

            // -- Facts across all blackboards --
            if (SearchBox.SearchFacts($"{member.Name}_fact_search", criterion.Fact, kind) is Fact newFact)
            {
                if (AssetsFilter.FetchTypeForFact(newFact.EditorName) is Type target)
                {
                    node = node.WithCriterion(criterion.WithFact(newFact, target));
                    changed = true;
                }
                else
                {
                    node = node.WithCriterion(criterion.WithFact(newFact, null));
                    changed = true;
                }
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
                string targetFieldName = DialogActionField.GetTargetFieldForFact(criterion.Fact.Kind);

                bool modifiedField = false;
                if (criterion.Fact.Kind == FactKind.Enum &&
                    AssetsFilter.FetchTypeForFact(criterion.Fact.EditorName) is Type valueType)
                {
                    FieldInfo valueField = typeof(Criterion).GetField(nameof(Criterion.IntValue))!;

                    EditorMember fieldMember = EditorMember.Create(valueField);
                    fieldMember = fieldMember.CreateFrom(valueType, targetFieldName, isReadOnly: false);

                    modifiedField = DrawValue(ref criterion, fieldMember);
                }
                else
                {
                    modifiedField = DrawValue(ref criterion, targetFieldName);
                }

                if (modifiedField)
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
    }
}