using ImGuiNET;
using Murder.Core.Graphics;
using Murder.Editor.Reflection;
using Murder.Utilities;
using System;
using System.Numerics;
using System.Text;

namespace Murder.Editor.ImGuiExtended;

public static class ImGuiHelpers
{
    /// <summary>
    /// Draws a splitter area after the next item with size0 height/width
    /// Don't forget to add a ImGui.Dummy(new Vector2(0,8)) after it to reserve the space for the splitter
    /// 
    /// </summary>
    public static void DrawSplitter(string id, bool split_vertically, float thickness, ref float size0, ref float size1,
            float min_size0, float min_size1, float size = -1.0f)
    {
        var backup_pos = ImGui.GetCursorPos();

        if (split_vertically)
            ImGui.SetCursorPosY(backup_pos.Y + size0 + 4);
        else
            ImGui.SetCursorPosX(backup_pos.X + size0);

        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
        ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(.6f, 0.6f, 0.6f, .5f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.6f, 0.6f, 0.6f, 0.1f));

        ImGui.Button(id, new Vector2(!split_vertically ? thickness : size, split_vertically ? thickness : size));

        Vector2 min = ImGui.GetItemRectMin();
        Vector2 max = ImGui.GetItemRectMax();
        Vector2 mid = Vector2.Lerp(min, max, 0.5f);

        if (split_vertically)
        {
            ImGui.GetWindowDrawList().AddLine(new Vector2(mid.X - 20, mid.Y), new Vector2(mid.X + 20, mid.Y), Color.ToUint(Game.Profile.Theme.Faded), 2);
        }
        else
        {
            ImGui.GetWindowDrawList().AddLine(new Vector2(mid.X, mid.Y - 20), new Vector2(mid.X, mid.Y + 20), Color.ToUint(Game.Profile.Theme.Faded), 2);
        }

        ImGui.PopStyleColor(3);
        ImGui.PopStyleVar();

        ImGui.SetNextItemAllowOverlap(); // This is to allow having other buttons OVER our splitter. 

        float previous0 = size0;
        if (ImGui.IsItemActive())
        {
            float mouseDelta = split_vertically ? ImGui.GetMouseDragDelta(ImGuiMouseButton.Left, 0f).Y : ImGui.GetMouseDragDelta(ImGuiMouseButton.Left, 0f).X;
            if (mouseDelta != 0)
            {
                size0 += mouseDelta;

                size0 = Math.Max(size0, min_size0);
                size1 = Math.Max(size1 + (previous0 - size0), min_size1);
                ImGui.ResetMouseDragDelta(ImGuiMouseButton.Left);
            }
        }

