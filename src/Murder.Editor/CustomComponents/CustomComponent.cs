using ImGuiNET;
using Murder.Attributes;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.CustomFields;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;

namespace Murder.Editor.CustomComponents
{
    [CustomComponentOf(typeof(object), priority: -1)]
    public class CustomComponent
    {
        public static bool ShowEditorOf<T>(ref T target)
        {
            object? boxed = target;
            if (ShowEditorOf(boxed))
            {
                target = (T)boxed!;
                return true;
            }

            return false;
        }

        public static bool ShowEditorOf(/* ref */ object? target)
        {
            if (target is not null && 
                CustomEditorsHelper.TryGetCustomComponent(target.GetType(), out var customFieldEditor))
            {
                return customFieldEditor.DrawAllMembersWithTable(ref target);
            }

            GameLogger.Error(
                $"Unable to find custom component editor for type: {target?.GetType()?.Name}.");

            return false;
        }

        /// <summary>
        /// Show an editor that targets <paramref name="target"/>.
        /// This returns true whether the object has been modified or not.
        /// </summary>
        protected virtual bool DrawAllMembersWithTable(ref object target)
        {
            bool fileChanged = false;

            if (ImGui.BeginTable($"field_{target.GetType().Name}", 2,
                ImGuiTableFlags.SizingFixedSame | ImGuiTableFlags.BordersOuter))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, -1, 0);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1, 1);
                fileChanged |= DrawAllMembers(target);
                    
                ImGui.EndTable();
            }

            return fileChanged;
        }

        public static bool DrawAllMembers(object target)
        {
            return DrawMembersForTarget(target, GetMembersOf(target.GetType()));
        }

        public static bool DrawMembersForTarget<T>(ref T target, IList<(string, EditorMember)> members)
        {
            object? boxed = target;
            if (boxed is not null && DrawMembersForTarget(boxed, members))
            {
                target = (T)boxed!;
                return true;
            }

            return false;
        }

        public static bool DrawMembersForTarget(object target, IList<(string, EditorMember)> members)
        {
            bool fileChanged = false;

            foreach (var (name, member) in members)
            {
                ImGui.TableNextRow();
                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImGui.Text($"{Prettify.FormatName(name)}:");

                ImGui.TableNextColumn();

                var fieldValue = member.GetValue(target);
                ImGui.PushItemWidth(-1);

                fileChanged |= ProcessInput(target, member, () => CustomField.DrawValue(member, fieldValue));

                ImGui.PopItemWidth();

                if (AttributeExtensions.IsDefined(member, typeof(TooltipAttribute)))
                {
                    if (ImGui.IsItemHovered())
                    {
                        if (AttributeExtensions.TryGetAttribute(member, out TooltipAttribute? tooltip))
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text(tooltip.Text);
                            ImGui.EndTooltip();
                        }
                    }
                }
            }

            return fileChanged;
        }

        internal static bool ProcessField<T>(ref T target, EditorMember member)
        {
            object? value = member.GetValue(target);
            (bool modified, object? newValue) = CustomField.DrawValue(member, value);
            if (modified)
            {
                member.SetValue(ref target, newValue);
            }

            return modified;
        }

        /// <summary>
        /// Process the ImGui with <paramref name="inputAction"/> and 
        /// attribute it to the <paramref name="target"/> if that changed.
        /// </summary>
        internal static bool ProcessInput(object target, EditorMember member, Func<(bool, object?)> inputAction)
        {
            bool fileChanged = false;
            ImGui.PushID(member.Name);

            (bool hasInput, object? newValue) = inputAction();
            if (hasInput)
            {
                fileChanged = true;

                // Set new value set by the user.
                member.SetValue(target, newValue);
            }

            ImGui.PopID();

            return fileChanged;
        }

        protected static IList<(string, EditorMember)> GetMembersOf(Type t)
        {
            return t.GetFieldsForEditor().Select(f => (f.Name, f)).ToList();
        }
    }
}
