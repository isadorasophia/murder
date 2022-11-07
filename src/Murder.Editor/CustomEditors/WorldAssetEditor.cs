using ImGuiNET;
using System.Collections.Immutable;
using Murder.Assets;
using Murder.Prefabs;
using Murder.ImGuiExtended;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.Stages;
using Murder.Editor.ImGuiExtended;

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

                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Game.Profile.Theme.Bg);
                        ImGui.BeginChild("world_child", new System.Numerics.Vector2(-1, 400)
                            - new System.Numerics.Vector2(0, 5) * Architect.Instance.DPIScale / 100f);

                        currentStage.ActivateSystemsWith(enable: true, typeof(WorldEditorAttribute));

                        DrawEntitiesEditor();

                        ImGui.EndChild();
                        ImGui.PopStyleColor();

                        ImGui.EndTabItem();


                        if (currentStage.EditorHook.Selected.Count > 0)
                        {
                            ImGui.BeginChild("##DockArea Selected Entity", new System.Numerics.Vector2(-1, -1), false);
                            ImGui.DockSpace(666);
                            ImGui.EndChild();

                            if (_selectedAsset is Guid selectedGuid && _world?.TryGetInstance(selectedGuid) is EntityInstance instance)
                            {
                                DrawInstanceWindow(currentStage, instance);
                            }
                            
                            for (int i = currentStage.EditorHook.Selected.Count - 1; i >= 0; i--)
                            {
                                int selected = currentStage.EditorHook.Selected[i];
                                
                                if (currentStage.FindInstance(selected) is EntityInstance e)
                                {
                                    DrawInstanceWindow(currentStage, e, selected);
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

        private void DrawEntitiesEditor()
        {
            GameLogger.Verify(_asset is not null && Stages.ContainsKey(_asset.Guid));

            _selecting = -1;

            if (CanAddInstance)
            {
                if (SearchBox.SearchInstantiableEntities() is Guid asset)
                {
                    EntityInstance instance = EntityBuilder.CreateInstance(asset);
                    AddInstance(instance);
                }
            }

            foreach (Guid entity in Instances)
            {
                if (ImGuiHelpers.DeleteButton($"Delete_{entity}"))
                {
                    DeleteInstance(parent: null, entity);
                }

                ImGui.SameLine();

                ImGui.PushID($"Entity_{entity}");

                if (ImGui.Selectable(TryFindInstance(entity)?.Name ?? "<?>", Stages[_asset.Guid].IsSelected(entity)))
                {
                    _selecting = Stages[_asset.Guid].SelectEntity(entity);
                    if (_selecting is -1)
                    {
                        // Unable to find the entity. This probably means that it has been deactivated.
                        _selectedAsset = entity;
                    }
                    else
                    {
                        _selectedAsset = null;
                    }
                }
                
                ImGui.PopID();
            }
        }

        private void DrawInstanceWindow(Stage stage, EntityInstance instance, int selected = -1)
        {
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
                stage.EditorHook.Selected.Remove(selected);
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
    }
}