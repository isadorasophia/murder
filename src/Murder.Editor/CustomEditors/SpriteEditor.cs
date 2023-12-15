using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
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

        private float _viewportSize = 500;

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

            float available = ImGui.GetContentRegionAvail().Y;

            ImGuiHelpers.DrawSplitter("###viewport_split", true, 12, ref _viewportSize, 400);

            _viewportSize = Math.Clamp(_viewportSize, 400, available - 200);

            if (ImGui.BeginChild("Viewport", new Vector2(-1, _viewportSize)))
            {
                if (ActiveEditors.ContainsKey(_sprite.Guid))
                {
                    windowSize = ImGui.GetContentRegionAvail();
                    Vector2 origin = ImGui.GetItemRectMin();
                    float length = windowSize.X / 3f;

                    stage.Draw();
                }
            }
            ImGui.EndChild();
            ImGui.Dummy(new Vector2(0, 8));

            if (ImGui.BeginChild("Timeline Area"))
            {
                if (_sprite.Animations.TryGetValue(info.SelectedAnimation, out Animation selectedAnimation))
                {
                    if (ImGui.Button(""))
                    {

                    }

                    ImGui.Text($"{info.SelectedAnimation}, {selectedAnimation.Events.Count} event{(selectedAnimation.Events.Count>1?"s":"")}, {selectedAnimation.AnimationDuration}s duration");
                    if (ImGui.BeginChild("Timeline"))
                    {
                        Vector2 position = ImGui.GetItemRectMin();
                        Vector2 area = ImGui.GetContentRegionMax();
                        float padding = 6;

                        var drawList = ImGui.GetWindowDrawList();

                        drawList.AddRectFilled(position, position + area, Color.ToUint(Game.Profile.Theme.BgFaded), 10f);

                        uint frameColor = Color.ToUint(Game.Profile.Theme.Faded);
                        uint frameKeyColor = Color.ToUint(Game.Profile.Theme.Yellow);
                        uint arrowColor = Color.ToUint(Game.Profile.Theme.HighAccent);

                        drawList.AddLine(position + new Vector2(padding, padding), position + new Vector2(padding, area.Y - padding * 2), frameColor, 2);

                        float currentPosition = padding;

                        int animationFrame = selectedAnimation.Evaluate(info.AnimationProgress * selectedAnimation.AnimationDuration, false).InternalFrame;

                        for (int i = 0; i < selectedAnimation.FrameCount; i++)
                        {
                            float frameDuration = selectedAnimation.FramesDuration[i];
                            float framePercent = frameDuration / (selectedAnimation.AnimationDuration * 1000);
                            float width = framePercent * (area.X - padding * 2);

                            Vector2 framePosition = position + new Vector2(currentPosition, padding);
                            Vector2 frameSize = new Vector2(width - 4, area.Y - padding * 2);
                            Rectangle frameRectangle = new Rectangle(framePosition, frameSize);
                            currentPosition += width;

                            drawList.AddLine(framePosition + new Vector2(width,0), framePosition + new Vector2(width, frameSize.Y), frameColor, 2);

                            if (animationFrame == i)
                            {
                                drawList.AddRectFilled(framePosition + new Vector2(padding, 0), framePosition + frameSize, frameColor, 8);
                            }

                            if (selectedAnimation.Events.TryGetValue(i, out var @event))
                            {
                                drawList.AddRect(framePosition + new Vector2(padding, 0), framePosition + frameSize, frameKeyColor, 8);
                                if (frameRectangle.Contains(ImGui.GetMousePos()))
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.Text($"{i}\n{@event}\n{frameDuration}ms");
                                    ImGui.EndTooltip();
                                }
                                if (frameRectangle.Width > @event.Length * 8)
                                {
                                    drawList.AddText(framePosition + new Vector2(padding * 2, padding * 2), frameKeyColor, @event);
                                }
                            }
                            else
                            {
                                if (frameRectangle.Contains(ImGui.GetMousePos()))
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.Text($"{i}\n{frameDuration}ms");
                                    ImGui.EndTooltip();
                                }
                            }
                        }

                        float rate = info.AnimationProgress;

                        Vector2 arrowPosition = position + new Vector2(padding * 3 + (area.X - padding * 5) * rate, 0);
                        drawList.AddTriangleFilled(arrowPosition+ new Vector2(-6, 0), arrowPosition + new Vector2(6, 0), arrowPosition + new Vector2(0, 20), arrowColor);
                        drawList.AddLine(arrowPosition, arrowPosition + new Vector2(0, area.Y), arrowColor);

                        if (new Rectangle(position, area).Contains(ImGui.GetMousePos()) && ImGui.IsMouseDown(ImGuiMouseButton.Left))
                        {
                            float mouseX = ImGui.GetMousePos().X - position.X;

                            info.AnimationProgress = Calculator.Clamp01(mouseX / (area.X - padding * 2));
                        }
                    }
                }
                else
                {
                    ImGui.Text("No animation selected");
                }
                ImGui.EndChild();
            }
            ImGui.EndChild();

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

                ImGui.PushID($"sound_test_{i}");
                if (CustomField.DrawValue(ref info, fieldName: nameof(SpriteInformation.SoundTest)))
                {

                }
                ImGui.PopID();
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

            /// <summary>
            /// Used to track the current selected frame.
            /// </summary>
            public float AnimationProgress = 0;

            [Tooltip("This will create a sound to test in this editor. The actual sound must be added to the entity!")]
            [Default("Add sound to test")]
            public SoundEventId? SoundTest = null;
        }
    }
}
