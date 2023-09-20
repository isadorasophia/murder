﻿using ImGuiNET;
using System.Collections.Immutable;
using Murder.Assets;
using Murder.Prefabs;
using Murder.Editor.ImGuiExtended;
using Murder.Diagnostics;
using Murder.Attributes;
using Murder.Editor.Attributes;
using Murder.Editor.Stages;
using Bang.Components;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Editor.CustomFields;
using Murder.Utilities;
using Murder.Core.Graphics;
using Murder.Editor.CustomDiagnostics;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(WorldAsset))]
    internal partial class WorldAssetEditor : AssetEditor
    {
        private WorldAsset? _world;

        private bool _showPuzzles = false;

        private bool _assetWindowOpen = true;
        private int _selecting;
        private readonly int[] _moveRoomAmount = new int[2];

        private Guid? _selectedAsset;

        public virtual EntityInstance? TryFindInstance(Guid guid) => _world?.TryGetInstance(guid);

        protected virtual ImmutableArray<Guid> Instances => _world?.Instances ?? ImmutableArray<Guid>.Empty;

        protected virtual bool ShouldDrawSystems => true;

        protected override void OnSwitchAsset(ImGuiRenderer imGuiRenderer, RenderContext renderContext, bool forceInit)
        {
            _world = (WorldAsset)_asset!;
            _world.ValidateInstances();

            if (!Stages.TryGetValue(_world.Guid, out Stage? stage) || stage.AssetReference != _world)
            {
                GameLogger.Verify(stage is null || 
                    stage.AssetReference != _world, "Why are we replacing the asset reference? Call isa to debug this! <3");
                
                InitializeStage(new(imGuiRenderer, renderContext, _world), _world.Guid);
            }

            // Disable custom shaders on prefab editors.
            renderContext.SwitchCustomShader(enable: Architect.EditorSettings.UseCustomShadersOnEditor);

            // Clear cache.
            _entitiesPerGroup.Clear();
        }

        protected override void InitializeStage(Stage stage, Guid guid)
        {
            base.InitializeStage(stage, guid);

            Stages[guid].EditorHook.AddPrefabWithStage += AddEntityFromWorld;
            Stages[guid].EditorHook.AddEntityWithStage += AddEntityFromWorld;
            Stages[guid].EditorHook.RemoveEntityWithStage += DeleteEntityFromWorld;
        }

        private IEntity? _openedEntity = null;

        public override IEntity? SelectedEntity => _world is null ? null : _openedEntity;

        float _entitiesEditorSize = 100;
        float _entitiesPickerSize = 100;
        float _entityInspectorSize = -1;

        public override void DrawEditor()
        {
            GameLogger.Verify(Stages.ContainsKey(_asset!.Guid));

            Stage currentStage = Stages[_asset.Guid];
            bool modified = false;

            if (ImGui.BeginTable("world table", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, 380, 0);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1f, 1);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                if (ImGui.BeginTabBar("WorldBar"))
                {
                    if (ImGui.BeginTabItem($"{Icons.World} World"))
                    {
                        ImGui.BeginChild("world_tab_container", new System.Numerics.Vector2(-1, -1), false, ImGuiWindowFlags.NoResize);

                        ImGuiHelpers.DrawSplitter("##splitter_world_tab_1",true, 8, ref _entitiesEditorSize, 100);
                        
                        // == Entities editor ==
                        ImGui.BeginChild("Entities Editor", new Vector2(-1, _entitiesEditorSize), false);
                        {
                            currentStage.ActivateSystemsWith(enable: true, typeof(WorldEditorAttribute));
                            DrawEntitiesEditor();
                        }
                        ImGui.EndChild();
                        ImGui.Dummy(new Vector2(0,8)); // Reserved for splitter
                        ImGuiHelpers.DrawSplitter("##splitter_world_tab_2", true, 8, ref _entitiesPickerSize, 100);

                        // == Entity picker ==
                        ImGui.BeginChild("Entity Picker", new Vector2(-1, _entitiesPickerSize), true, ImGuiWindowFlags.NoResize);
                        {
                            DrawAllInstancesToAdd(currentStage.EditorHook);
                        }
                        ImGui.EndChild();

                        ImGui.Dummy(new Vector2(0,8)); // Reserved for splitter

                        if (ImGui.GetContentRegionAvail().Y < 100)
                        {
                            _entityInspectorSize = 100;
                        }
                        else
                        {
                            _entityInspectorSize = -1;
                        }
                        ImGui.BeginChild("Entity Inspector", new System.Numerics.Vector2(-1, _entityInspectorSize), true);
                        if (currentStage.EditorHook.AllOpenedEntities.Length > 0)
                        {
                            // == Entities Inspector ==
                            
                            ImGui.DockSpace(id: 555);

                            if (_selectedAsset is Guid selectedGuid && _world?.TryGetInstance(selectedGuid) is EntityInstance instance)
                            {
                                DrawInstanceWindow(dockId: 555, currentStage, instance);
                            }

                            foreach ((int openedInstanceId, _) in currentStage.EditorHook.AllSelectedEntities)
                            {
                                if (currentStage.FindInstance(openedInstanceId) is EntityInstance e)
                                {
                                    DrawInstanceWindow(dockId: 555, currentStage, e, openedInstanceId);
                                }
                            }
                        }
                        ImGui.EndChild();

                        ImGui.EndChild();
                        ImGui.EndTabItem();
                    }
                    else
                    {
                        currentStage.ActivateSystemsWith(enable: false, typeof(WorldEditorAttribute));
                    }

                    if (ShouldDrawSystems)
                    {
                        if (ImGui.BeginTabItem($"{Icons.System} Systems"))
                        {
                            ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                            ImGui.BeginChild("systems_child", ImGui.GetContentRegionAvail()
                                - new System.Numerics.Vector2(0, 5));

                            DrawSystemsEditor();

                            ImGui.EndChild();
                            ImGui.PopStyleColor();

                            ImGui.EndTabItem();
                        }
                    }

                    if (ImGui.BeginTabItem($"{Icons.Tiles} Tiles"))
                    {
                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                        ImGui.BeginChild("tile_editor_child", ImGui.GetContentRegionAvail()
                            - new System.Numerics.Vector2(0, 5));

                        currentStage.ActivateSystemsWith(enable: true, typeof(TileEditorAttribute));
                        modified |= DrawTileEditor(currentStage);

                        ImGui.EndChild();
                        ImGui.PopStyleColor();

                        ImGui.EndTabItem();
                    }
                    else
                    {
                        currentStage.ActivateSystemsWith(enable: false, typeof(TileEditorAttribute));
                    }

                    if (ImGui.BeginTabItem($"{Icons.Cutscene} Story"))
                    {
                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                        ImGui.BeginChild("cutscene_editor_child", ImGui.GetContentRegionAvail()
                            - new System.Numerics.Vector2(0, 5));

                        currentStage.ActivateSystemsWith(enable: true, typeof(StoryEditorAttribute));
                        modified |= DrawStoryEditor(currentStage);

                        ImGui.EndChild();
                        ImGui.PopStyleColor();

                        ImGui.EndTabItem();
                    }
                    else
                    {
                        currentStage.ActivateSystemsWith(enable: false, typeof(StoryEditorAttribute));
                    }

                    if (ImGui.BeginTabItem($"{Icons.Sound} Sounds"))
                    {
                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                        ImGui.BeginChild("sound_editor_child", ImGui.GetContentRegionAvail()
                            - new System.Numerics.Vector2(0, 5));

                        currentStage.ActivateSystemsWith(enable: true, typeof(SoundEditorAttribute));
                        modified |= DrawSoundEditor(currentStage);

                        ImGui.EndChild();
                        ImGui.PopStyleColor();

                        ImGui.EndTabItem();
                    }
                    else
                    {
                        currentStage.ActivateSystemsWith(enable: false, typeof(SoundEditorAttribute));
                    }

                    if (ImGui.BeginTabItem($"{Icons.Settings} Settings"))
                    {
                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                        ImGui.BeginChild("cutscene_editor_child", ImGui.GetContentRegionAvail()
                            - new System.Numerics.Vector2(0, 5));
                        
                        ImGuiHelpers.ColorIcon('\uf57e', Game.Profile.Theme.Accent);
                        ImGuiHelpers.HelpTooltip("Display name of the world.");
                        ImGui.SameLine();

                        modified |= CustomField.DrawValueWithId(ref _asset, nameof(WorldAsset.WorldName));
                        
                        ImGuiHelpers.ColorIcon('\uf0dc', Game.Profile.Theme.Accent);
                        ImGuiHelpers.HelpTooltip("Order which this world should be displayed.");
                        ImGui.SameLine();

                        modified |= CustomField.DrawValueWithId(ref _asset, nameof(WorldAsset.Order));

                        ImGui.DragInt2("##MoveRoom", ref _moveRoomAmount[0], 1);
                        ImGui.SameLine();
                        if (ImGui.Button("Move Whole Map"))
                        {
                            MoveMap(currentStage, new(_moveRoomAmount[0], _moveRoomAmount[1]));
                        }

                        ImGui.EndChild();
                        ImGui.PopStyleColor();

                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }

                ImGui.TableNextColumn();

                currentStage.Draw();

                ImGui.EndTable();

                if (modified)
                {
                    _asset.FileChanged = true;
                }
            }
        }

        private void DrawInstanceWindow(uint dockId, Stage stage, EntityInstance instance, int selected = -1)
        {
            GameLogger.Verify(_asset is not null);

            // ImGui.SetNextWindowBgAlpha(0.75f);

            bool inspectingWindowOpen = true;

            ImGui.SetNextWindowDockID(dockId, ImGuiCond.Appearing);
            _assetWindowOpen = ImGui.Begin($"{instance.Name}##Instance_Editor_{instance.Guid}", ref inspectingWindowOpen);

            if (_selecting != -1 && _selecting == selected)
            {
                ImGui.SetWindowFocus();
            }

            if (_assetWindowOpen)
            {
                ImGui.Text(instance.Name);
                
                DrawEntity(instance, false);

                // Keep track of the last opened entity.
                _openedEntity = instance;
            }

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

        protected virtual void AddEntityFromWorld(Guid asset, IComponent[] components, string? group)
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

            if (group is not null)
            {
                MoveToGroup(group, instance.Guid);
            }
        }

        protected virtual void AddEntityFromWorld(IComponent[] components, string? group, string? name)
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

            name ??= "Instance";

            // If this is a tile grid, create a new room for it.
            if (isTilegrid && AddGroup("Room") is string roomName)
            {
                name = roomName;

                MoveToGroup(roomName, empty.Guid);
            }
            else if (group is not null)
            {
                MoveToGroup(group, empty.Guid);
            }

            // When adding tilegrids, make sure we also have a tileset.
            // Otherwise, create one.
            if (isTilegrid && Stages[_asset.Guid].FindEntitiesWith(typeof(TilesetComponent)).Count == 0)
            {
                EntityInstance tilesetEntity = EntityBuilder.CreateInstance(Guid.Empty, "Tileset");
                tilesetEntity.AddOrReplaceComponent(new TilesetComponent());
                
                AddInstance(tilesetEntity);
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

        public bool ShowCameraBounds
        {
            get
            {
                foreach ((Guid guid, Stage stage) in Stages)
                {
                    if (guid == _world?.Guid)
                    {
                        return stage.EditorHook.DrawCameraBounds is not null;
                    }
                }
                return false;
            }
            set
            {
                foreach ((Guid guid, Stage stage) in Stages)
                {
                    if (guid == _world?.Guid)
                    {
                        if (value)
                        {
                            if (stage.EditorHook.DrawCameraBounds == null)
                                stage.EditorHook.DrawCameraBounds = new Utilities.EditorHook.CameraBoundsInfo();
                        }
                        else
                            stage.EditorHook.DrawCameraBounds = null;
                    }
                }
            }
        }
        public bool HideStatic
        {
            get
            {
                foreach ((Guid guid, Stage stage) in Stages)
                {
                    if (guid == _world?.Guid)
                    {
                        return stage.EditorHook.HideStatic;
                    }
                }
                return false;
            }
            set
            {
                foreach ((Guid guid, Stage stage) in Stages)
                {
                    if (guid == _world?.Guid)
                    {
                        stage.EditorHook.HideStatic = value;
                    }
                }
            }
        }
        public void ResetCameraBounds()
        {
            foreach ((Guid guid, Stage stage) in Stages)
            {
                if (guid == _world?.Guid)
                {
                    if (stage.EditorHook.DrawCameraBounds is not null)
                    {
                        stage.EditorHook.DrawCameraBounds.ResetCameraBounds = true;
                    }
                }
            }
        }

        public bool ShowPuzzles
        {
            get => _showPuzzles;
            set
            {
                if (_showPuzzles == value)
                {
                    return;
                }

                _showPuzzles = value;

                foreach ((_, Stage stage) in Stages)
                {
                    stage.EditorHook.DrawTargetInteractions = value;
                }
            }
        }

        public override bool RunDiagnostics()
        {
            bool isValid = true;

            ImmutableArray<Guid> instances = Instances;
            foreach (Guid g in instances)
            {
                EntityInstance? e = TryFindInstance(g);
                if (e is null)
                {
                    continue;
                }

                foreach (IComponent c in e.Components)
                {
                    isValid |= CustomDiagnostic.ScanAllMembers(e.Name, c, outputResult: true);
                }
            }

            return isValid;
        }
    }
}