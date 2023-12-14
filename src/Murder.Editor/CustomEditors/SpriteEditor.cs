using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Stages;
using Murder.Editor.Systems;
using Murder.Editor.Utilities;
using Murder.Utilities;
using Newtonsoft.Json.Linq;
using System.Numerics;

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

            DrawFirstColumn(info);

            ImGui.TableNextColumn();

            Stage stage = info.Stage;

            if (ActiveEditors.ContainsKey(_sprite.Guid))
            {
                windowSize = ImGui.GetContentRegionAvail();
                Vector2 origin = ImGui.GetItemRectMin();
                float length = windowSize.X / 3f;

                stage.Draw();
            }

            ImGui.TableNextColumn();

            ImGui.TextColored(Game.Profile.Theme.HighAccent, "\uf0e0 Animation Messages");

            int value = 0;
            ImGui.InputInt($"##frame {_sprite.Guid}", ref value);

            string message = string.Empty;
            ImGui.InputTextWithHint($"##input {_sprite.Guid}", "Message name...", ref message, 256);

            ImGui.Button("Add message!");

            DrawMessages(info);
        }

        private void DrawFirstColumn(SpriteInformation info)
        {
            GameLogger.Verify(_sprite is not null);

            ImGui.TextColored(Game.Profile.Theme.Accent, $"\uf520 {_sprite.Name}");
            ImGui.Dummy(new(10, 10));

            IEnumerable<string> keys = _sprite.Animations.Keys.Order();

            bool displayed = false;
            foreach (string animation in keys)
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

        /// <summary>
        /// Select and preview an animation for this asset.
        /// </summary>
        private void SelectAnimation(SpriteInformation info, string animation)
        {
            info.SelectedAnimation = animation;
            info.Stage.AddOrReplaceComponentOnEntity(
                info.HelperId, 
                new AnimationOverloadComponent(animation, loop: true, ignoreFacing: true));
        }

        private void DrawMessages(SpriteInformation info)
        {
            GameLogger.Verify(_sprite is not null);

            using TableMultipleColumns table = new($"events_editor_component",
                flags: ImGuiTableFlags.NoBordersInBody,
                (-1, ImGuiTableColumnFlags.WidthFixed),
                (-1, ImGuiTableColumnFlags.WidthFixed),
                (-1, ImGuiTableColumnFlags.WidthStretch));

            Animation animation = _sprite.Animations[info.SelectedAnimation];
            for (int i = 0; i < animation.FrameCount; ++i)
            {
                if (!animation.Events.TryGetValue(i, out string? message))
                {
                    continue;
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                if (ImGuiHelpers.DeleteButton($"delete_event_listener_{i}"))
                {
                }

                ImGui.SameLine();

                ImGuiHelpers.SelectedButton($"Frame {i}");

                ImGui.TableNextColumn();
                ImGui.Text(message);

                ImGui.TableNextColumn();

                if (CustomField.DrawValue(ref info, fieldName: nameof(SpriteInformation.SoundTest)))
                {

                }
            }
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

            [Tooltip("This will create a sound to test in this editor. The actual sound must be added to the entity!")]
            [Default("Add sound to test")]
            public SoundEventId? SoundTest = null;
        }
    }
}
