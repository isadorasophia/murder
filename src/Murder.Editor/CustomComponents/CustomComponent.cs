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
            if (target is null)
            {
                GameLogger.Error("Unable to show the editor of null target.");
                return false;
            }

            if (CustomEditorsHelper.TryGetCustomComponent(target.GetType(), out var customFieldEditor))
            {
                object? boxed = target!;
                if (customFieldEditor.DrawAllMembersWithTable(ref boxed))
                {
                    target = (T)boxed!;
                    return true;
                }

                return false;
            }

            GameLogger.Error(
                $"Unable to find custom component editor for type: {target?.GetType()?.Name}.");

            return false;
        }
        
        public static bool ShowEditorOf(/* ref */ object? target)
        {
            if (target is null)
            {
                GameLogger.Error("Unable to show the editor of null target.");
                return false;
            }

            if (CustomEditorsHelper.TryGetCustomComponent(target.GetType(), out var customFieldEditor))
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

            IList<(string, EditorMember)> members = GetMembersOf(target.GetType(), exceptForMembers: null);
            if (members.Count == 0)
            {
                return false;
            }

            if (ImGui.BeginTable($"field_{target.GetType().Name}", 2,
                 ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersInnerH))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, -1, 0);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1, 1);
                
                fileChanged |= DrawMembersForTarget(target, members);

                ImGui.EndTable();
            }
            
            return fileChanged;
        }

        public static bool DrawAllMembers(object target, HashSet<string>? exceptForMembers = null)
        {
            return DrawMembersForTarget(target, GetMembersOf(target.GetType(), exceptForMembers));
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
                ImGui.TableNextColumn();
                // Draw Label
                ImGui.Text($"{Prettify.FormatName(name)}:");

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
                ImGui.TableNextColumn();
                DrawMember(target,ref fileChanged, member);
            }
            return fileChanged;
        }

        private static void DrawMember(object target, ref bool fileChanged, EditorMember member)
        {
            var fieldValue = member.GetValue(target);

            ImGui.PushItemWidth(-1);
            fileChanged |= ProcessInput(target, member, () => CustomField.DrawValue(member, fieldValue));
            ImGui.PopItemWidth();
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
            if (hasInput && !member.IsReadOnly)
            {
                fileChanged = true;

                // Set new value set by the user.
                member.SetValue(target, newValue);
            }

            ImGui.PopID();

            return fileChanged;
        }

        public static IList<(string, EditorMember)> GetMembersOf(Type t, HashSet<string>? exceptForMembers)
        {
            return t.GetFieldsForEditor().Select(f => (f.Name, f))
                .Where(f => exceptForMembers == null ? true : !exceptForMembers.Contains(f.Name)).ToList();
        }
    }
}
