using Gum.InnerThoughts;
using ImGuiNET;
using Murder.Core.Dialogs;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using System;
using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;

namespace Murder.Editor.CustomComponents
{
    [CustomComponentOf(typeof(SoundRuleAction))]
    public class SoundRuleActionComponent : CustomComponent
    {
        protected override bool DrawAllMembersWithTable(ref object target)
        {
            bool modified = false;

            SoundRuleAction action = (SoundRuleAction)target;
            Type? targetType = FetchTargetFieldType(action.Fact);

            if (targetType is null)
            {
                ImGui.TextColored(Game.Profile.Theme.Warning, "\uf071");
                ImGuiHelpers.HelpTooltip("Rule was not found in the blackboard.");

                ImGui.SameLine();
            }

            modified |= CustomField.DrawValue(ref target, fieldName: nameof(SoundRuleAction.Fact));

            action = (SoundRuleAction)target;
            targetType = FetchTargetFieldType(action.Fact);

            FieldInfo? valueField = typeof(SoundRuleAction).GetField(nameof(SoundRuleAction.Value));

            if (targetType is null || valueField is null)
            {
                return modified;
            }

            if (modified)
            {
                // Initialize the field value, if it's null or a mismatch.
                if (action.Value is null || action.Value.GetType() != targetType)
                {
                    object? v = Activator.CreateInstance(targetType);
                    if (targetType.IsEnum)
                    {
                        v = Convert.ToInt32(v);
                    }

                    valueField.SetValue(target, v);
                }
            }

            // Set the action.
            modified |= DrawActionKind(ref target, action, targetType);

            EditorMember member = EditorMember.Create(valueField);
            member = member.CreateFrom(targetType, nameof(SoundRuleAction.Value), isReadOnly: false);

            ImGui.SameLine();
            object? value = valueField.GetValue(target);
            (bool modifiedValue, value) = CustomField.DrawValue(member, value);

            if (modifiedValue)
            {
                if (targetType.IsEnum)
                {
                    value = Convert.ToInt32(value);
                }

                valueField.SetValue(target, value);
                modified = true;
            }

            return modified;
        }

        private bool DrawActionKind(ref object target, SoundRuleAction action, Type t)
        {
            BlackboardActionKind[] possibleKinds = FetchTargetActionKind(t);
            if (possibleKinds.Length == 0)
            {
                return false;
            }

            ImGui.SameLine();

            if (possibleKinds.Length == 1)
            {
                ImGui.Text("\uf30b");

                return false;
            }

            bool modified = false;

            ImGui.PushItemWidth(80);

            BlackboardActionKind kind = action.Kind;

            // Multiple choices!
            if (ImGuiHelpers.DrawEnumField($"##sound_rule_{action.Fact.Name}", possibleKinds, ref kind))
            {
                FieldInfo? field = typeof(SoundRuleAction).GetField(nameof(SoundRuleAction.Kind));
                field?.SetValue(target, kind);

                modified = true;
            }

            ImGui.PopItemWidth();

            return modified;
        }

        public static Type? FetchTargetFieldType(SoundFact fact)
        {
            if (string.IsNullOrEmpty(fact.Name))
            {
                return null;
            }

            ImmutableArray<Type> soundBlackboards = AssetsFilter.FetchAllSoundBlackboards();
            foreach (Type t in soundBlackboards)
            {
                FieldInfo? field = t.GetField(fact.Name);
                if (field is not null)
                {
                    return field.FieldType;
                }
            }

            return null;
        }

        private static BlackboardActionKind[] FetchTargetActionKind(Type t)
        {
            if (t.IsPrimitive && !t.IsEnum)
            {
                return new BlackboardActionKind[]
                    { BlackboardActionKind.Add, BlackboardActionKind.Set, BlackboardActionKind.Minus };
            }

            return new BlackboardActionKind[] { BlackboardActionKind.Set };
        }
    }
}