using ImGuiNET;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    public abstract class ImmutableArrayField<T> : CustomField
    {
        private int _draggedIndex = -1;
        private string _draggedId = string.Empty;

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

            int maxLength = 128;
            string id = $"{member.Member.ReflectedType}";
            ImGui.BeginGroup();
            ImGui.Dummy(new System.Numerics.Vector2(2, 2));
            ImGui.SameLine();
            ImGui.BeginGroup();
            ImGui.Dummy(new System.Numerics.Vector2(2, 2));
            for (int index = 0; index < Math.Min(maxLength, elements.Length); index++)
            {
                
                ImGui.PushID($"{id}_{index}");
                element = elements[index];

                ImGui.BeginGroup();
                {
                    ImGuiHelpers.IconButton('\uf0c9', $"{member.Name}_alter", Game.Data.GameProfile.Theme.Bg);

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.OpenPopup($"{member.Member.ReflectedType}_{index}_extras");
                        ImGui.SetNextWindowPos(ImGui.GetItemRectMin() + new System.Numerics.Vector2(-12, -2));
                    }
                    ImGui.SameLine();

                    DrawDragHandle(ref modified, ref elements, id, index);

                    if (ImGui.BeginPopup($"{member.Member.ReflectedType}_{index}_extras"))
                    {
                        ImGui.BeginGroup();
                        if (ImGuiHelpers.IconButton('', $"{member.Name}_remove", Game.Data.GameProfile.Theme.Bg))
                        {
                            elements = elements.RemoveAt(index);
                            modified = true;
                        }
                        ImGuiHelpers.HelpTooltip("Remove");
                        ImGui.SameLine();

                        if (ImGuiHelpers.IconButton('', $"{member.Name}_duplicate", Game.Data.GameProfile.Theme.Bg))
                        {
                            elements = elements.Insert(index, elements[index]);
                            modified = true;
                        }
                        ImGuiHelpers.HelpTooltip("Duplicate");

                        ImGui.EndGroup();
                        if (!ImGui.IsWindowAppearing() && ImGui.IsWindowFocused() && !ImGui.IsMouseHoveringRect(ImGui.GetWindowPos(), ImGui.GetWindowPos() + ImGui.GetWindowSize()))
                            ImGui.CloseCurrentPopup();
                        ImGui.EndPopup();
                    }

                    if (DrawElement(ref element, member, index))
                    {
                        elements = elements.SetItem(index, element!);
                        modified = true;
                    }
                }
                ImGui.EndGroup();
                ImGui.PopID();
            }
            ImGui.Dummy(new System.Numerics.Vector2(2,2));
            ImGui.EndGroup();
            ImGui.SameLine();
            ImGui.Dummy(new System.Numerics.Vector2(2, 2));

            ImGui.EndGroup();
            ImGuiHelpers.DrawBorderOnPreviousItem(Game.Profile.Theme.BgFaded, 1);

            if (elements.Length >= maxLength)
            {
                ImGui.Text($"List is too long ({elements.Length} items hidden)...");
            }
            return (modified, elements);
        }

        private void DrawDragHandle(ref bool modified, ref ImmutableArray<T> elements, string id, int index)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, System.Numerics.Vector4.Zero);
            ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Faded);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, System.Numerics.Vector4.Zero);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, System.Numerics.Vector4.Zero);
            ImGui.Button("\uf256");
            ImGui.PopStyleColor(4);
            ImGuiHelpers.HelpTooltip("Drag to reorder");

            if (ImGui.BeginDragDropSource())
            {
                ImGui.SetDragDropPayload("Child", IntPtr.Zero, 0);
                _draggedIndex = index;
                _draggedId = id;

                ImGui.Text($"Moving '{id}' element...");
                ImGui.EndDragDropSource();
            }

            if (_draggedId.Equals(id) && ImGui.BeginDragDropTarget())
            {
                ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload("Child");

                bool hasDropped;
                unsafe
                {
                    hasDropped = payload.NativePtr != null;
                }

                if (hasDropped)
                {
                    // Make sure we can insert the dragged element at the new index.
                    if (elements.Length > _draggedIndex && elements.Length>index)
                    {
                        elements = elements.RemoveAt(_draggedIndex).Insert(index, elements[_draggedIndex]);
                        modified = true;
                    }
                }

                ImGui.EndDragDropTarget();
            }

            ImGui.SameLine();
        }

        protected virtual bool DrawElement(ref T? element, EditorMember member, int index)
        {
            bool modified = false;

            if (DrawValue(member.CreateFrom(typeof(T), "Value", element: default), element, out T? modifiedValue))
            {
                element = modifiedValue;
                modified = true;
            }

            return modified;
        }
    }
}