using Bang.Components;
using Bang.Entities;
using ImGuiNET;
using Murder.Assets;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.CustomDiagnostics;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(WorldAsset))]
    internal partial class WorldAssetEditor : AssetEditor
    {
        /// <summary>
        /// Switches the active WorldAssetEditor to the Tilesets tab
        /// </summary>
        public void SwitchToTilesetsTab() => _switchToTilesetsTab = true;
        private bool _switchToTilesetsTab = false;


        private WorldAsset? _world;

        private bool _showPuzzles = false;

        private bool _assetWindowOpen = true;
        private int _selecting;
        private readonly int[] _moveRoomAmount = new int[2];

        private Guid? _selectedAsset;

        public virtual EntityInstance? TryFindInstance(Guid guid) => _world?.TryGetInstance(guid);

        protected virtual ImmutableArray<Guid> Instances => _world?.Instances ?? ImmutableArray<Guid>.Empty;

        protected virtual bool ShouldDrawSystems => true;

        /// <summary>
        /// Keep track of the information of the world for this particular stage per asset.
        /// </summary>
        private readonly Dictionary<Guid, WorldStageInfo> _worldStageInfo = new();

        protected override void OnSwitchAsset(ImGuiRenderer imGuiRenderer, RenderContext renderContext, bool forceInit)
        {
            _world = (WorldAsset)_asset!;
            _world.ValidateInstances();

            if (!Stages.TryGetValue(_world.Guid, out Stage? stage) || stage.AssetReference != _world)
            {
                GameLogger.Verify(stage is null ||
                    stage.AssetReference != _world, "Why are we replacing the asset reference? Call isa to debug this! <3");

                stage = new(imGuiRenderer, renderContext, Stage.StageType.None, _world);
                InitializeStage(stage, _world.Guid);
            }

            // Clear cache.
            _entitiesPerGroup.Clear();

            // Assign this so we can update the cache
            WorldTab previousTab = _previousActiveTab;
            _previousActiveTab = WorldTab.None;

            DeactivateAndActivateSystemsForTab(stage, previousTab);
        }

        protected override void InitializeStage(Stage stage, Guid guid)
        {
            base.InitializeStage(stage, guid);

            Stages[guid].EditorHook.AddPrefabWithStage += AddEntityFromWorld;
            Stages[guid].EditorHook.AddEntityWithStage += AddEntityFromWorld;
            Stages[guid].EditorHook.RemoveEntityWithStage += DeleteEntityFromWorld;

            Stages[guid].EditorHook.MoveEntitiesToFolder += MoveEntitiesToGroup;
            Stages[guid].EditorHook.GetAvailableFolders = GetAvailableGroups;

            Stages[guid].EditorHook.DuplicateEntitiesAt += DuplicateAndSelectEntitiesAt;

            if (Architect.EditorSettings.WorldAssetInfo.TryGetValue(guid, out PersistWorldStageInfo info))
            {
                _worldStageInfo[guid] = new()
                {
                    HiddenGroups = info.HiddenGroups,
                    SkipGroups = info.LockedGroups,
                };

                foreach (string group in info.LockedGroups)
                {
                    SkipGroupInEditor(group, true);
                }

                foreach (string group in info.HiddenGroups)
                {
                    SwitchGroupVisibility(group, false); 
                }
            }
            else
            {
                _worldStageInfo[guid] = new();
            }
        }

        private IEntity? _openedEntity = null;

        public override IEntity? SelectedEntity => _world is null ? null : _openedEntity;

        float _entitiesEditorSize = 300;
        float _entitiesPickerSize = 200;
        float _entityInspectorSize = -1;

        public override void UpdateEditor()
        {
            if (_asset is null || !Stages.ContainsKey(_asset!.Guid))
            {
                return;
            }

            Stage currentStage = Stages[_asset.Guid];
            currentStage.Update();
        }

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
                        ImGui.BeginChild("world_tab_container", new System.Numerics.Vector2(-1, -1), ImGuiChildFlags.None, ImGuiWindowFlags.NoResize);

                        ImGuiHelpers.DrawSplitter("##splitter_world_tab_1", true, 8, ref _entitiesEditorSize, 200);

                        // == Entities editor and room list ==
                        ImGui.BeginChild("Entities Editor", new Vector2(-1, _entitiesEditorSize), ImGuiChildFlags.None);
                        {
                            DeactivateAndActivateSystemsForTab(currentStage, WorldTab.World);
                            DrawEntitiesEditor(currentStage);
                        }
                        ImGui.EndChild();
                        ImGui.Dummy(new Vector2(0, 8)); // Reserved for splitter
                        ImGuiHelpers.DrawSplitter("##splitter_world_tab_2", true, 8, ref _entitiesPickerSize, 100);

                        // == Entity picker ==

                        ImGui.BeginChild("Entity Picker", new Vector2(-1, _entitiesPickerSize), ImGuiChildFlags.None, ImGuiWindowFlags.NoResize);
                        {
                            ImGui.SeparatorText("Prefab Palette");

                            DrawAllInstancesToAdd(currentStage.EditorHook);
                        }
                        ImGui.EndChild();

                        ImGui.Dummy(new Vector2(0, 8)); // Reserved for splitter

                        if (ImGui.GetContentRegionAvail().Y < 100)
                        {
                            _entityInspectorSize = 100;
                        }
                        else
                        {
                            _entityInspectorSize = -1;
                        }
                        ImGui.BeginChild("Entity Inspector", new System.Numerics.Vector2(-1, _entityInspectorSize), ImGuiChildFlags.None);
                        if (currentStage.EditorHook.AllOpenedEntities.Length > 0)
                        {
                            // == Entities Inspector ==

                            ImGui.DockSpace(dockspace_id: 555);

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

                    if (ShouldDrawSystems)
                    {
                        if (ImGui.BeginTabItem($"{Icons.System} Systems"))
                        {
                            ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                            ImGui.BeginChild("systems_child", ImGui.GetContentRegionAvail() - new Vector2(0, 5));

                            DrawSystemsEditor();

                            ImGui.EndChild();
                            ImGui.PopStyleColor();

                            ImGui.EndTabItem();
                        }
                    }

                    bool always = true;
                    if (ImGui.BeginTabItem($"{Icons.Tiles} Tiles", ref always, _switchToTilesetsTab? ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None))
                    {
                        _switchToTilesetsTab = false;
                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                        ImGui.BeginChild("tile_editor_child", ImGui.GetContentRegionAvail() - new Vector2(0, 5));

                        DeactivateAndActivateSystemsForTab(currentStage, WorldTab.Tiles);
                        modified |= DrawTileEditor(currentStage);

                        ImGui.EndChild();
                        ImGui.PopStyleColor();

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem($"{Icons.Map} Pathfind"))
                    {
                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                        ImGui.BeginChild("pathfind_editor_child", ImGui.GetContentRegionAvail() - new Vector2(0, 5));

                        DeactivateAndActivateSystemsForTab(currentStage, WorldTab.Pathfind);
                        modified |= DrawPathfindEditor(currentStage);

                        ImGui.EndChild();
                        ImGui.PopStyleColor();

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem($"{Icons.Cutscene} Story"))
                    {
                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                        ImGui.BeginChild("cutscene_editor_child", ImGui.GetContentRegionAvail() - new Vector2(0, 5));

                        DeactivateAndActivateSystemsForTab(currentStage, WorldTab.Story);
                        modified |= DrawStoryEditor(currentStage);

                        ImGui.EndChild();
                        ImGui.PopStyleColor();

                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem($"{Icons.Sound} Sounds"))
                    {
                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                        ImGui.BeginChild("sound_editor_child", ImGui.GetContentRegionAvail() - new Vector2(0, 5));

                        DeactivateAndActivateSystemsForTab(currentStage, WorldTab.Sound);
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
                        ImGui.BeginChild("cutscene_editor_child", ImGui.GetContentRegionAvail() - new Vector2(0, 5));

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
                currentStage.PersistInfo(_asset.Guid);

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
            _assetWindowOpen = ImGui.Begin($"{instance.Name}##Instance_Editor_{instance.Guid}", ref inspectingWindowOpen, ImGuiWindowFlags.NoNav);

            if (_selecting != -1 && _selecting == selected)
            {
                ImGui.SetWindowFocus();
            }

            if (_assetWindowOpen)
            {
                ImGui.SeparatorText(instance.Name);

                DrawEntity(instance, false);

                // Keep track of the last opened entity.
                _openedEntity = instance;
            }

            ImGui.End();

            if (!inspectingWindowOpen)
            {
                stage.SelectEntity(selected, select: false, clear: true);
            }
        }

        protected virtual bool CanAddInstance => true;

        protected override void Duplicate(EntityInstance instance)
        {
            EntityInstance copy = SerializationHelper.DeepCopy(instance);
            copy.UpdateGuid(Guid.NewGuid());

            AddInstance(copy);
        }

        protected virtual int AddInstance(EntityInstance instance)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));
            GameLogger.Verify(_world is not null);

            _world.AddInstance(instance);
            int entityId = Stages[_asset.Guid].AddEntity(instance);

            _world.FileChanged = true;

            return entityId;
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

            // Add cute ~tween~ effect.
            instance.AddOrReplaceComponent(new PlacedInWorldComponent(Game.NowUnscaled));

            // Add instance to the world instance.
            AddInstance(instance);

            // Immediately remove our cute tween.
            instance.RemoveComponent(typeof(PlacedInWorldComponent));

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

        protected void DuplicateAndSelectEntitiesAt(int[] entities, Vector2 offset)
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            List<Guid> createdEntities = new();

            ActWithUndo(
                @do: () =>
                {
                    Stage stage = Stages[_asset.Guid];
                    stage.EditorHook.UnselectAll();

                    foreach (int entityId in entities)
                    {
                        if (stage.FindInstance(entityId) is not EntityInstance instance)
                        {
                            GameLogger.Error($"Unable to copy entity {entityId}.");
                            continue;
                        }

                        EntityInstance copy = SerializationHelper.DeepCopy(instance);
                        if (copy.HasComponent(typeof(PositionComponent)))
                        {
                            PositionComponent transform =
                                (PositionComponent)copy.GetComponent(typeof(PositionComponent));

                            copy.AddOrReplaceComponent(transform.Add(offset));
                        }

                        copy.UpdateGuid(Guid.NewGuid());

                        createdEntities.Add(copy.Guid);

                        int result = AddInstance(copy);
                        stage.SelectEntity(result, select: true, clear: false);
                    }

                    _asset.FileChanged = true;
                },
                @undo: () =>
                {
                    foreach (Guid createdInstance in createdEntities)
                    {
                        DeleteInstance(parent: null, createdInstance);
                    }
                });
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
                        // It's actually been nice to not move entities with the room, but let's see...
                        // MoveGroupOfEntities(
                        //    entity.Guid, from: previousTileGrid.Origin, to: tileGrid.Origin, groupForTilegrid);
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

                PositionComponent transform =
                    (PositionComponent)entity.GetComponent(typeof(PositionComponent));

                ReplaceComponent(parent: null, entity, transform.Add(worldDelta));
            }
        }

        private IEnumerable<string> GetAvailableGroups()
        {
            if (_world is null)
            {
                return Enumerable.Empty<string>();
            }

            GameLogger.Verify(_world is not null);
            return _world.FetchFolderNames();
        }

        private void MoveEntitiesToGroup(string targetGroup, IEnumerable<int> entities)
        {
            GameLogger.Verify(_world is not null);

            Stage stage = Stages[_world.Guid];
            WorldStageInfo info = _worldStageInfo[_world.Guid];

            foreach (int entityId in entities)
            {
                (Guid? parent, Guid? g) = stage.FindInstanceGuid(entityId);
                if (g is null)
                {
                    continue;
                }

                MoveToGroup(targetGroup, g.Value);

                // Check if we "inherited" anything from this new group...
                if (info.HiddenGroups.Contains(targetGroup))
                {
                    HideInstanceInEditor(parent, g.Value);
                }

                if (info.SkipGroups.Contains(targetGroup))
                {
                    stage.ReplaceComponentForInstance(parent, g.Value, new SkipComponent());
                }
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

        internal void ResetCamera()
        {
            foreach ((Guid guid, Stage stage) in Stages)
            {
                if (guid == _world?.Guid)
                {
                    stage.ResetCamera();
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

        protected override bool UpdateEntityInstanceReferences(EntityInstance instance)
        {
            if (_world is null)
            {
                return false;
            }

            _world.AddInstance(instance);
            return true;
        }

        public override void CloseEditor(Guid guid)
        {
            base.CloseEditor(guid);

            _worldStageInfo.Remove(guid);
        }

        private class WorldStageInfo
        {
            public HashSet<string> HiddenSoundGroups { get; init; } = new();
            public HashSet<string> HiddenGroups { get; init; } = new();

            public HashSet<string> SkipGroups { get; init; } = new();

            public string? HideGroupsExceptFor = null;
        }

        private enum WorldTab
        {
            None = 0,
            World = 1,
            Systems = 2,
            Tiles = 3,
            Pathfind = 4,
            Story = 5,
            Sound = 6,
            Settings = 7
        }

        private WorldTab _previousActiveTab = WorldTab.World;

        private readonly static Dictionary<WorldTab, Type> _tabToAttributeToDeactivate = new()
        {
            { WorldTab.World, typeof(WorldEditorAttribute) },
            { WorldTab.Tiles, typeof(TileEditorAttribute) },
            { WorldTab.Pathfind, typeof(PathfindEditorAttribute) },
            { WorldTab.Story, typeof(StoryEditorAttribute) },
            { WorldTab.Sound, typeof(SoundEditorAttribute) }
        };

        private void DeactivateAndActivateSystemsForTab(Stage stage, WorldTab activeTab)
        {
            if (_previousActiveTab == activeTab)
            {
                return;
            }

            // First, deactivate previous systems.
            foreach ((WorldTab tab, Type attr) in _tabToAttributeToDeactivate)
            {
                if (tab == activeTab)
                {
                    continue;
                }

                stage.ActivateSystemsWith(enable: false, attr);
            }

            ShowGrid = false;
            ShowReflection = false;

            // Now, activate current systems.
            stage.ActivateSystemsWith(enable: true, _tabToAttributeToDeactivate[activeTab]);

            _previousActiveTab = activeTab;
        }

    }
}