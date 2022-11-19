using ImGuiNET;
using System.Collections.Immutable;
using Murder.Assets;
using Murder.Prefabs;
using Murder.ImGuiExtended;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.Stages;
using Bang.Components;
using Murder.Editor.Utilities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(WorldAsset))]
    internal partial class WorldAssetEditor : AssetEditor
    {
        private WorldAsset? _world;

        private bool _assetWindowOpen = true;
        private int _selecting;

        private Guid? _selectedAsset;

        protected virtual EntityInstance? TryFindInstance(Guid guid) => _world?.TryGetInstance(guid);

        protected virtual ImmutableArray<Guid> Instances => _world?.Instances ?? ImmutableArray<Guid>.Empty;

        protected virtual bool ShouldDrawSystems => true;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, object target)
        {
            _asset = (GameAsset)target;
            _world = (WorldAsset)target;

            _world.ValidateInstances();

            if (!Stages.ContainsKey(_asset.Guid))
            {
                InitializeStage(new(imGuiRenderer, _world), _asset.Guid);
            }
        }

        protected override void InitializeStage(Stage stage, Guid guid)
        {
            base.InitializeStage(stage, guid);

            Stages[guid].EditorHook.AddPrefabWithStage += AddEntityFromWorld;
            Stages[guid].EditorHook.AddEntityWithStage += AddEntityFromWorld;
            Stages[guid].EditorHook.RemoveEntityWithStage += DeleteEntityFromWorld;
        }

        public override async ValueTask DrawEditor()
        {
            GameLogger.Verify(Stages.ContainsKey(_asset!.Guid));

            Stage currentStage = Stages[_asset.Guid];

            if (ImGui.BeginTable("world table", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, 380, 0);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1f, 1);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                if (ImGui.BeginTabBar("WorldBar"))
                {
                    if (ImGui.BeginTabItem("World"))
                    {
                        int dockShowEntitiesSize = 400 - 5.WithDpi();

                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                        ImGui.BeginChild("world_child", new System.Numerics.Vector2(-1, dockShowEntitiesSize));

                        currentStage.ActivateSystemsWith(enable: true, typeof(WorldEditorAttribute));

                        DrawEntitiesEditor();

                        ImGui.EndChild();
                        ImGui.PopStyleColor();

                        ImGui.EndTabItem();

                        bool showOpenedEntities = currentStage.EditorHook.AllOpenedEntities.Length > 0;

                        float height = ImGui.GetContentRegionMax().Y - 100.WithDpi();
                        float dockSelectEntitiesSize = showOpenedEntities ? height / 4 : height / 2;
                        float dockOpenedEntitiesSize = height - dockShowEntitiesSize - dockSelectEntitiesSize;

                        ImGui.BeginChild("##DockArea New Entity", new System.Numerics.Vector2(-1, dockSelectEntitiesSize), false, ImGuiWindowFlags.HorizontalScrollbar);
                        ImGui.DockSpace(id: 669);
                        ImGui.EndChild();

                        DrawAllInstancesToAdd(id: 669, currentStage.EditorHook);

                        if (currentStage.EditorHook.AllOpenedEntities.Length > 0)
                        {
                            ImGui.BeginChild("##DockArea Selected Entity", new System.Numerics.Vector2(-1, dockOpenedEntitiesSize), false);
                            ImGui.DockSpace(id: 666);
                            ImGui.EndChild();

                            if (_selectedAsset is Guid selectedGuid && _world?.TryGetInstance(selectedGuid) is EntityInstance instance)
                            {
                                DrawInstanceWindow(currentStage, instance);
                            }

                            foreach ((int openedInstanceId, _) in currentStage.EditorHook.AllSelectedEntities)
                            {
                                if (currentStage.FindInstance(openedInstanceId) is EntityInstance e)
                                {
                                    DrawInstanceWindow(currentStage, e, openedInstanceId);
                                }
                            }
                        }

                    }
                    else
                    {
                        currentStage.ActivateSystemsWith(enable: false, typeof(WorldEditorAttribute));
                    }

                    if (ShouldDrawSystems)
                    {
                        if (ImGui.BeginTabItem("Systems"))
                        {
                            ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                            ImGui.BeginChild("systems_child", ImGui.GetContentRegionAvail()
                                - new System.Numerics.Vector2(0, 5) * Architect.Instance.DPIScale / 100f);

                            DrawSystemsEditor();

                            ImGui.EndChild();
                            ImGui.PopStyleColor();

                            ImGui.EndTabItem();
                        }
                    }

                    if (ImGui.BeginTabItem("Tile Editor"))
                    {
                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                        ImGui.BeginChild("tile_editor_child", ImGui.GetContentRegionAvail()
                            - new System.Numerics.Vector2(0, 5) * Architect.Instance.DPIScale / 100f);

                        currentStage.ActivateSystemsWith(enable: true, typeof(TileEditorAttribute));
                        _asset.FileChanged |= DrawTileEditor(currentStage);

                        ImGui.EndChild();
                        ImGui.PopStyleColor();

                        ImGui.EndTabItem();
                    }
                    else
                    {
                        currentStage.ActivateSystemsWith(enable: false, typeof(TileEditorAttribute));
                    }

                    ImGui.EndTabBar();
                }

                ImGui.TableNextColumn();

                await currentStage.Draw();

                ImGui.EndTable();
            }
        }

        private void DrawInstanceWindow(Stage stage, EntityInstance instance, int selected = -1)
        {
            GameLogger.Verify(_asset is not null);

            ImGui.SetNextWindowBgAlpha(0.75f);

            bool inspectingWindowOpen = true;

            ImGui.SetNextWindowDockID(666, ImGuiCond.Appearing);
            _assetWindowOpen = ImGui.Begin($"{instance.Name}##Instance_Editor_{instance.Guid}", ref inspectingWindowOpen);

            if (_selecting != -1 && _selecting == selected)
            {
                ImGui.SetWindowFocus();
            }

            if (_assetWindowOpen)
                DrawEntity(instance);

            ImGui.End();

            if (!inspectingWindowOpen)
            {
                stage.SelectEntity(selected, select: false);
            }
        }

        protected virtual bool CanAddInstance => true;

        protected virtual void AddInstance(EntityInstance instance)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));
            GameLogger.Verify(_world is not null);

            _world.AddInstance(instance);
            Stages[_asset.Guid].AddEntity(instance);

            _world.FileChanged = true;
        }

        protected override void DeleteInstance(IEntity? parent, Guid instanceGuid)
        {
            // Delete the instance and then track it down in the world.
            base.DeleteInstance(parent, instanceGuid);

            _world?.RemoveInstance(instanceGuid);
        }

        private void DrawSystemsEditor()
        {
            GameLogger.Verify(_world is not null);

            if (FeatureAssetEditor.DrawSystemsEditor(_world.Systems, _world.FetchAllSystems(), out var updatedSystems))
            {
                _world.UpdateSystems(updatedSystems);
            }

            if (FeatureAssetEditor.DrawFeaturesEditor(_world.Features, out var updatedFeatures))
            {
                _world.UpdateFeatures(updatedFeatures);
            }
        }

        protected virtual void AddEntityFromWorld(Guid asset, IComponent[] components)
        {
            GameLogger.Verify(_world is not null && _asset is not null && Stages.ContainsKey(_asset.Guid));

            _asset.FileChanged = true;

            EntityInstance instance = EntityBuilder.CreateInstance(asset);
            foreach (IComponent c in components)
            {
                // Override components specified in constructor.
                instance.AddOrReplaceComponent(c);
            }

            // Add instance to the world instance.
            AddInstance(instance);
        }

        protected virtual void AddEntityFromWorld(IComponent[] components)
        {
            GameLogger.Verify(_world is not null && _asset is not null && Stages.ContainsKey(_asset.Guid));

            _asset.FileChanged = true;

            // We need to do fancier stuff for tile grid entities.
            bool isTilegrid = false;

            EntityInstance empty = EntityBuilder.CreateInstance(Guid.Empty);
            foreach (IComponent c in components)
            {
                empty.AddOrReplaceComponent(c);

                if (c is TileGridComponent)
                {
                    isTilegrid = true;
                }
            }
            
            // Add instance to the world instance.
            AddInstance(empty);

            string name = "Instance";

            // If this is a tile grid, create a new room for it.
            if (isTilegrid && AddGroup("Room") is string roomName)
            {
                name = roomName;

                MoveToGroup(roomName, empty.Guid);
            }
            
            empty.SetName(name);
        }

        protected virtual void DeleteEntityFromWorld(int id)
        {
            GameLogger.Verify(_world is not null && _asset is not null && Stages.ContainsKey(_asset.Guid));

            _asset.FileChanged = true;

            IEntity? e = Stages[_asset.Guid]?.FindInstance(id);
            if (e is null)
            {
                GameLogger.Error($"Unable to remove entity {id} from world.");
                return;
            }
            
            // TODO: Support removing children?
            DeleteInstance(parent: null, e.Guid);
        }
        
        protected override void OnEntityModified(int entityId, IComponent c)
        {
            GameLogger.Verify(_asset is not null);

            Type tTileGrid = typeof(TileGridComponent);
            
            // First, we need to check if this is actually a tile grid component.
            if (c is TileGridComponent tileGrid)
            {
                TileGridComponent previousTileGrid;
                string? groupForTilegrid;
                
                Stage stage = Stages[_asset.Guid];
                if (stage.FindInstance(entityId) is IEntity entity && entity.HasComponent(tTileGrid))
                {
                    previousTileGrid = (TileGridComponent)entity.GetComponent(tTileGrid);
                    groupForTilegrid = _world?.GetGroupOf(entity.Guid);

                    // Only move entities if the room belongs to an actual group.
                    if (groupForTilegrid is not null)
                    {
                        MoveGroupOfEntities(
                            entity.Guid, from: previousTileGrid.Origin, to: tileGrid.Origin, groupForTilegrid);
                    }
                }
                else
                {
                    GameLogger.Warning("We do not support moving rooms as children just yet.");
                }
            }
            
            base.OnEntityModified(entityId, c);
        }

        /// <summary>
        /// This will group all the root entities within a room to a delta position from <paramref name="from"/> to <paramref name="to"/>.
        /// </summary>
        /// <param name="roomEntity">Entity that corresponds to the tile grid component.</param>
        /// <param name="from">Previous position.</param>
        /// <param name="to">Target position.</param>
        /// <param name="group">Group that this room belongs.</param>
        private void MoveGroupOfEntities(Guid roomEntity, Point from, Point to, string group)
        {
            GameLogger.Verify(_world is not null);

            Point worldDelta = (to - from) * Grid.CellSize;
            
            foreach (Guid guid in _world.FetchEntitiesOfGroup(group))
            {
                if (guid == roomEntity)
                {
                    // Skip room entity itself.
                    continue;
                }
                
                if (_world?.TryGetInstance(guid) is not IEntity entity)
                {
                    // Entity is not valid?
                    continue;
                }

                if (!entity.HasComponent(typeof(ITransformComponent)))
                {
                    // Entity doesn't really have a transform component to move.
                    continue;
                }
                
                IMurderTransformComponent? transform = 
                    (IMurderTransformComponent)entity.GetComponent(typeof(IMurderTransformComponent));

                ReplaceComponent(parent: null, entity, transform.Add(worldDelta));
            }
        }
    }
}