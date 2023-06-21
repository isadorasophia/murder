using ImGuiNET;
using Murder.Editor.Stages;
using Murder.Prefabs;
using Murder.Utilities;
using Murder.Utilities.Attributes;

namespace Murder.Editor.CustomEditors
{
    internal partial class WorldAssetEditor
    {
        protected virtual bool DrawSoundEditor(Stage stage)
        {
            if (_world is null)
            {
                return false;
            }

            bool modified = false;

            int dockShowEntitiesSize = Calculator.RoundToInt(ImGui.GetContentRegionMax().Y / 2.5f);

            ImGui.BeginChild("##sound_entities_child", new System.Numerics.Vector2(-1, dockShowEntitiesSize));
            modified |= DrawSoundEntities(stage);

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

        protected virtual bool DrawSoundEntities(Stage stage)
        {
            if (TreeEntityGroupNode("Music Entities", Game.Profile.Theme.Yellow, icon: '\uf025', flags: ImGuiTreeNodeFlags.DefaultOpen))
            {
                IList<IEntity> entities = stage.FindEntitiesWithAttribute<SoundAttribute>();
                DrawEntityList(stage, entities);

                ImGui.TreePop();
            }

            if (TreeEntityGroupNode("Sound Entities", Game.Profile.Theme.Yellow, icon: '\uf569'))
            {
                IList<IEntity> entities = stage.FindEntitiesWithAttribute<SoundPlayerAttribute>();
                DrawEntityList(stage, entities);

                ImGui.TreePop();
            }

            return false;
        }
    }
}