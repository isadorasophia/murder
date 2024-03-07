using Bang;
using Bang.Components;
using Bang.Entities;
using ImGuiNET;
using Murder.Components;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Prefabs;
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

                ImGui.SeparatorText("Debug Tools");
                if (ImGui.Button("Send Message"))
                {
                    ImGui.OpenPopup("Send Message");
                }
                if (ImGui.BeginPopup("Send Message"))
                {

                    ImGui.EndPopup();
                }

                if (ImGui.Button("Add Component"))
                {
                    ImGui.OpenPopup("Add Component");
                }
                if (ImGui.BeginPopup("Add Component"))
                {
                    Type? componentType = SearchBox.SearchComponent(entity.Components);
                    if (componentType is not null)
                    {
                        if (Activator.CreateInstance(componentType) is IComponent component)
                        {
                            entity.AddComponent(component, componentType);
                        }
                    }

                    ImGui.EndPopup();
                }

            }
            ImGui.End();

            return isOpen;
        }
    }
}