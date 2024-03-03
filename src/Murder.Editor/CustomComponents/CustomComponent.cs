using ImGuiNET;
using Murder.Attributes;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;

namespace Murder.Editor.CustomComponents
{
    [Flags]
    public enum CustomComponentsFlags
    {
        None = 0,
        SkipSameLineForFilterField = 1 << 0, // The Filter Field on common components starts with ImGui.SameLine. This skips that
    }


    [CustomComponentOf(typeof(object), priority: -1)]
    public class CustomComponent
    {
        private readonly Dictionary<string, string> _searchField = new();

        public static bool ShowEditorOf<T>(ref T target, CustomComponentsFlags flags = CustomComponentsFlags.None)
        {
            if (target is null)
            {
                GameLogger.Error("Unable to show the editor of null target.");
                return false;
            }

            if (CustomEditorsHelper.TryGetCustomComponent(target.GetType(), out var customFieldEditor))
            {
                object? boxed = target!;
                if (customFieldEditor.DrawAllMembersWithTable(ref boxed, !flags.HasFlag(CustomComponentsFlags.SkipSameLineForFilterField)))
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
                return customFieldEditor.DrawAllMembersWithTable(ref target, true);
            }

            GameLogger.Error(
                $"Unable to find custom component editor for type: {target?.GetType()?.Name}.");

            return false;
        }

        /// <summary>
        /// Show an editor that targets <paramref name="target"/>.
        /// This returns true whether the object has been modified or not.
        /// </summary>
        /// <param name="target">The target object</param>
        /// <param name="sameLineFilter">Will draw the filter field on the same line, if available.</param>
        /// <returns></returns>
        protected virtual bool DrawAllMembersWithTable(ref object target, bool sameLineFilter)
        {
            string name = target.GetType().Name;
            bool fileChanged = false;

            if (sameLineFilter)
            {
                ImGui.SameLine();
            }

            ImGui.BeginGroup();

            var filter = _searchField.GetValueOrDefault(name) ?? string.Empty;

            int popColors = 0;
            if (string.IsNullOrWhiteSpace(filter))
            {
                ImGui.PushStyleColor(ImGuiCol.FrameBg, Game.Profile.Theme.Bg);
                popColors++;
            }

            // Draw the X Button

            if (!sameLineFilter) // Why do we need this? I feel I am misunderstanding something from ImGui
            {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4);
            }
            ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.Bg);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Game.Profile.Theme.Bg);
            ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.BgFaded);
            if (ImGui.SmallButton(string.IsNullOrEmpty(filter) ? "" : ""))
            {
                _searchField[name] = filter = string.Empty;
            }
            ImGui.PopStyleColor(3);

            ImGui.SameLine();

            // Draw the search field
            ImGui.PushItemWidth(-1);

            if (!sameLineFilter) // Return the cursor bacck up
            {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 4);
            }

            if (ImGui.InputTextWithHint($"##search_field_{name}", "Filter...", ref filter, 256))
            {
                _searchField[name] = filter;
            }

            ImGui.PopStyleColor(popColors);

            ImGui.EndGroup();
            ImGui.GetWindowDrawList().AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), ImGuiHelpers.MakeColor32(Game.Profile.Theme.BgFaded), 3f);

            IList<(string, EditorMember)> members = GetMembersOf(target.GetType(), exceptForMembers: null);
            if (members.Count == 0)
            {
                return false;
            }

            if (ImGui.BeginTable($"field_{name}", 2,
                 ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersInnerH))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, -1, 0);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1, 1);

                fileChanged |= DrawMembersForTarget(target, members, filter);

                ImGui.EndTable();
            }
            ImGui.PopItemWidth();


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

        public static bool DrawMemberForTarget<T>(ref T target, string[] memberNames)
        {
            if (target is null)
            {
                return false;
            }

            bool modified = false;

            object? boxed = target;
            foreach (string m in memberNames)
            {
                if (target.GetType().TryGetFieldForEditor(m) is not EditorMember member)
                {
                    continue;
                }

                modified |= DrawMemberForTarget(boxed, m, member);
            }

            if (modified)
            {
                target = (T)boxed!;
                return true;
            }

            return false;
        }

        public static bool DrawMemberForTarget<T>(ref T target, string memberName)
        {
            if (target is null || target.GetType().TryGetFieldForEditor(memberName) is not EditorMember member)
            {
                return false;
            }

            object? boxed = target;
            if (boxed is not null && DrawMemberForTarget(boxed, memberName, member))
            {
                target = (T)boxed!;
                return true;
            }

            return false;
        }

        public static bool DrawMembersForTarget(object target, IList<(string, EditorMember)> members, string? filter = null)
        {
            bool fileChanged = false;

            foreach (var (name, member) in members)
            {
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    if (!name.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                }

                fileChanged |= DrawMemberForTarget(target, name, member);
            }

            return fileChanged;
        }

        public static bool DrawMemberForTarget(object target, string memberName, EditorMember member)
        {
            ImGui.TableNextColumn();

            // Draw Label
            ImGui.Text($"{Prettify.FormatName(memberName)}:");

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

            bool fileChanged = false;
            DrawMember(target, ref fileChanged, member);

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