using ImGuiNET;
using System.Collections.Immutable;
using Murder.Assets;
using Murder.Prefabs;
using Murder.ImGuiExtended;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.Stages;
using Murder.Editor.ImGuiExtended;
using Bang.Components;
using Microsoft.Xna.Framework.Input;
using System.Security.Cryptography;
using Murder.Components;
using Murder.Editor.Utilities;

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

            Stages[guid].EditorHook.AddEntityWithStage += AddEntityFromWorld;
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

                        DrawAllInstancesToAdd(669);

                        if (currentStage.EditorHook.AllOpenedEntities.Length > 0)
                        {
                            ImGui.BeginChild("##DockArea Selected Entity", new System.Numerics.Vector2(-1, dockOpenedEntitiesSize), false);
                            ImGui.DockSpace(id: 666);
                            ImGui.EndChild();

                            if (_selectedAsset is Guid selectedGuid && _world?.TryGetInstance(selectedGuid) is EntityInstance instance)
                            {
                                DrawInstanceWindow(currentStage, instance);
                            }

                            for (int i = currentStage.EditorHook.AllOpenedEntities.Length - 1; i >= 0; i--)
                            {
                                int opened = currentStage.EditorHook.AllOpenedEntities[i];

                                if (currentStage.FindInstance(opened) is EntityInstance e)
                                {
                                    DrawInstanceWindow(currentStage, e, opened);
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

        protected virtual void AddEntityFromWorld(IComponent[] components)
        {
            GameLogger.Verify(_world is not null && _asset is not null && Stages.ContainsKey(_asset.Guid));

            _asset.FileChanged = true;

            EntityInstance empty = EntityBuilder.CreateInstance(Guid.Empty);
            foreach (IComponent c in components)
            {
                empty.AddOrReplaceComponent(c);
            }

            empty.SetName($"Instance");

            // Add instance to the world instance.
            AddInstance(empty);
        }
    }
}