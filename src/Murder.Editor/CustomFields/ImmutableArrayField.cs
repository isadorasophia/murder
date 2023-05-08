using ImGuiNET;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    public abstract class ImmutableArrayField<T> : CustomField
    {
        protected abstract bool Add(in EditorMember member, [NotNullWhen(true)] out T? element);

        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            ImmutableArray<T> elements = (ImmutableArray<T>)fieldValue!;
            if (elements.IsDefault) elements = ImmutableArray<T>.Empty;

            ImGui.PushID($"Add ${member.Member.ReflectedType}");

            if (Add(member, out T? element))
            {
                elements = elements.Add(element);
                modified = true;
            }

            ImGui.PopID();

            if (modified || elements.Length == 0)
            {
                return (modified, elements);
            }

            for (int index = 0; index < elements.Length; index++)
            {
                ImGui.PushID($"{member.Member.ReflectedType}_{index}");
                element = elements[index];

                ImGui.BeginGroup();
                ImGuiHelpers.IconButton('', $"{member.Name}_alter", Game.Data.GameProfile.Theme.Accent);

                if (ImGui.IsItemHovered())
                {
                    ImGui.OpenPopup($"{member.Member.ReflectedType}_{index}_extras");
                    ImGui.SetNextWindowPos(ImGui.GetItemRectMin() + new System.Numerics.Vector2(-12,-2));
                }

                if (ImGui.BeginPopup($"{member.Member.ReflectedType}_{index}_extras"))
                {
                    ImGui.BeginGroup();
                    if (ImGuiHelpers.IconButton('', $"{member.Name}_remove", Game.Data.GameProfile.Theme.HighAccent))
                    {
                        elements = elements.RemoveAt(index);
                        modified = true;
                    }
                    ImGuiHelpers.HelpTooltip("Remove");

                    if (ImGuiHelpers.IconButton('', $"{member.Name}_duplicate", Game.Data.GameProfile.Theme.HighAccent))
                    {
                        elements = elements.Insert(index, elements[index]);
                        modified = true;
                    }
                    ImGuiHelpers.HelpTooltip("Duplicate");

                    if (ImGuiHelpers.IconButton('', $"{member.Name}_move_up", Game.Data.GameProfile.Theme.HighAccent))
                    {
                        if (index > 0)
                        {
                            elements = elements.RemoveAt(index).Insert(index - 1, elements[index]);
                            modified = true;
                        }
                    }
                    ImGuiHelpers.HelpTooltip("Move up");

                    if (ImGuiHelpers.IconButton('', $"{member.Name}_move_down", Game.Data.GameProfile.Theme.HighAccent))
                    {
                        if (index < elements.Length - 1)
                        {
                            elements = elements.RemoveAt(index).Insert(index + 1, elements[index]);
                            modified = true;
                        }
                    }
                    ImGuiHelpers.HelpTooltip("Move down");

                    ImGui.EndGroup();
                    if (!ImGui.IsWindowAppearing() && ImGui.IsWindowFocused() && !ImGui.IsMouseHoveringRect(ImGui.GetWindowPos(), ImGui.GetWindowPos() + ImGui.GetWindowSize()))
                        ImGui.CloseCurrentPopup();
                    ImGui.EndPopup();
                }
                
                ImGui.SameLine();

                if (DrawElement(ref element, member, index))
                {
                    elements = elements.SetItem(index, element!);
                    modified = true;
                }

                ImGui.EndGroup();
                ImGui.PopID();
            }
            return (modified, elements);
        }

        protected virtual bool DrawElement(ref T? element, EditorMember member, int index)
        {
            return CustomComponent.ShowEditorOf(ref element);
        }
    }
}
