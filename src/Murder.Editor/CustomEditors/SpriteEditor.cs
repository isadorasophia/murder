using Assimp;
using Assimp.Unmanaged;
using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Editor.Systems;
using Murder.Utilities;
using System.Numerics;
using System.Reflection.Emit;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(SpriteAsset))]
    internal class SpriteEditor : CustomEditor
    {
        /// <summary>
        /// Tracks the dialog system editors across different guids.
        /// </summary>
        protected Dictionary<Guid, SpriteInformation> ActiveEditors { get; private set; } = new();

        private SpriteAsset? _sprite = null;

        public override object Target => _sprite!;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, RenderContext renderContext, object target, bool overwrite)
        {
            _sprite = (SpriteAsset)target;

            if (!ActiveEditors.ContainsKey(_sprite.Guid))
            {
                Stage stage = new(imGuiRenderer, renderContext);

                SpriteInformation info = new(stage);
                ActiveEditors[_sprite.Guid] = info;

                InitializeStage(stage, info);
            }
        }

        private void InitializeStage(Stage stage, SpriteInformation info)
        {
            GameLogger.Verify(_sprite is not null);

            stage.ToggleSystem(typeof(EntitiesSelectorSystem), false);

            IEnumerable<string> animations = _sprite.Animations.Keys;
            string animation = animations.FirstOrDefault(s => !string.IsNullOrEmpty(s)) ?? string.Empty;

            Portrait portrait = new(_sprite.Guid, animation);
            
            info.HelperId = stage.AddEntityWithoutAsset(new PositionComponent(), new SpriteComponent(portrait));
            info.SelectedAnimation = portrait.AnimationId;

            stage.ShowInfo = false;
            stage.EditorHook.DrawSelection = false;
            stage.EditorHook.CurrentZoomLevel = 5;
        }

        public override void DrawEditor()
        {
            GameLogger.Verify(_sprite is not null);

            if (!ActiveEditors.TryGetValue(_sprite.Guid, out SpriteInformation? info))
            {
                GameLogger.Warning("Unitialized stage for particle editor?");
                return;
            }

            Vector2 windowSize = ImGui.GetContentRegionAvail();

            using TableMultipleColumns table = new(
                $"sprite_stage_{_sprite.Guid}", ImGuiTableFlags.Resizable, Calculator.RoundToInt(windowSize.X / 5), -1, Calculator.RoundToInt(windowSize.X / 5));

            ImGui.TableNextColumn();

            DrawFistColumn(info);

            ImGui.TableNextColumn();

            Stage stage = info.Stage;

            if (ActiveEditors.ContainsKey(_sprite.Guid))
            {
                windowSize = ImGui.GetContentRegionAvail();
                Vector2 origin = ImGui.GetItemRectMin();
                float length = windowSize.X / 3f;

                stage.Draw();
            }

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
        }

        private void DrawFistColumn(SpriteInformation info)
        {
            GameLogger.Verify(_sprite is not null);

            ImGui.TextColored(Game.Profile.Theme.Accent, $"\uf520 {_sprite.Name}");
            ImGui.Dummy(new(10, 10));

            bool displayed = false;
            foreach (string animation in _sprite.Animations.Keys)
            {
                if (string.IsNullOrEmpty(animation))
                {
                    continue;
                }

                displayed = true;

                bool selected = ImGuiHelpers.PrettySelectableWithIcon(
                    animation,
                    selectable: true,
                    disabled: info.SelectedAnimation == animation);

                if (selected)
                {
                    SelectAnimation(info, animation);
                }
            }

            if (!displayed)
            {
                ImGuiHelpers.PrettySelectableWithIcon(
                    "Default",
                    selectable: true,
                    disabled: true);

                info.SelectedAnimation = string.Empty;
            }
        }

        private void SelectAnimation(SpriteInformation info, string animation)
        {
            info.SelectedAnimation = animation;
            info.Stage.AddOrReplaceComponentOnEntity(
                info.HelperId, 
                new AnimationOverloadComponent(animation, loop: true, ignoreFacing: true));
        }

        public override void CloseEditor(Guid target)
        {
            if (ActiveEditors.TryGetValue(target, out SpriteInformation? info))
            {
                info.Stage.Dispose();
            }

            ActiveEditors.Remove(target);
        }

        protected record SpriteInformation(Stage Stage)
        {
            /// <summary>
            /// This is the entity id in the world.
            /// </summary>
            public int HelperId = 0;

            /// <summary>
            /// The last selected animation.
            /// </summary>
            public string SelectedAnimation = string.Empty;
        }
    }
}
