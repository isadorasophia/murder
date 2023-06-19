using Bang.Components;
using Bang.StateMachines;
using ImGuiNET;
using Murder.Components;
using Murder.Components.Cutscenes;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using Murder.Utilities;
using Murder.Utilities.Attributes;

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

                ImGui.BeginChild("##DockArea Selected Entity", new System.Numerics.Vector2(-1, dockOpenedEntitiesSize), false);
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
                IList<IEntity> cutscenes = stage.FindEntitiesWith(typeof(CutsceneAnchorsComponent));
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

                empty.AddOrReplaceComponent(new CutsceneAnchorsComponent());
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

                        if (SearchBox.SearchStateMachines() is Type sm)
                        {
                            AddComponent(null, cutscene, sm);
                        }

                        ImGui.TreePop();
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
                IList<IEntity> entities = stage.FindEntitiesWithAttribute<StoryAttribute>();
                DrawEntityList(stage, entities);

                ImGui.TreePop();
            }

            return false;
        }

        /// <summary>
        /// Draw the entity list of <paramref name="group"/>.
        /// </summary>
        /// <param name="group">Group unique name. None if it doesn't belong to any.</param>
        /// <param name="entities">Entities that belong to the group.</param>
        private void DrawEntityList(Stage stage, IList<IEntity> entities)
        {
            GameLogger.Verify(_asset is not null);

            if (entities.Count == 0)
            {
                return;
            }

            for (int i = 0; i < entities.Count; ++i)
            {
                IEntity entity = entities[i];

                if (ImGuiHelpers.DeleteButton($"Delete_{entity}"))
                {
                    DeleteInstance(parent: null, entity.Guid);

                    continue;
                }

                ImGui.SameLine();

                ImGui.PushID($"Entity_bar_{entity}");

                bool isSelected = stage.IsSelected(entity.Guid);

                string? name = entity.Name;
                if (ImGui.Selectable(name ?? "<?>", isSelected))
                {
                    _selecting = stage.SelectEntity(entity.Guid, select: true);
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
            }
        }
    }
}