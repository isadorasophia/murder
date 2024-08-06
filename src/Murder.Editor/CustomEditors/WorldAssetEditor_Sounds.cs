using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Diagnostics;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Prefabs;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;

namespace Murder.Editor.CustomEditors
{
    internal partial class WorldAssetEditor
    {
        private float _dockShowEntitiesSize = -1;
        protected virtual bool DrawSoundEditor(Stage stage)
        {
            if (_world is null)
            {
                return false;
            }

            bool modified = false;

            if (_dockShowEntitiesSize <= 0)
                _dockShowEntitiesSize = Calculator.RoundToInt(ImGui.GetContentRegionAvail().Y / 2f);

            ImGuiHelpers.DrawSplitter("##splitter_sound_tab_1", true, 8, ref _dockShowEntitiesSize, 100);

            ImGui.BeginChild("##sound_entities_child", new System.Numerics.Vector2(-1, _dockShowEntitiesSize));
            modified |= DrawSoundEntities(stage);

            ImGui.EndChild();

            ImGui.Dummy(new Vector2(0, 8)); // Reserved for splitter

            bool showOpenedEntities = stage.EditorHook.AllOpenedEntities.Length > 0;
            if (showOpenedEntities)
            {
                float height = ImGui.GetCursorPosY() + ImGui.GetContentRegionAvail().Y - 20;
                float dockOpenedEntitiesSize = height - _dockShowEntitiesSize;

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

        protected virtual bool DrawSoundEntities(Stage stage)
        {
            if (_world is null)
            {
                return false;
            }

            const string popupName = "New filter";
            if (ImGuiHelpers.IconButton('\uf65e', $"add_filter"))
            {
                _groupName = string.Empty;
                ImGui.OpenPopup(popupName);
            }

            DrawCreateOrRenameFilterPopup(popupName);

            ImGuiHelpers.HelpTooltip("Create a new filter");

            ImGui.PushItemWidth(-1);
            ImGui.SameLine();
            ImGui.InputTextWithHint("##search_assets", "Search...", ref _searchInstanceText, 256);
            ImGui.PopItemWidth();

            if (TreeEntityGroupNode("Area Triggers", Game.Profile.Theme.Yellow, icon: '\uf025', flags: ImGuiTreeNodeFlags.DefaultOpen))
            {
                List<IEntity> entities = stage.FindEntitiesWithAttribute<SoundAttribute>();
                DrawEntityList(stage, entities, filterGroup: null);

                ImGui.TreePop();
            }

            foreach ((string filter, _) in _world.FetchFilters())
            {
                if (TreeEntitySoundGroupNode(filter, Game.Profile.Theme.Yellow, icon: '\uf1bb'))
                {
                    List<IEntity> entities = _world.FetchEntitiesInFilter(filter);
                    DrawEntityList(stage, entities, filterGroup: filter);

                    ImGui.TreePop();
                }
            }

            if (TreeEntityGroupNode("Entities with sound hooks", Game.Profile.Theme.White, icon: '\uf569'))
            {
                List<IEntity> entities = stage.FindEntitiesWithAttribute<SoundPlayerAttribute>();
                DrawEntityList(stage, entities, filterGroup: null);

                ImGui.TreePop();
            }

            return false;
        }

        private void DrawCreateOrRenameFilterPopup(string popupName, string? previousName = null)
        {
            GameLogger.Verify(_world is not null);

            bool isRename = previousName is not null;

            if (ImGui.BeginPopup(popupName))
            {
                if (!isRename)
                {
                    ImGui.Text("What's the new filter name?");
                }

                ImGui.InputText("##group_name", ref _groupName, 128, ImGuiInputTextFlags.AutoSelectAll);

                string buttonLabel = isRename ? "Rename" : "Create";

                if (!string.IsNullOrWhiteSpace(_groupName) && _world is not null &&
                    !_world.FetchFilters().ContainsKey(_groupName))
                {
                    if (ImGui.Button(buttonLabel) || Game.Input.Pressed(Keys.Enter))
                    {
                        if (previousName is not null)
                        {
                            foreach (Guid instance in _world.FetchEntitiesGuidInFilter(previousName))
                            {
                                MoveToFilter(_groupName, instance, position: -1);
                            }

                            _world.DeleteFilter(previousName);
                        }
                        else
                        {
                            _world.AddFilter(_groupName);
                        }

                        ImGui.CloseCurrentPopup();
                    }
                }
                else
                {
                    ImGuiHelpers.DisabledButton(buttonLabel);
                    ImGuiHelpers.HelpTooltip("Unable to add group with duplicate or empty names.");
                }

                ImGui.EndPopup();
            }
        }

        private void RenameFilter(string oldName, string? newName)
        {
            Debug.Assert(_world is not null);
            foreach (Guid instance in _world.FetchEntitiesGuidInFilter(oldName))
            {
                MoveToFilter(newName, instance, position: -1);
            }

            _world.DeleteFilter(oldName);
        }
    }
}