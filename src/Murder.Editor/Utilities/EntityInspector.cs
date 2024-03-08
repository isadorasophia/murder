using Bang;
using Bang.Components;
using Bang.Entities;
using ImGuiNET;
using Murder.Components;
using Murder.Core.Input;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Prefabs;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Editor.Utilities
{
    internal static class EntityInspector
    {
        public static bool DrawInspector(World world, Entity entity)
        {
            bool isOpen = true;

            if (ImGui.Begin($"{entity.EntityId}##Entity_Inspector", ref isOpen))
            {
                var cameraMan = world.GetUniqueEntity<CameraFollowComponent>();
                if (cameraMan.HasIdTarget())
                {
                    if (ImGui.SmallButton("release camera"))
                    {
                        cameraMan.RemoveIdTarget();
                    }
                }
                else if (ImGui.SmallButton("focus"))
                {
                    cameraMan.SetIdTarget(entity.EntityId);
                }


                ImGui.TextColored(Game.Profile.Theme.Faded, $"{entity.EntityId}:");
                ImGui.SameLine();
                if (EntityServices.TryGetEntityName(entity) is string entityName)
                {
                    ImGui.TextColored(Game.Profile.Theme.HighAccent, entityName);
                }
                else
                {
                    ImGui.TextColored(Game.Profile.Theme.Faded, "Unnamed Enitity");
                }

                DrawInpsectorCore(world, entity);

            }
            ImGui.End();

            return isOpen;
        }

        private static void DrawInpsectorCore(World world, Entity entity)
        {
            foreach (IComponent c in entity.Components)
            {

                if (ImGui.TreeNode($"{c.GetType().Name}##Component_inspector_{c.GetType().Name}"))
                {
                    // This is modifying the memory of all readonly structs, so only create a copy if this 
                    // is not a modifiable component.
                    IComponent copy = c is IModifiableComponent ? c : SerializationHelper.DeepCopy(c);
                    if (CustomComponent.ShowEditorOf(ref copy))
                    {
                        // This will trigger reactive systems.
                        entity.ReplaceComponent(copy, copy.GetType());
                    }

                    ImGui.TreePop();
                }
            }

            Type? componentType = SearchBox.SearchComponent(entity.Components);
            if (componentType is not null)
            {
                if (Activator.CreateInstance(componentType) is IComponent component)
                {
                    entity.AddComponent(component, componentType);
                }
            }

            ImGui.SeparatorText("Children");


            foreach (var childId in entity.FetchChildrenWithNames)
            {
                if (world.TryGetEntity(childId.Key) is Entity child)
                {
                    ImGui.TextColored(Game.Profile.Theme.Faded, $"{childId.Key}:");
                    ImGui.SameLine();

                    if (ImGui.TreeNode($"{childId.Value}##Entity_Inspector_{child.EntityId}"))
                    {
                        DrawInpsectorCore(world, child);
                        ImGui.TreePop();
                    }
                }
            }
        }
    }
}