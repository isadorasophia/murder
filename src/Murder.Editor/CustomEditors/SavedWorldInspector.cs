using Bang;
using ImGuiNET;
using Murder.Assets;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using System.Collections.Immutable;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(SavedWorld))]
    internal class SavedWorldInspector : WorldAssetEditor
    {
        private SavedWorld? _savedWorld;

        protected override bool SupportsAdvancedEditing => false;

        protected override bool CanAddInstance => false;

        protected override ImmutableArray<Guid> Instances => _savedWorld?.Instances ?? ImmutableArray<Guid>.Empty;

        protected override void OnSwitchAsset(ImGuiRenderer imGuiRenderer, RenderContext renderContext, bool forceInit)
        {
            _savedWorld = (SavedWorld)_asset!;

            if (forceInit || !Stages.ContainsKey(_savedWorld.Guid))
            {
                Stage stage = new(imGuiRenderer, renderContext, Stage.StageType.None, _savedWorld);
                InitializeStage(stage, _asset!.Guid);

                CopySettingsFromWorldAsset(stage, renderContext);

                // force restarting systems.
                _previousActiveTab = WorldTab.None;
                DeactivateAndActivateSystemsForTab(stage, WorldTab.World);
            }
        }

        /// <summary>
        /// This copy all the settings from the world asset, hopefully aiming for a more "seamless" experience 
        /// between both assets.
        /// </summary>
        private void CopySettingsFromWorldAsset(Stage stage, RenderContext render)
        {
            GameLogger.Verify(_savedWorld is not null);

            if (Architect.EditorSettings.StageInfo.TryGetValue(_savedWorld.WorldGuid, out PersistStageInfo info))
            {
                render.Camera.Position = info.Position;
                render.OnClientWindowChanged(
                    new WindowChangeSettings(info.Size)
                    {
                        NativeResolution = info.Size,
                        Force = true
                    });

                stage.EditorHook.CurrentZoomLevel = info.Zoom;
                stage.EditorHook.StageSettings = info.Settings;
            }

            // this gets extra annoying for entities that cover things (like a roof?) so let's propagate this preference.
            if (Game.Data.TryGetAsset<WorldAsset>(_savedWorld.WorldGuid) is WorldAsset referenceWorld && 
                Architect.EditorSettings.WorldAssetInfo.TryGetValue(_savedWorld.WorldGuid, out PersistWorldStageInfo worldInfo))
            {
                foreach (string group in worldInfo.HiddenGroups)
                {
                    foreach (Guid id in referenceWorld.FetchEntitiesOfGroup(group))
                    {
                        if (referenceWorld.TryGetInstance(id) is EntityInstance instance)
                        {
                            SwitchInstanceVisibility(parent: null, instance);
                        }
                    }
                }
            }
        }

        public override EntityInstance? TryFindInstance(Guid guid) => _savedWorld?.TryGetInstance(guid);

        protected override bool CanDeleteInstance(IEntity? parent, IEntity entity) => true;

        protected override void DeleteInstance(IEntity? parent, Guid instanceGuid)
        {
            GameLogger.Verify(_savedWorld is not null);

            // Right now, we're just manually iterating over all entities. I guess we could keep ourselves a cache
            // to make things faster. But I'll be practical for now.
            SavedWorldData data = _savedWorld.Data;
            for (int i = 0; i < data.EntityInstances.Length; ++i)
            {
                EntityInstance instance = data.EntityInstances[i];
                if (instance.Guid == instanceGuid)
                {
                    SavedWorldData updatedData = new(data.EntityInstances.RemoveAt(i), data.NextEligibleEntityId);
                    _savedWorld.UpdateData(updatedData);

                    return;
                }
            }

            GameLogger.Error("Unable to find entity to be deleted?");
        }

        public override void DrawTopBar()
        {
            GameLogger.Verify(_savedWorld is not null);

            ImGui.SameLine();

            if (Game.Data.PendingSave is not null && Game.Data.PendingSave?.IsCompleted is false)
            {
                ImGuiHelpers.SelectedButton("Fetch From Asset");
                return;
            }

            if (ImGuiHelpers.ColoredButton("Fetch From Asset"))
            {
                Game.Data.WorldProcessor.UpdateFromAsset(_savedWorld);

                // force world to refresh.
                if (Architect.Instance.ActiveScene is EditorScene scene)
                {
                    scene.OpenAssetEditor(_savedWorld, true);
                }
            }
        }
    }
}