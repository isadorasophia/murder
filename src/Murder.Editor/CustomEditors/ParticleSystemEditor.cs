using ImGuiNET;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Assets.Graphics;
using Murder.Editor.Stages;
using Murder.Components;
using Murder.Editor.CustomFields;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(ParticleSystemAsset))]
    internal class ParticleSystemEditor : CustomEditor
    {
        /// <summary>
        /// Tracks the particle system editors across different guids.
        /// </summary>
        protected Dictionary<Guid, (Stage Stage, int EntityId)> Stages { get; private set; } = new();
        
        public override object Target => _particleAsset!;

        private ParticleSystemAsset? _particleAsset;
        
        public override void OpenEditor(ImGuiRenderer imGuiRenderer, object target)
        {
            _particleAsset = (ParticleSystemAsset)target;
            
            if (!Stages.ContainsKey(_particleAsset.Guid))
            {
                Stage stage = new(imGuiRenderer);
                int entityId = stage.AddEntityWithoutAsset(
                    _particleAsset.GetTrackerComponent(), new PositionComponent(0, 0));
                
                Stages[_particleAsset.Guid] = (stage, entityId);
            }
        }

        public override void DrawEditor()
        {
            GameLogger.Verify(Stages is not null);
            GameLogger.Verify(_particleAsset is not null);
            
            if (!Stages.TryGetValue(_particleAsset.Guid, out var stageInfo))
            {
                GameLogger.Warning("Unitialized stage for particle editor?");
                return;
            }

            (Stage stage, int entityId) = stageInfo;

            if (ImGui.BeginTable("particles_table", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthStretch, -1f, 1);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthFixed, 600.WithDpi(), 0);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                if (Stages.ContainsKey(_particleAsset.Guid))
                {
                    stage.EditorHook.DrawSelection = false;
                    stage.Draw();
                }

                ImGui.TableNextColumn();

                float height = ImGui.GetWindowContentRegionMax().Y - 60.WithDpi();
                ImGui.BeginChild("particles_table", new System.Numerics.Vector2(-1, height));

                if (TreeEntityGroupNode("Emitter", Game.Profile.Theme.White, icon: '\uf0c2'))
                {
                    if (CustomField.DrawValue(ref _particleAsset, nameof(ParticleSystemAsset.Emitter)))
                    {
                        _particleAsset.FileChanged = true;
                        
                        stage.ReplaceComponentOnEntity(entityId, _particleAsset.GetTrackerComponent());
                    }

                    ImGui.TreePop();
                }

                if (TreeEntityGroupNode("Particle", Game.Profile.Theme.White, icon: '\uf002'))
                {
                    if (CustomField.DrawValue(ref _particleAsset, nameof(ParticleSystemAsset.Particle)))
                    {
                        _particleAsset.FileChanged = true;

                        stage.ReplaceComponentOnEntity(entityId, _particleAsset.GetTrackerComponent());
                    }

                    ImGui.TreePop();
                }

                ImGui.EndChild();

                ImGui.EndTable();
            }
        }
        
        private bool TreeEntityGroupNode(string name, System.Numerics.Vector4 textColor, char icon = '\ue1b0', ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None) =>
            ImGuiHelpers.TreeNodeWithIconAndColor(
                icon: icon,
                label: name,
                flags: ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.FramePadding | flags,
                text: textColor,
                background: Game.Profile.Theme.BgFaded,
                active: Game.Profile.Theme.Bg);
    }
}
