using System.Collections.Immutable;
using InstallWizard.Util;
using Editor.Reflection;
using ImGuiNET;
using InstallWizard.Core.Helpers;
using InstallWizard;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<Directions>))]
    internal class ImmutableDirectionsField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            ImmutableArray<Directions> collection = (ImmutableArray<Directions>)fieldValue!;

            if (collection.IsDefault)
            {
                // We do not support unitialized arrays, create an empty array.
                return (true, ImmutableArray<Directions>.Empty);
            }

            if (collection.IsEmpty)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "Empty");
            }
            else
            {
                int toRemove = -1;

                for (int i = 0; i < collection.Length; i++)
                {
                    var direction = collection[i];
                    if (i>0) ImGui.SameLine();
                    
                    ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.Bg);
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Game.Profile.Theme.Red);
                    if (ImGuiExtended.IconButton(direction.ToIcon(), $"{member.Name}_remove_{i}"))
                    {
                        toRemove = i;
                    }
                    ImGui.PopStyleColor(2);
                }

                if (toRemove >= 0)
                {
                    collection = collection.RemoveAt(toRemove);
                    modified=true;
                }
            }

            if (ImGui.ArrowButton($"{member.Name}_add_left",ImGuiDir.Left))
            {
                collection = collection.Add(Directions.Left);
                modified = true;
            }
            ImGui.SameLine();

            if (ImGui.ArrowButton($"{member.Name}_add_up",ImGuiDir.Up))
            {
                collection = collection.Add(Directions.Up);
                modified = true;
            }
            ImGui.SameLine();

            if (ImGui.ArrowButton($"{member.Name}_add_down",ImGuiDir.Down))
            {
                collection = collection.Add(Directions.Down);
                modified = true;
            }
            ImGui.SameLine();

            if (ImGui.ArrowButton($"{member.Name}_add_right",ImGuiDir.Right))
            {
                collection = collection.Add(Directions.Right);
                modified = true;
            }

            return (modified, collection);
        }
    }
}