        ImGui.SetCursorPos(backup_pos);
    }
    /// <summary>
    /// Draws a splitter area after the next item with size0 height/width
    /// Don't forget to add a ImGui.Dummy(new Vector2(0,8)) after it to reserve the space for the splitter
    /// </summary>
    public static void DrawSplitter(string id, bool split_vertically, float thickness, ref float size0,
            float min_size0, float size = -1.0f)
    {
        var backup_pos = ImGui.GetCursorPos();

        if (split_vertically)
            ImGui.SetCursorPosY(backup_pos.Y + size0 + 4);
        else
            ImGui.SetCursorPosX(backup_pos.X + size0);

        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
        ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(.6f, 0.6f, 0.6f, .5f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.6f, 0.6f, 0.6f, 0.1f));

        ImGui.Button(id, new Vector2(!split_vertically ? thickness : size, split_vertically ? thickness : size));

        Vector2 min = ImGui.GetItemRectMin();
        Vector2 max = ImGui.GetItemRectMax();
        Vector2 mid = Vector2.Lerp(min, max, 0.5f);

        if (split_vertically)
        {
            ImGui.GetWindowDrawList().AddLine(new Vector2(mid.X - 20, mid.Y), new Vector2(mid.X + 20, mid.Y), Color.ToUint(Game.Profile.Theme.Faded), 2);
        }
        else
        {
            ImGui.GetWindowDrawList().AddLine(new Vector2(mid.X, mid.Y - 20), new Vector2(mid.X, mid.Y + 20), Color.ToUint(Game.Profile.Theme.Faded), 2);
        }

        ImGui.PopStyleColor(3);
        ImGui.PopStyleVar();

        ImGui.SetNextItemAllowOverlap(); // This is to allow having other buttons OVER our splitter. 

        if (ImGui.IsItemActive())
        {
            float mouseDelta = split_vertically ? ImGui.GetMouseDragDelta(ImGuiMouseButton.Left, 0f).Y : ImGui.GetMouseDragDelta(ImGuiMouseButton.Left, 0f).X;
            if (mouseDelta != 0)
            {
                size0 += mouseDelta;

                size0 = Math.Max(size0, min_size0);
                ImGui.ResetMouseDragDelta(ImGuiMouseButton.Left);
            }
        }

        ImGui.SetCursorPos(backup_pos);
    }

    public static uint MakeColor32(byte r, byte g, byte b, byte a) { uint ret = a; ret <<= 8; ret += b; ret <<= 8; ret += g; ret <<= 8; ret += r; return ret; }

    public static uint MakeColor32(Vector4 vector) => MakeColor32((byte)(vector.X * 255), (byte)(vector.Y * 255), (byte)(vector.Z * 255), (byte)(vector.W * 255));

    public static void HelpTooltip(string description)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35f);
            ImGui.TextUnformatted(description);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }
    public static bool PressedButtton(string label, ref bool pressed, bool reverse = false)
    {
        bool wasPressed = (pressed && !reverse) || (!pressed && reverse);
        if (wasPressed)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.BgFaded);
        }

        bool down = false;
        if (ImGui.Button(label))
        {
            pressed = !pressed;
            down = true;
        }

        if (wasPressed)
        {
            ImGui.PopStyleColor(1);
        }

        return down;
    }
    public static bool SelectedButton(string label, Vector4? color = default)
    {
        color ??= Game.Profile.Theme.BgFaded;

        ImGui.PushStyleColor(ImGuiCol.Button, color.Value);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color.Value);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, color.Value);

        var pressed = ImGui.Button(label);

        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();

        return pressed;
    }

    public static bool SelectedImageButton(nint image, Vector2 size, Vector4? color = default)
    {
        color ??= Game.Profile.Theme.BgFaded;

        ImGui.PushStyleColor(ImGuiCol.Button, color.Value);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color.Value);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, color.Value);

        var pressed = ImGui.ImageButton($"{image}_selected_img", image, size);

        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();

        return pressed;
    }

    public static bool LowButton(string label)
    {
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.5f, 0.5f, 0, 1));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.5f, 0, 1));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(1, 1, 0.25f, 1));
        var pressed = ImGui.Button(label);
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();

        return pressed;
    }

    public static bool TreeNodeWithIcon(char icon, string label, ImGuiTreeNodeFlags flags)
    {
        int indexOfLabel = label.IndexOf('#');
        string id = indexOfLabel == -1 ? label : label.Substring(label.LastIndexOf('#') + 1);
        bool result = ImGui.TreeNodeEx($"{icon} {(indexOfLabel == -1 ? label : label.Substring(0, indexOfLabel))} ###{id}", flags);

        return result;
    }

    public static bool TreeNodeWithIconAndColor(
        char icon, string label, ImGuiTreeNodeFlags flags, Vector4 text, Vector4 background, Vector4 active)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1f);

        ImGui.PushStyleColor(ImGuiCol.Text, text);
        ImGui.PushStyleColor(ImGuiCol.Header, background);
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, active);
        ImGui.PushStyleColor(ImGuiCol.HeaderActive, active);

        bool result = TreeNodeWithIcon(icon, label, flags);

        ImGui.PopStyleVar();
        ImGui.PopStyleColor(4);

        return result;
    }

    public static bool PrettySelectableWithIcon(string label, bool selectable, bool disabled = false)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1f);
        ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(.5f, .5f));

        ImGuiSelectableFlags flags = ImGuiSelectableFlags.AllowOverlap;
        if (disabled)
        {
            flags |= ImGuiSelectableFlags.Disabled;
        }

        bool result = ImGui.Selectable($"{label} ", selectable,
            flags,
            new(x: 0, 18));

        ImGui.AlignTextToFramePadding();

        ImGui.PopStyleVar();
        ImGui.PopStyleVar();

        return result;
    }

    public static void ColorIcon(char icon, Vector4 color)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, color);
        ImGui.Text($"{icon}");
        ImGui.PopStyleColor();
    }

    public static bool ShowIcon(char icon, Vector4 selectedColor, Vector4 unselectedColor, bool selected = true)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, selected ? selectedColor : unselectedColor);
        var clicked = ImGui.Selectable(icon.ToString());
        ImGui.PopStyleColor();
        return clicked;
    }

    public static bool FadedSelectableWithIcon(string text, char icon, bool selected)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Faded);
        bool clicked = SelectableWithIcon(text, icon, selected);
        ImGui.PopStyleColor();

        return clicked;
    }

    public static bool SelectableWithIcon(string text, char icon, bool selected) =>
        SelectableWithIconColor(text, icon, Game.Profile.Theme.White, Game.Profile.Theme.White, Game.Profile.Theme.Faded, selected);

    public static bool SelectableWithIconColor(string text, char icon, Vector4 iconSelectedColor, Vector4 selectedColor, Vector4 unselectedColor, bool selected)
    {
        var clicked = ShowIcon(icon, iconSelectedColor, unselectedColor, selected);
        ImGui.SameLine();
        ImGui.PushStyleColor(ImGuiCol.Text, selected ? selectedColor : unselectedColor);
        clicked = ImGui.Selectable(text, selected) || clicked;
        ImGui.PopStyleColor();
        return clicked;
    }

    public static bool BeginPopupModalCentered(string name)
    {
        Vector2 center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));

        return BeginPopupModal(name, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.Modal);
    }

    public static bool BeginPopupModal(string name, ImGuiWindowFlags _)
    {
        return ImGui.BeginPopupModal(name);
    }

    public static bool DeleteButton(string id)
    {
        var theme = Game.Profile.Theme;

        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, theme.Red);
        ImGui.PushStyleColor(ImGuiCol.Button, theme.Faded);
        var result = IconButton('\uf2ed', id, tooltip: "Delete");
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        return result;
    }

    public static bool BlueIcon(char name, string id)
    {
        var theme = Game.Profile.Theme;

        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, theme.Red);
        ImGui.PushStyleColor(ImGuiCol.Button, theme.Faded);
        var result = IconButton(name, id);
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();

        return result;
    }

    public static bool IconButton(char icon, string id, Vector4? color = null, Vector4? bgColor = null, bool sameLine = false, string? tooltip = null)
    {
        if (sameLine)
        {
            ImGui.SameLine();
        }

        if (color is not null)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, color.Value);
        }

        if (bgColor is not null)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, bgColor.Value);
        }

        ImGui.PushID(id);
        var result = ImGui.Button(icon.ToString());
        ImGui.PopID();

        if (color is not null)
        {
            ImGui.PopStyleColor();
        }

        if (bgColor is not null)
        {
            ImGui.PopStyleColor();
        }

        if (tooltip is not null)
        {
            HelpTooltip(tooltip);
        }

        return result;
    }

    public static bool ColoredIconButton(char icon, string id, bool isActive)
    {
        Vector4 textColor = Game.Profile.Theme.White;
        Vector4 selectedTextColor = Game.Profile.Theme.Faded;

        Vector4 backgroundColor = Game.Profile.Theme.Bg;

        ImGui.PushStyleColor(ImGuiCol.Text, isActive ? textColor : selectedTextColor);

        ImGui.PushStyleColor(ImGuiCol.Button, backgroundColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, backgroundColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, backgroundColor);

        ImGui.PushID(id);
        var result = ImGui.Button(icon.ToString());
        ImGui.PopID();

        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();

        ImGui.PopStyleColor();

        return result;
    }

    public static bool SelectedIconButton(char icon, Vector4? color = default)
    {
        color ??= Game.Profile.Theme.BgFaded;

        ImGui.PushStyleColor(ImGuiCol.Button, color.Value);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color.Value);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, color.Value);

        var pressed = ImGui.Button(icon.ToString());

        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();

        return pressed;
    }

    public static void DisabledButton(string text)
        => DisabledButton(() => ImGui.Button(text));

    public static void DisabledButton(Action button)
    {
        var theme = Game.Profile.Theme;
        ImGui.PushStyleColor(ImGuiCol.Button, theme.BgFaded);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, theme.BgFaded);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, theme.BgFaded);

        button();

        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
    }

    public static bool DrawEnumField<T>(string id, T[] values, ref T initialValue) where T : Enum
    {
        bool modified = false;

        Type enumType = typeof(T);

        string[] enumFields = values.Select(v => v.ToString()).ToArray();
        int index = Array.IndexOf(values, initialValue);
        if (index == -1)
        {
            index = 0;
        }

        modified = ImGui.Combo(id, ref index, enumFields, enumFields.Length);
        initialValue = values[index];

        return modified;
    }

    public static bool DrawEnumField<T>(string id, ref T fieldValue) where T : Enum
    {
        var t = fieldValue.GetType();

        var (mod, result) = DrawEnumField(id, t, (int)(object)fieldValue!);
        fieldValue = (T)(object)result;

        return mod;
    }

    private static string _enumFilterCache = string.Empty;
    public static bool DrawEnumFieldWithSearch<T>(string id, ref T fieldValue) where T : Enum
    {
        var t = fieldValue.GetType();
        bool modified = false;

        string[] fieldNames = Enum.GetNames(t);
        Array values = Enum.GetValues(t);

        int selectedValue = 0;
        int selectedIndex = 0;

        // Find the right index (tentatively).
        int i = 0;
        foreach (var value in values)
        {
            int v = Convert.ToInt32(value);

            if (v == (int)(object)fieldValue!)
            {
                selectedValue = v;
                selectedIndex = i;
                break;
            }

            i++;
        }

        bool clicked = ImGui.ArrowButton("##" + id, ImGuiDir.Down);
        Vector2 startPosition = ImGui.GetItemRectMax();

        ImGui.SameLine();

        ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Accent);
        ImGui.PushStyleColor(ImGuiCol.Header, Game.Profile.Theme.Bg);
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Game.Profile.Theme.Faded);
        clicked |= ImGui.Selectable(fieldNames[selectedIndex], true, ImGuiSelectableFlags.None);
        ImGui.PopStyleColor(3);


        if (clicked)
        {
            ImGui.OpenPopup($"{id}_search");
            ImGui.SetNextWindowPos(startPosition, ImGuiCond.Always);
            _enumFilterCache = string.Empty;
        }

        if (ImGui.BeginPopup($"{id}_search", ImGuiWindowFlags.NoMove))
        {
            ImGui.SetWindowPos(startPosition, ImGuiCond.Always);

            if (clicked)
            {
                ImGui.SetKeyboardFocusHere();
            }
            ImGui.InputText($"##{id}-search-field", ref _enumFilterCache, 364);

            for (int index = 0; index < fieldNames.Length; index++)
            {
                string EnumFieldName = fieldNames[index];
                int value = Convert.ToInt32(values.GetValue(index));

                if (!string.IsNullOrWhiteSpace(EnumFieldName) && !StringHelper.FuzzyMatch(_enumFilterCache, EnumFieldName))
                {
                    continue;
                }

                if (ImGui.Selectable(EnumFieldName, value == selectedValue, ImGuiSelectableFlags.None, new Vector2(0, 0)))
                {
                    selectedValue = value;
                    modified = true;
                }
            }

            fieldValue = (T)(object)selectedValue;


            ImGui.EndPopup();
        }

        return modified;
    }

    // Helper method to calculate the combined value of all enum flags
    private static int GetAllFlagsValue(Type enumType)
    {
        int allFlags = 0;
        foreach (int value in Enum.GetValues(enumType))
        {
            allFlags |= value;
        }
        return allFlags;
    }

    public static bool DrawEnumFieldAsFlagList(string id, string emptyText, Type enumType, ref int intValue)
        => DrawEnumFieldAsFlagList(id, emptyText, null, enumType, ref intValue);

    public static bool DrawEnumFieldAsFlagList(string id, string? emptyText, string? allText, Type enumType, ref int intValue)
    {
        bool modified = false;

        if (Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
        {
            // Check if a single value is selected or if multiple values are selected.
            bool isSingleValue = (intValue & (intValue - 1)) == 0;

            if (!string.IsNullOrEmpty(emptyText))
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.BgFaded);
                ImGui.BeginDisabled(intValue == 0);
                if (ImGui.Button($"{emptyText}##{id}-clear"))
                {
                    intValue = 0;
                    modified = true;
                }
                ImGui.SameLine();
                ImGui.EndDisabled();
                ImGui.PopStyleColor();
            }

            int allFlags = GetAllFlagsValue(enumType);
            if (!string.IsNullOrEmpty(allText))
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.BgFaded);
                ImGui.BeginDisabled(intValue == allFlags);
                if (ImGui.Button($"{allText}##{id}-all"))
                {
                    intValue = GetAllFlagsValue(enumType);
                    modified = true;
                }
                ImGui.SameLine();
                ImGui.EndDisabled();
                ImGui.PopStyleColor();
            }

            // Now draw a combo box, highlighting the ones selected.
            Array values = Enum.GetValues(enumType);
            string[] prettyNames = Enum.GetNames(enumType);

            bool comboOpen = false;

            // Find the right index (tentatively).
            if (isSingleValue)
            {
                int index = 0;
                int result = 0;

                foreach (var value in values)
                {
                    int v = Convert.ToInt32(value);
                    if (v == intValue)
                    {
                        result = index;
                        break;
                    }

                    index++;
                }
                comboOpen = (ImGui.BeginCombo($"{id}-enum-combo", prettyNames[result]));
            }
            else
            {
                if (intValue == allFlags)
                {
                    // If all values are selected, we can show the pretty name of that value
                    comboOpen = (ImGui.BeginCombo($"{id}-enum-combo", allText));
                }
                else
                {
                    // If multiple values are selected, we need to show a comma separated list
                    StringBuilder selected = new();
                    foreach (var value in values)
                    {
                        int v = Convert.ToInt32(value);
                        if (v == 0 || v == allFlags)
                        {
                            continue;
                        }

                        if ((v & intValue) != 0)
                        {
                            if (selected.Length > 0)
                            {
                                selected.Append(", ");
                            }
                            selected.Append(prettyNames[Array.IndexOf(values, value)]);
                        }
                    }

                    comboOpen = ImGui.BeginCombo($"{id}-enum-combo", selected.ToString());
                }
            }

            if (comboOpen)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (values.GetValue(i) is not object objValue)
                    {
                        continue;
                    }

                    int value = (int)objValue;
                    if (value == 0 || value == allFlags)
                    {
                        continue;
                    }

                    bool isChecked = (value & intValue) != 0;

                    bool changed = false;
                    changed |= ImGui.Checkbox($"##{id}-{i}-layer", ref isChecked);
                    ImGui.SameLine();
                    if (ImGui.Selectable(prettyNames[i], isChecked, ImGuiSelectableFlags.NoAutoClosePopups))
                    {
                        changed = true;
                        isChecked = !isChecked;
                    }

                    if (changed)
                    {
                        if (isChecked)
                        {
                            intValue |= value;
                        }
                        else
                        {
                            intValue &= ~value;
                        }

                        modified = true;
                    }

                }
                ImGui.EndCombo();
            }

        }
        else
        {
            DrawEnumField(id, enumType, ref intValue, ref modified);
        }

        return modified;
    }
    public static bool DrawEnumFieldAsFlags(string id, Type enumType, ref int intValue, int width = -1)
    {
        using TableMultipleColumns table = new($"##{id}-col-table",
            flags: ImGuiTableFlags.SizingFixedFit, width, width, width);

        ImGui.TableNextRow();
        ImGui.TableNextColumn();

        Array values = Enum.GetValues(enumType);
        string[] prettyNames = Enum.GetNames(enumType);

        int tableIndex = 0;
        bool changed = false;
        for (int i = 0; i < values.Length; i++)
        {
            if (values.GetValue(i) is not object objValue)
            {
                continue;
            }

            int value = (int)objValue;
            if (value == 0)
            {
                continue;
            }

            bool isChecked = ~(~value | intValue) == 0;

            if (ImGui.Checkbox($"##{id}-{i}-col-layer", ref isChecked))
            {
                if (isChecked)
                {
                    intValue |= value;
                }
                else
                {
                    intValue &= ~value;
                }

                changed = true;
            }

            ImGui.SameLine();
            ImGui.Text(prettyNames[i]);

            ImGui.TableNextColumn();

            if ((tableIndex + 1) % 3 == 0)
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
            }

            tableIndex++;

        }

        return changed;
    }

    public static void DrawEnumField(string id, Type enumType, ref int fieldValue, ref bool modified)
    {
        string[] fields = Enum.GetNames(enumType);
        Array values = Enum.GetValues(enumType);

        int result = 0;

        // Find the right index (tentatively).
        int index = 0;
        foreach (var value in values)
        {
            int v = Convert.ToInt32(value);

            if (v == fieldValue)
            {
                result = index;
                break;
            }

            index++;
        }

        modified |= ImGui.Combo($"{id}-enum-combo", ref result, fields, fields.Length);
        if (result < 0)
        {
            return;
        }

        fieldValue = Convert.ToInt32(values.GetValue(result));
    }

    public static (bool modified, int result) DrawEnumFieldWithSearch(string id, Type enumType, int fieldValue)
    {
        string[] fields = Enum.GetNames(enumType);
        Array values = Enum.GetValues(enumType);

        int result = 0;

        // Find the right index (tentatively).
        int index = 0;
        foreach (var value in values)
        {
            int v = Convert.ToInt32(value);

            if (v == fieldValue)
            {
                result = index;
                break;
            }

            index++;
        }

        bool modified = ImGui.Combo(id, ref result, fields, fields.Length);
        if (result < 0)
        {
            return (false, 0);
        }

        return (modified, Convert.ToInt32(values.GetValue(result)));
    }

    public static (bool modified, int result) DrawEnumField(string id, Type enumType, int fieldValue)
    {
        string[] fields = Enum.GetNames(enumType);
        Array values = Enum.GetValues(enumType);

        int result = 0;

        // Find the right index (tentatively).
        int index = 0;
        foreach (var value in values)
        {
            int v = Convert.ToInt32(value);

            if (v == fieldValue)
            {
                result = index;
                break;
            }

            index++;
        }

        bool modified = ImGui.Combo(id, ref result, fields, fields.Length);
        if (result < 0)
        {
            return (false, 0);
        }

        return (modified, Convert.ToInt32(values.GetValue(result)));
    }



    public static void DrawHistogram(IEnumerable<(string label, double size)> values)
    {
        uint[] colors = new uint[] { ImGuiHelpers.MakeColor32(Game.Profile.Theme.Accent), ImGuiHelpers.MakeColor32(Game.Profile.Theme.HighAccent), ImGuiHelpers.MakeColor32(Game.Profile.Theme.Yellow) };

        float width = ImGui.GetContentRegionAvail().X;
        float height = 25;

        Vector2 size = new(width, height);
        Vector2 position = ImGui.GetCursorScreenPos();

        ImDrawListPtr ptr = ImGui.GetWindowDrawList();

        ptr.AddRectFilled(
            p_min: position, p_max: position + size, col: ImGuiHelpers.MakeColor32(Game.Profile.Theme.Faded), rounding: 4f);

        Vector2 verticalPadding = new(0, 3);
        Vector2 horizontalPadding = new(3, 0);

        // Apply horizontal padding
        position += horizontalPadding;

        float maxX = position.X + size.X - 2 * horizontalPadding.X;

        int index = 0;

        double totalSize = 0;
        List<(string label, double currentSize)> highestValues = values.OrderByDescending(x => x.size).Take(8).ToList();

        foreach ((string label, double currentSize) in highestValues)
        {
            totalSize += currentSize;
        }

        foreach ((string label, double currentSize) in highestValues)
        {
            float currentWidth = (float)(currentSize / totalSize) * (width - horizontalPadding.X * 2);

            Vector2 barSize = new(x: currentWidth, height);

            Vector2 min = position + verticalPadding;
            Vector2 max = position + barSize - verticalPadding;

            uint color = colors[index++ % 3];
            if (ImGui.IsMouseHoveringRect(min, max, clip: false))
            {
                color = ImGuiHelpers.MakeColor32(Game.Profile.Theme.White);
                ImGui.SetTooltip(label);
            }

            ptr.AddRectFilled(min, max, color, rounding: 2f);

            position += new Vector2(barSize.X, 0);
        }
    }

    internal static bool SelectableColor(string id, Vector4 color)
    {
        var interacted = ImGui.Selectable(id);
        var p_min = ImGui.GetItemRectMin();
        var p_max = ImGui.GetItemRectMax();
        ImGui.GetWindowDrawList().AddRectFilled(p_min, p_max, ImGuiHelpers.MakeColor32(color));

        return interacted;
    }

    public static void DrawBorderOnPreviousItem(Vector4 color, float padding, float rounding = 5)
    {
        Vector2 min = ImGui.GetItemRectMin() - new Vector2(padding);
        Vector2 max = ImGui.GetItemRectMax() + new Vector2(padding);

        var dl = ImGui.GetWindowDrawList();

        dl.AddRect(min, max, Color.ToUint(color), rounding);
    }
}