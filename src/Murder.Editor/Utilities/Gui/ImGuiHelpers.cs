﻿using ImGuiNET;
using System.Numerics;

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
        ImGui.PopStyleColor(3);
        ImGui.PopStyleVar();

        ImGui.SetItemAllowOverlap(); // This is to allow having other buttons OVER our splitter. 

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
        ImGui.PopStyleColor(3);
        ImGui.PopStyleVar();

        ImGui.SetItemAllowOverlap(); // This is to allow having other buttons OVER our splitter. 

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

    public static bool PrettySelectableWithIcon(string label, bool selectable)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, 1f);
        ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(.5f, .5f));

        bool result = ImGui.Selectable($"{label} ", selectable,
            ImGuiSelectableFlags.AllowItemOverlap,
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
        SelectableWithIconColor(text, icon, Game.Profile.Theme.White, Game.Profile.Theme.Faded, selected);

    public static bool SelectableWithIconColor(string text, char icon, Vector4 selectedColor, Vector4 unselectedColor, bool selected)
    {
        var clicked = ShowIcon(icon, selectedColor, unselectedColor, selected);
        ImGui.SameLine();
        clicked = ImGui.Selectable(text, selected) || clicked;

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
        var result = IconButton('\uf2ed', id);
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        return result;
    }

    public static bool IconButton(char icon, string id, Vector4? color = null, Vector4? bgColor = null, bool sameLine = false)
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

    public static (bool modified, int result) DrawEnumField(string id, Type enumType, int fieldValue)
    {
        string[] fields = Enum.GetNames(enumType);
        Array values = Enum.GetValues(enumType);

        int result = 0;

        // Find the right index (tentatively).
        int index = 0;
        foreach (var value in values)
        {
            int v = (int)value;

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

        return (modified, (int)values.GetValue(result)!);
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
}