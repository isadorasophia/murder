using Bang;
using Bang.Components;
using Bang.Entities;
using ImGuiNET;
using Murder.Components;
using Murder.Core.Input;
using Murder.Diagnostics;
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
                if (EntityServices.TryGetEntityName(entity) is string entityName)
                {
                    StartEntityTree(world, entity, entityName, false);
                }
                else
                {
                    StartEntityTree(world, entity, "Unnamed Entity", false);
                }

                DrawInspectorCore(world, entity);

            }
            ImGui.End();

            return isOpen;
        }

        private static void DrawInspectorCore(World world, Entity entity)
        {
            foreach (IComponent c in entity.Components)
            {
                string componentName = ReflectionHelper.GetGenericName(c.GetType());

                if (ImGui.TreeNode($"{componentName}##Component_inspector_{componentName}"))
                {
                    bool succeededCopy = true;

                    // This is modifying the memory of all readonly structs, so only create a copy if this 
                    // is not a modifiable component.
                    IComponent copy;
                    try
                    {
                        copy = c is IModifiableComponent ? c : SerializationHelper.DeepCopy(c);
                    }
                    catch (NotSupportedException)
                    {
                        // We might not support deep copying some runtime fields.
                        // This is probably okay because we won't serialize this anyway in real world.
                        copy = c;
                        succeededCopy = false;
                    }

                    if (CustomComponent.ShowEditorOf(ref copy))
                    {
                        if (!succeededCopy)
                        {
                            GameLogger.Warning("Modifying field that is not supported to be serialized!");
                        }

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
                    if (StartEntityTree(world, child, childId.Value ?? "Unnamed Entity", true))
                    {
                        DrawInspectorCore(world, child);
                        ImGui.TreePop();
                    }
                }
            }
        }

        private static bool StartEntityTree(World world, Entity entity, string entityName, bool pushTree)
        {
            ImGui.TextColored(Game.Profile.Theme.Faded, $"{entity.EntityId}:");
            ImGui.SameLine();

            bool isEntityActive = entity.IsActive;
            if (ImGui.Checkbox($"##Entity_Inspector_Checkbox_{entity.EntityId}", ref isEntityActive))
            {
                if (isEntityActive)
                {
                    entity.Activate();
                }
                else
                {
                    entity.Deactivate();
                }
            }
            ImGui.SameLine();


            if (world.TryGetUniqueEntityCameraFollow() is not Entity cameraMan)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.Faded);
                ImGui.Button("");
                ImGui.PopStyleColor();
                ImGui.SameLine();
            }
            else
            {
                if (cameraMan.HasIdTarget())
                {
                    ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.Green);
                    if (ImGui.Button(""))
                    {
                        cameraMan.RemoveIdTarget();
                    }
                    ImGuiHelpers.HelpTooltip("Recover camera");
                }
                else
                {
                    ImGui.PushStyleColor(ImGuiCol.Button, Game.Profile.Theme.Faded);
                    if (ImGui.Button(""))
                    {
                        cameraMan.SetIdTarget(entity.EntityId);
                    }
                    ImGuiHelpers.HelpTooltip("Center camera on entity");

                }
                ImGui.PopStyleColor();
                ImGui.SameLine();

                if (entity.Parent == null)
                {
                    ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Game.Profile.Theme.Red);
                    if (ImGui.Button(""))
                    {
                        entity.Destroy();
                    }
                    ImGui.PopStyleColor();

                    ImGui.SameLine();
                }
            }

            if (pushTree)
            {
                return ImGui.TreeNode($"{entityName}##Entity_Inspector_{entity.EntityId}");
            }
            else
            {
                ImGui.Text(entityName);
                return false;
            }
        }
    }
}