using Bang.Components;
using Bang.StateMachines;
using ImGuiNET;
using Murder.Components;
using Murder.Components.Serialization;
using Murder.Diagnostics;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Editor.CustomEditors
{
    internal partial class WorldAssetEditor
    {
        protected virtual bool DrawStoryEditor(Stage stage)
        {
            if (_world is null)
            {
                return false;
            }

            GameLogger.Verify(_world is not null);

            bool modified = false;

            int dockShowEntitiesSize = 400 - 5;

            ImGui.BeginChild("##story_entities_child", new System.Numerics.Vector2(-1, dockShowEntitiesSize));

            modified |= DrawCutsceneEntities(stage);
            modified |= DrawStoryEntities(stage);

            ImGui.EndChild();

            bool showOpenedEntities = stage.EditorHook.AllOpenedEntities.Length > 0;
            if (showOpenedEntities)
            {
                float height = ImGui.GetContentRegionMax().Y - 10;
                float dockOpenedEntitiesSize = height - dockShowEntitiesSize;

                uint dockId = 555;

                ImGui.BeginChild("##DockArea Selected Entity", new System.Numerics.Vector2(-1, dockOpenedEntitiesSize), ImGuiChildFlags.None);
                ImGui.DockSpace(dockId);
                ImGui.EndChild();

                if (_selectedAsset is Guid selectedGuid && _world?.TryGetInstance(selectedGuid) is EntityInstance instance)
                {
                    DrawInstanceWindow(dockId, stage, instance);
                }

                foreach ((int openedInstanceId, _) in stage.EditorHook.AllSelectedEntities)
                {
                    if (stage.FindInstance(openedInstanceId) is EntityInstance e)
                    {
                        DrawInstanceWindow(dockId, stage, e, openedInstanceId);
                    }
                }

                _selecting = -1;
            }

            return modified;
        }

        protected virtual bool DrawCutsceneEntities(Stage stage)
        {
            bool modified = false;
            if (TreeEntityGroupNode("Cutscenes", Game.Profile.Theme.White, icon: '\ue131'))
            {
                IList<IEntity> cutscenes = stage.FindEntitiesWith(typeof(CutsceneAnchorsEditorComponent));
                modified |= DrawCutsceneEntities(stage, cutscenes);

                ImGui.TreePop();
            }

            return modified;
        }

        protected virtual bool DrawCutsceneEntities(Stage stage, IList<IEntity> cutscenes)
        {
            bool modified = false;

            if (ImGui.Button("New cutscene!"))
            {
                EntityInstance empty = EntityBuilder.CreateInstance(Guid.Empty, "Cutscene");

                empty.AddOrReplaceComponent(new CutsceneAnchorsEditorComponent());
                empty.AddOrReplaceComponent(new PositionComponent());

                AddInstance(empty);
                modified = true;
            }

            foreach (IEntity cutscene in cutscenes)
            {
                if (TreeEntityGroupNode($"{cutscene.Name}##{cutscene.Guid}", Game.Profile.Theme.White, icon: '\ue131'))
                {
                    if (ImGuiHelpers.DeleteButton($"Delete#{cutscene.Guid}"))
                    {
                        DeleteInstance(parent: null, cutscene.Guid);
                        modified = true;
                    }

                    ImGui.SameLine();

                    // Do not modify the name for entity assets, only instances.
                    if (ImGuiHelpers.IconButton('\uf304', $"rename_{cutscene.Guid}"))
                    {
                        ImGui.OpenPopup($"Rename#{cutscene.Guid}");
                    }

                    if (ImGui.BeginPopup($"Rename#{cutscene.Guid}"))
                    {
                        if (DrawRenameInstanceModal(parent: null, cutscene))
                        {
                            ImGui.CloseCurrentPopup();
                        }

                        ImGui.EndPopup();
                    }

                    // Draw components!
                    var components = GetComponents(parent: null, cutscene);
                    foreach (IComponent c in components)
                    {
                        Type t = c.GetType();

                        if (ImGui.TreeNodeEx(ReflectionHelper.GetGenericName(t)))
                        {
                            // We only support replacing the state machine.
                            if (t.IsAssignableTo(typeof(IStateMachineComponent)) &&
                                ImGuiHelpers.DeleteButton($"Delete_{t}"))
                            {
                                RemoveComponent(parent: null, cutscene, t);
                            }

                            // TODO: This is modifying the memory of all readonly structs.
                            IComponent copy = SerializationHelper.DeepCopy(c);

                            if (CustomComponent.ShowEditorOf(copy))
                            {
                                // Asset was already modified, just pass along the updated asset.
                                ReplaceComponent(null, cutscene, copy);
                            }

                            ImGui.TreePop();
                        }
                    }

                    if (!cutscene.HasComponent(typeof(IStateMachineComponent)))
                    {
                        ImGui.Dummy(new Vector2(15 /* padding */ / 2f, 0));
                        ImGui.SameLine();

                        if (SearchBox.SearchStateMachines(initialValue: null, out Type? sm) && sm is not null)
                        {
                            AddComponent(null, cutscene, sm);
                        }
                    }

                    ImGui.TreePop();
                }
            }

            return modified;
        }

        protected virtual bool DrawStoryEntities(Stage stage)
        {
            if (TreeEntityGroupNode("Story Entities", Game.Profile.Theme.Yellow, icon: '\uf236'))
            {
                List<IEntity> entities = stage.FindEntitiesWithAttribute<StoryAttribute>();
                DrawEntityList(stage, entities, filterGroup: null);

                ImGui.TreePop();
            }

            return false;
        }

        /// <param name="entities">Entities that belong to the group.</param>
        /// <param name="filterGroup">This has a value when displaying a specific filter of entities.</param>
        private void DrawEntityList(Stage stage, List<IEntity> entities, string? filterGroup)
        {
            GameLogger.Verify(_asset is not null);

            if (filterGroup is not null)
            {
                if (ImGuiHelpers.DeleteButton($"Delete_group_{filterGroup}"))
                {
                    RenameFilter(filterGroup, newName: null);
                }

                ImGuiHelpers.HelpTooltip("Delete filter (entities will lose the filter)");

                ImGui.SameLine();

                string popupName = $"Rename group##{filterGroup}";
                if (ImGuiHelpers.IconButton('\uf304', $"rename_group_{filterGroup}"))
                {
                    _groupName = filterGroup;

                    ImGui.OpenPopup(popupName);
                }

                ImGuiHelpers.HelpTooltip("Rename filter");

                DrawCreateOrRenameFilterPopup(popupName, previousName: filterGroup);
            }

            if (filterGroup is not null && entities.Count == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "Drag an entity here!");

                if (DragDrop<Guid>.DragDropTarget($"drag_instance", out Guid draggedId))
                {
                    MoveToFilter(filterGroup, draggedId, position: -1);
                }

                return;
            }

            // Whether this is skipping entities which are already on a filter.
            // This is false when already displaying a filter group of entities.
            bool skipIfOnFilter = filterGroup is null;

            for (int i = 0; i < entities.Count; ++i)
            {
                IEntity entity = entities[i];
                if (skipIfOnFilter && _world is not null && _world.IsOnFilter(entity.Guid))
                {
                    continue;
                }

                string? name = entity.Name ?? "Instance (no name)";

                if (!string.IsNullOrEmpty(_searchInstanceText))
                {
                    bool matchName = name.Contains(_searchInstanceText, StringComparison.OrdinalIgnoreCase);
                    if (!matchName)
                    {
                        continue;
                    }
                }

                if (ImGuiHelpers.DeleteButton($"Delete_{entity.Guid}"))
                {
                    DeleteInstance(parent: null, entity.Guid);

                    continue;
                }

                ImGui.SameLine();

                ImGui.PushID($"Entity_bar_{entity.Guid}");

                bool isSelected = stage.IsSelected(entity.Guid);

                if (ImGui.Selectable(name, isSelected))
                {
                    _selecting = stage.SelectEntity(entity.Guid, select: true, clear: true);
                    if (_selecting is -1)
                    {
                        // Unable to find the entity. This probably means that it has been deactivated.
                        _selectedAsset = entity.Guid;
                    }
                    else
                    {
                        _selectedAsset = null;
                    }
                }

                DragDrop<Guid>.DragDropSource("drag_instance", name, entity.Guid);

                ImGui.PopID();

                if (DragDrop<Guid>.DragDropTarget($"drag_instance", out Guid draggedId))
                {
                    _world?.MoveToFilter(filterGroup, draggedId, i);
                }
            }
        }

        private void MoveToFilter(string? targetGroup, Guid instance, int position = -1)
        {
            GameLogger.Verify(_asset is not null);

            _world?.MoveToFilter(targetGroup, instance, position);
        }
    }
}