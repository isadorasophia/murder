using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Attributes;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Stages;
using Murder.Editor.Systems;
using Murder.Editor.Utilities;
using Murder.Editor.Utilities.Attributes;
using Murder.Systems.Effects;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        private readonly float _timelineSize = 100;

        private int? _targetFrameForPopup = null;
        private string _message = string.Empty;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, RenderContext renderContext, object target, bool overwrite)
        {
            _sprite = (SpriteAsset)target;

            if (!ActiveEditors.ContainsKey(_sprite.Guid))
            {
                Stage stage = new(imGuiRenderer, renderContext, hook: new TimelineEditorHook(), Stage.StageType.None);

                SpriteInformation info = new(stage);
                ActiveEditors[_sprite.Guid] = info;

                InitializeStage(stage, info);
            }
        }

        private void InitializeStage(Stage stage, SpriteInformation info)
        {
            GameLogger.Verify(_sprite is not null);

            stage.ActivateSystemsWith(enable: true, typeof(SpriteEditorAttribute));
            stage.ToggleSystem(typeof(EntitiesSelectorSystem), false);

            IEnumerable<string> animations = _sprite.Animations.Keys;
            string animation = animations.FirstOrDefault(s => !string.IsNullOrEmpty(s)) ?? string.Empty;

            Portrait portrait = new(_sprite.Guid, animation);
            
            info.HelperId = stage.AddEntityWithoutAsset(new PositionComponent(), new SpriteComponent(portrait));
            info.SelectedAnimation = portrait.AnimationId;

            stage.ShowInfo = false;
            stage.EditorHook.DrawSelection = false;
            stage.EditorHook.CurrentZoomLevel = 6;
            stage.CenterCamera();

            stage.ToggleSystem(typeof(EventListenerSystem), enable: true);
        }

        public override void UpdateEditor()
        {
            if (_sprite is null || !ActiveEditors.TryGetValue(_sprite.Guid, out SpriteInformation? info))
            {
                return;
            }

            info.Stage.Update();
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
            float _viewportSize = available - _timelineSize;

            ImGuiHelpers.DrawSplitter("###viewport_split", true, 12, ref _viewportSize, 200);

            _viewportSize = Math.Clamp(_viewportSize, 400, Math.Max(400, available - 200));

            if (ImGui.BeginChild("Viewport", new Vector2(-1, _viewportSize)))
            {
                windowSize = ImGui.GetContentRegionAvail();
                Vector2 origin = ImGui.GetItemRectMin();

                stage.Draw();
            }

            ImGui.EndChild();
            ImGui.Dummy(new Vector2(0, 8));

            DrawTimeline(info);

            ImGui.TableNextColumn();

            ImGui.TextColored(Game.Profile.Theme.HighAccent, "\uf0e0 Messages");

            DrawMessages(info);
        }

        private void AddMessage(string animation, int frame, string message)
        {
            GameLogger.Verify(_sprite is not null);

            SpriteEventDataManagerAsset manager = Architect.EditorData.GetOrCreateSpriteEventData();

            // Start by updating our manager.
            SpriteEventData data = manager.GetOrCreate(_sprite.Guid);
            data.AddEvent(animation, frame, message);

            // Also, let the sprite know that this is a thing now.
            _sprite.AddMessageToAnimationFrame(animation, frame, message);
            _sprite.TrackAssetOnSave(manager.Guid);

            manager.FileChanged = true;

            Architect.EditorData.EditorSettings.LastMetadataImported = DateTime.Now;
        }

        private void DeleteMessage(string animation, int frame)
        {
            GameLogger.Verify(_sprite is not null);

            SpriteEventDataManagerAsset manager = Architect.EditorData.GetOrCreateSpriteEventData();

            SpriteEventData data = manager.GetOrCreate(_sprite.Guid);
            data.RemoveEvent(animation, frame);

            _sprite.RemoveMessageFromAnimationFrame(animation, frame);
            _sprite.TrackAssetOnSave(manager.Guid);

            manager.FileChanged = true;

            Architect.EditorData.EditorSettings.LastMetadataImported = DateTime.Now;
        }

        private bool AddTestSounds(SpriteInformation info)
        {
            GameLogger.Verify(_sprite is not null);

            var builder = ImmutableDictionary.CreateBuilder<string, SpriteEventInfo>();

            foreach ((string message, SoundEventId? sound) in info.SoundTests)
            {
                if (sound is null || sound.Value.IsGuidEmpty)
                {
                    continue;
                }

                builder[message] = new(message, sound.Value, persisted: null, interactions: null);
            }

            info.Stage.AddOrReplaceComponentOnEntity(
                info.HelperId,
                new EventListenerComponent(builder.ToImmutable()));

            return true;
        }

        /// <summary>
        /// Select and preview an animation for this asset.
        /// </summary>
        private void SelectAnimation(SpriteInformation info, string animation)
        {
            info.SelectedAnimation = animation;

            info.Stage.AddOrReplaceComponentOnEntity(
                info.HelperId,
                new AnimationOverloadComponent(animation, loop: true, ignoreFacing: true, startTime: 0));
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

        private void DrawMessages(SpriteInformation info)
        {
            GameLogger.Verify(_sprite is not null);

            using TableMultipleColumns table = new($"events_editor_component",
                flags: ImGuiTableFlags.NoBordersInBody,
                (-1, ImGuiTableColumnFlags.WidthFixed),
                (-1, ImGuiTableColumnFlags.WidthFixed),
                (-1, ImGuiTableColumnFlags.WidthStretch),
                (53, ImGuiTableColumnFlags.WidthFixed));

            Animation animation = _sprite.Animations[info.SelectedAnimation];
            for (int i = 0; i < animation.FrameCount; ++i)
            {
                if (!animation.Events.TryGetValue(i, out string? message))
                {
                    continue;
                }

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImGuiHelpers.SelectedButton($"{i}");

                ImGui.TableNextColumn();
                ImGui.Text(message);

                ImGui.TableNextColumn();

                EditorMember? member = ReflectionHelper.TryGetFieldForEditor(typeof(SpriteInformation), nameof(SpriteInformation.SoundTests));
                member = member?.CreateFrom(typeof(SoundEventId?), name: "SoundTest"); // Make a fake member so the editor is happy drawing this.

                info.SoundTests.TryGetValue(message, out SoundEventId? sound);

                ImGui.PushID($"sound_test_{i}");

                if (member is not null && 
                    CustomField.DrawValue(member, sound, out SoundEventId? result))
                {
                    info.SoundTests[message] = result;
                    AddTestSounds(info);
                }

                ImGui.PopID();

                ImGuiHelpers.HelpTooltip("This value is just for testing, it must be set on the prefab that uses this sprite.");

                ImGui.TableNextColumn();

                if (ImGuiHelpers.IconButton('\uf303', $"rename_event_listener_{i}"))
                {
                    _message = message;
                    ImGui.OpenPopup($"rename_message_to_frame_{i}");
                }

                DrawRenamePopup(info, i);

                ImGui.SameLine();

                if (ImGuiHelpers.DeleteButton($"delete_event_listener_{i}"))
                {
                    DeleteMessage(info.SelectedAnimation, frame: i);
                }

                ImGui.SameLine();
            }
        }

        private void DrawTimeline(SpriteInformation info)
        {
            GameLogger.Verify(_sprite is not null);

            int targetAnimationFrame;

            Vector2 position;
            float mouseRatio = 0;

            if (ImGui.BeginChild("Timeline Area"))
            {
                if (_sprite.Animations.TryGetValue(info.SelectedAnimation, out Animation selectedAnimation))
                {
                    if (info.Hook.IsPaused && (ImGui.Button("\uf04b") || Game.Input.Pressed(MurderInputButtons.Space)))
                    {
                        info.Hook.IsPaused = false;
                    }
                    else if (!info.Hook.IsPaused && (ImGui.Button("\uf04c") || Game.Input.Pressed(MurderInputButtons.Space)))
                    {
                        info.Hook.IsPaused = true;
                    }

                    ImGui.SameLine();

                    float rate = (info.Hook.Time % selectedAnimation.AnimationDuration) / selectedAnimation.AnimationDuration;
                    targetAnimationFrame = selectedAnimation.Evaluate(rate * selectedAnimation.AnimationDuration, false).InternalFrame;
                    
                    DrawHeader(info, targetAnimationFrame, selectedAnimation);

                    if (ImGui.BeginChild("Timeline"))
                    {
                        position = ImGui.GetItemRectMin();
                        float mouseX = ImGui.GetMousePos().X - position.X - 10 /* cursor offset...? */;

                        Vector2 area = ImGui.GetContentRegionAvail();
                        float padding = 6;

                        var drawList = ImGui.GetWindowDrawList();

                        drawList.AddRectFilled(position, position + area, Color.ToUint(Game.Profile.Theme.BgFaded), 10f);

                        uint frameColor = Color.ToUint(Game.Profile.Theme.Faded);
                        uint frameKeyColor = Color.ToUint(Game.Profile.Theme.Yellow);
                        uint arrowColor = Color.ToUint(Game.Profile.Theme.HighAccent);

                        drawList.AddLine(position + new Vector2(padding, padding), position + new Vector2(padding, area.Y - padding * 2), frameColor, 2);

                        float currentPosition = padding;


                        for (int i = 0; i < selectedAnimation.FrameCount; i++)
                        {
                            float framePercent;
                            float frameDuration;
                            if (selectedAnimation.AnimationDuration == 0 && selectedAnimation.FramesDuration[i] == 0)
                            {
                                framePercent = 1;
                                frameDuration = 1;
                            }
                            else
                            {
                                frameDuration = selectedAnimation.FramesDuration[i];
                                framePercent = frameDuration / (MathF.Max(0.0001f, selectedAnimation.AnimationDuration) * 1000);
                            }

                            float width = framePercent * (area.X - padding * 2);

                            Vector2 framePosition = position + new Vector2(currentPosition, padding);
                            Vector2 frameSize = new Vector2(width - 4, area.Y - padding * 2);
                            Rectangle frameRectangle = new Rectangle(framePosition, frameSize);
                            currentPosition += width;

                            drawList.AddLine(framePosition + new Vector2(width, 0), framePosition + new Vector2(width, frameSize.Y), frameColor, 2);

                            if (targetAnimationFrame == i)
                            {
                                drawList.AddRectFilled(framePosition + new Vector2(padding, 0), framePosition + frameSize, frameColor, 8);
                            }

                            if (selectedAnimation.Events.TryGetValue(i, out var @event))
                            {
                                drawList.AddRect(framePosition + new Vector2(padding, 0), framePosition + frameSize, frameKeyColor, 8);

                                if (frameRectangle.Contains(ImGui.GetMousePos()) && !info.Hook.IsPopupOpen)
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.Text($"{i}\n\"{@event}\"\n{frameDuration}ms");
                                    ImGui.EndTooltip();
                                }

                                if (frameRectangle.Width > @event.Length * 8)
                                {
                                    drawList.AddText(framePosition + new Vector2(padding * 2, padding * 2), frameKeyColor, @event);
                                }
                            }
                            else
                            {
                                if (frameRectangle.Contains(ImGui.GetMousePos()) && !info.Hook.IsPopupOpen)
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.Text($"{i}\n{frameDuration}ms");
                                    ImGui.EndTooltip();
                                }
                            }
                        }

                        mouseRatio = Calculator.Clamp01(mouseX / (area.X - padding * 2));

                        if (new Rectangle(position, area).Contains(ImGui.GetMousePos()) && ImGui.IsMouseDown(ImGuiMouseButton.Left) && !info.Hook.IsPopupOpen)
                        {
                            info.AnimationProgress = mouseRatio;
                            rate = info.AnimationProgress;
                            info.Hook.Time = info.AnimationProgress * selectedAnimation.AnimationDuration;
                        }

                        Vector2 arrowPosition = position + new Vector2(padding * 3 + (area.X - padding * 5) * rate, 0);

                        drawList.AddTriangleFilled(arrowPosition + new Vector2(-6, 0), arrowPosition + new Vector2(6, 0), arrowPosition + new Vector2(0, 20), arrowColor);
                        drawList.AddLine(arrowPosition, arrowPosition + new Vector2(0, area.Y), arrowColor);
                    }
                }
                else
                {
                    ImGui.Text("No animation selected");
                }

                ImGui.EndChild();

                targetAnimationFrame = _targetFrameForPopup ?? selectedAnimation.Evaluate(mouseRatio * selectedAnimation.AnimationDuration, false).InternalFrame;
                selectedAnimation.Events.TryGetValue(targetAnimationFrame, out string? selectedMessage);

                DrawAddMessageOnRightClick(info, targetAnimationFrame, selectedMessage);
            }

            ImGui.EndChild();
        }

        private void DrawRenamePopup(SpriteInformation info, int frame)
        {
            if (ImGui.BeginPopup($"rename_message_to_frame_{frame}"))
            {
                ImGui.PushItemWidth(170);

                ImGui.Text($"Name for this message:");
                ImGui.InputText("##add_new_message_input", ref _message, 128);

                if (string.IsNullOrEmpty(_message))
                {
                    ImGuiHelpers.SelectedButton("Ok!");
                }
                else if (ImGui.Button("Ok!") || Game.Input.Pressed(MurderInputButtons.Submit))
                {
                    AddMessage(info.SelectedAnimation, frame, _message);

                    _message = string.Empty;

                    ImGui.CloseCurrentPopup();
                }

                ImGui.PopItemWidth();
                ImGui.EndPopup();
            }
        }

        private void DrawAddMessageOnRightClick(SpriteInformation info, int frame, string? message)
        {
            bool open = false;
            if (ImGui.BeginPopupContextItem())
            {
                _targetFrameForPopup ??= frame;

                info.Hook.IsPopupOpen = true;

                string text = string.IsNullOrEmpty(message) ? "Add message..." : "Rename";
                if (ImGui.Selectable(text))
                {
                    open = true;
                }

                if (!string.IsNullOrEmpty(message) && ImGui.Selectable("Delete"))
                {
                    DeleteMessage(info.SelectedAnimation, frame);
                }

                ImGui.EndPopup();
            }
            else
            {
                info.Hook.IsPopupOpen = false;
            }

            if (open)
            {
                ImGui.OpenPopup("add_message_to_frame");
                _message = message ?? string.Empty;
            }

            if (ImGui.BeginPopup("add_message_to_frame"))
            {
                _targetFrameForPopup ??= frame;

                ImGui.PushItemWidth(170);
                info.Hook.IsPopupOpen = true;

                ImGui.Text($"Fire message at frame {_targetFrameForPopup.Value}:");
                ImGui.InputText("##add_new_message_input", ref _message, 128);

                if (string.IsNullOrEmpty(_message))
                {
                    ImGuiHelpers.SelectedButton("Add");
                }
                else if (ImGui.Button("Add") || Game.Input.Pressed(MurderInputButtons.Submit))
                {
                    AddMessage(info.SelectedAnimation, frame, _message);

                    _message = string.Empty;
                    info.Hook.IsPopupOpen = false;

                    ImGui.CloseCurrentPopup();
                }

                ImGui.PopItemWidth();
                ImGui.EndPopup();
            }

            if (!info.Hook.IsPopupOpen)
            {
                _targetFrameForPopup = null;
            }
        }

        private void DrawHeader(SpriteInformation info, int currentFrame, Animation animation)
        {
            StringBuilder text = new();
            if (string.IsNullOrEmpty(info.SelectedAnimation))
            {
                text.Append("Default animation");
            }
            else
            {
                text.Append($"\"{info.SelectedAnimation}\"");
            }

            if (animation.Events.Count == 0)
            {
                text.Append(" | 0 events");
            }
            else
            {
                text.Append($" | {animation.Events.Count} event{((animation.Events.Count) > 1 ? "s" : "")}");
            }

            float currentTime = 0;
            for (int i = 0; i <= currentFrame; i++)
            {
                currentTime += animation.FramesDuration[i];
            }

            text.Append($" | current frame: {currentFrame} ({currentTime/1000f:0.00}s)");

            text.Append($" | {animation.AnimationDuration}s");

            ImGui.TextColored(Game.Profile.Theme.Faded, text.ToString());
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

            /// <summary>
            /// Shortcut for the timeline editor hook.
            /// </summary>
            public TimelineEditorHook Hook => (TimelineEditorHook)Stage.EditorHook;

            [Tooltip("This will create a sound to test in this editor. The actual sound must be added to the entity!")]
            [Default("Add sound to test")]
            [JsonIgnore] // This won't be serializable.
            public Dictionary<string, SoundEventId?> SoundTests = new();
        }
    }
}
