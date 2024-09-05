using ImGuiNET;
using Murder.Attributes;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Utilities;
using Murder.Utilities.Attributes;

namespace Murder.Editor.CustomComponents;

[Flags]
public enum CustomComponentsFlags
{
    None = 0,
}


[CustomComponentOf(typeof(object), priority: -1)]
public class CustomComponent
{
    private readonly Dictionary<string, string> _searchField = [];
    private static readonly HashSet<string> _unfoldedFolders = [];

    public static bool ShowEditorOf<T>(ref T target, CustomComponentsFlags _ = CustomComponentsFlags.None)
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
    /// <param name="target">The target object</param>
    /// <param name="sameLineFilter">Will draw the filter field on the same line, if available.</param>
    /// <returns></returns>
    protected virtual bool DrawAllMembersWithTable(ref object target)
    {
        Type type = target.GetType();
        string name = type.Name;
        bool fileChanged = false;

        IList<(string, EditorMember)> members = GetMembersOf(target.GetType(), exceptForMembers: null);
        if (members.Count == 0)
        {
            return false;
        }

        // Draw the X Button
        var filter = _searchField.GetValueOrDefault(name) ?? string.Empty;

        EditorFieldFlags flags = EditorFieldFlags.None;
        // Use GetCustomAttributes to get all attributes of the specified type
        object[] attributes = type.GetCustomAttributes(typeof(EditorFieldPropertiesAttribute), false);
        if (attributes.Length > 0)
        {
            // Cast the first attribute to the correct type and get the Flags
            EditorFieldPropertiesAttribute attribute = (EditorFieldPropertiesAttribute)attributes[0];
            flags = attribute.Flags;
        }


        if (!flags.HasFlag(EditorFieldFlags.NoFilter) && members.Count > 5) 
        {
            ImGui.BeginGroup();


            int popColors = 0;
            if (string.IsNullOrWhiteSpace(filter))
            {
                ImGui.PushStyleColor(ImGuiCol.FrameBg, Game.Profile.Theme.Bg);
                popColors++;
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

            if (ImGui.InputTextWithHint($"##search_field_{name}", "Filter...", ref filter, 256))
            {
                _searchField[name] = filter;
            }
            
            ImGui.PopItemWidth();

            ImGui.PopStyleColor(popColors);
            ImGui.EndGroup();
            ImGui.GetWindowDrawList().AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), ImGuiHelpers.MakeColor32(Game.Profile.Theme.BgFaded), 3f);

        }
        else if (flags.HasFlag(EditorFieldFlags.SingleLine))
        {
            ImGui.NewLine();
        }

        if (ImGui.BeginTable($"field_{name}", 2,
                ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersInnerH))
        {
            float maxColumnWidth = ImGui.GetContentRegionAvail().X;
            float firstColumnWidth = Math.Clamp(maxColumnWidth * 0.3f, 45, 140);
            float secondColumnWidth = maxColumnWidth - firstColumnWidth;

            ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, firstColumnWidth, 0);
            ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthFixed, secondColumnWidth, 1);

            fileChanged |= DrawMembersForTarget(target, members, filter);

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
        bool unfolded = true;
        if (AttributeExtensions.IsDefined(member, typeof(FolderAttribute)))
        {
            string id = $"{memberName}_{target.GetHashCode()}";

            unfolded = _unfoldedFolders.Contains(id);
            ImGui.PushStyleColor(ImGuiCol.Text, unfolded ? Game.Profile.Theme.White: Game.Profile.Theme.Faded);
            if (ImGui.Selectable($"{(unfolded ? "\uf07c" : "\uf07b")} {Prettify.FormatName(memberName)}{(unfolded ? ":" : "")}", unfolded)) 
            {
                if (unfolded)
                {
                    _unfoldedFolders.Remove(id);
                }
                else
                {
                    _unfoldedFolders.Add(id);
                }
            }

            ImGui.PopStyleColor();
        }
        else
        {
            // Draw Label
            ImGui.PushStyleColor(ImGuiCol.Text, member.GetValue(target) == null ? Game.Profile.Theme.Faded : Game.Profile.Theme.White);

            ImGui.Text($"{Prettify.FormatName(memberName)}:");

            ImGui.PopStyleColor();
        }


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
        if (unfolded)
        {
            bool fileChanged = false;
            DrawMember(target, ref fileChanged, member);

            return fileChanged;
        }

        return false;
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