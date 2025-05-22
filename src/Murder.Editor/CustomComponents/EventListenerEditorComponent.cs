using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core.Sounds;
using Murder.Editor.Attributes;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using Murder.Systems.Effects;
using System.Collections.Immutable;
using System.Reflection;

namespace Murder.Editor.CustomComponents
{
    [CustomComponentOf(typeof(EventListenerEditorComponent))]
    internal class EventListenerEditor : CustomComponent
    {
        private static string _eventInput = string.Empty;

        protected override bool DrawAllMembersWithTable(ref object target)
        {
            EventListenerEditorComponent listener = (EventListenerEditorComponent)target;

            (string[] Animations, HashSet<string> Events)? entityProperties = StageHelpers.GetEventsForSelectedEntity();
            
            bool changed = DrawEventsListener(
                ref listener, drawPlayAnimations: true, entityProperties?.Animations, entityProperties?.Events);

            if (changed)
            {
                target = listener;
            }

            return changed;
        }

        public static bool DrawEventsListener(ref EventListenerEditorComponent listener, bool drawPlayAnimations, string[]? animations, HashSet<string>? eventNames)
        {
            bool fileChanged = false;

            ImmutableArray<SpriteEventInfo> events = eventNames is null || eventNames.Count == 0 ?
                listener.Events :
                eventNames.Select(e => new SpriteEventInfo(e)).ToImmutableArray();

            // ======
            // Initialize according to the sprite events.
            // ======
            if (eventNames is not null && eventNames.Count > 0)
            {
                Dictionary<string, int> trackedEvents = [];
                for (int i = 0; i < eventNames.Count; ++i)
                {
                    SpriteEventInfo @event = events[i];

                    trackedEvents[@event.Id] = i;
                }

                foreach (SpriteEventInfo info in listener.Events)
                {
                    if (trackedEvents.TryGetValue(info.Id, out int index))
                    {
                        events = events.SetItem(index, info);
                        continue;
                    }

                    events = events.Add(info);
                }
            }
            
            // ======
            // Add new event.
            // ======
            string addEventPopupId = "popup_add_event_sprite";
            if (ImGuiHelpers.IconButton('\uf055', "add_events"))
            {
                ImGui.OpenPopup(addEventPopupId);
            }

            if (ImGui.BeginPopup(addEventPopupId))
            {
                ImGui.InputText("##add_new_event_input", ref _eventInput, 128);

                if (string.IsNullOrEmpty(_eventInput) || events.Any(e => e.Id.Equals(_eventInput, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ImGuiHelpers.SelectedButton("Track event!");
                }
                else if (ImGui.Button("Track event!"))
                {
                    events = events.Add(new(_eventInput));
                    fileChanged = true;

                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            // ======
            // Things get interesting here: play an animation.
            // ======
            if (animations is not null && drawPlayAnimations)
            {
                SelectAnimation(animations);
            }

            // ======
            // Draw current events.
            // ======
            {
                using TableMultipleColumns table = new($"events_editor_component",
                    flags: ImGuiTableFlags.NoBordersInBody,
                    (-1, ImGuiTableColumnFlags.WidthFixed), (-1, ImGuiTableColumnFlags.WidthFixed), (-1, ImGuiTableColumnFlags.WidthStretch), (-1, ImGuiTableColumnFlags.WidthFixed));

                for (int i = 0; i < events.Length; ++i)
                {
                    SpriteEventInfo info = events[i];

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    if (eventNames is not null && eventNames.Contains(info.Id))
                    {
                        ImGuiHelpers.SelectedIconButton('', Game.Profile.Theme.Faded);
                    }
                    else
                    {
                        if (ImGuiHelpers.IconButton('\uf2ed', $"delete_event_listener_{i}"))
                        {
                            events = events.RemoveAt(i);
                            fileChanged = true;
                        }
                    }

                    ImGui.TableNextColumn();

                    if (ImGui.Selectable(info.Id))
                    {
                        ImGui.SetClipboardText(info.Id);
                    }

                    ImGui.TableNextColumn();

                    ImGui.PushID($"##dropdown_listener_{i}");

                    if (CustomField.DrawValue(ref info, fieldName: nameof(SpriteEventInfo.Sound)))
                    {
                        events = events.SetItem(i, info);
                        fileChanged = true;
                    }

                    ImGui.PopID();

                    ImGui.TableNextColumn();

                    SoundLayer? layer = info.Persisted;
                    if (layer is null)
                    {
                        bool isPersist = false;
                        if (ImGui.Checkbox($"##dropdown_checkbox_{i}", ref isPersist))
                        {
                            events = events.SetItem(i, info.WithPersist(SoundLayer.Ambience));
                            fileChanged = true;
                        }
                    }
                    else
                    {
                        ImGui.PushID($"##dropdown_layer_{i}");

                        if (CustomField.DrawValue(ref info, fieldName: nameof(SpriteEventInfo.Persisted)))
                        {
                            events = events.SetItem(i, info);
                            fileChanged = true;
                        }

                        ImGui.PopID();
                    }

                    ImGui.SameLine();
                    ImGuiHelpers.HelpTooltip("Whether this event should persist");

                    if (info.Interactions is null)
                    {
                        if (ImGuiHelpers.ColoredIconButton('\uf70c', $"add_interactions_{i}", isActive: true))
                        {
                            events = events.SetItem(i, info with { Interactions = [] });
                            fileChanged = true;
                        }

                        ImGuiHelpers.HelpTooltip("Play interactions on event");
                    }
                    else
                    {
                        ImGuiHelpers.ColoredIconButton('\uf70c', $"add_interactions_{i}", isActive: false);

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();
                        ImGui.TableNextColumn();

                        if (CustomField.DrawValue(ref info, fieldName: nameof(SpriteEventInfo.Interactions)))
                        {
                            events = events.SetItem(i, info);
                            fileChanged = true;
                        }
                    }
                }
            }

            if (fileChanged)
            {
                SpriteAsset? spriteAsset = StageHelpers.GetSpriteAssetForSelectedEntity();
                if (spriteAsset is not null)
                {
                    EditorServices.SaveAssetWhenSelectedAssetIsSaved(spriteAsset.Guid);
                }

                listener = listener with
                {
                    Events = events.Where(s => (eventNames is not null && !eventNames.Contains(s.Id)) || s.Sound is not null || eventNames is null).ToImmutableArray()
                };
            }

            return fileChanged;
        }

        private static int _lastAnimationSelected = 0;

        private static void SelectAnimation(string[] animations)
        {
            ImGui.SameLine();
            ImGui.Text("\uf03d");

            ImGui.SameLine();
            if (_lastAnimationSelected >= animations.Length)
            {
                _lastAnimationSelected = 0;
            }

            ImGui.Combo("##select_animation", ref _lastAnimationSelected, animations, animations.Length);
            ImGui.SameLine();

            if (ImGuiHelpers.IconButton('\uf04b', "##select_animation_for_event"))
            {
                StageHelpers.ToggleSystem(typeof(EventListenerSystem), true);

                StageHelpers.AddComponentsOnSelectedEntityForWorldOnly(
                    new AnimationOverloadComponent(animations[_lastAnimationSelected], loop: false, ignoreFacing: true));
            }

            ImGui.SameLine();
            if (ImGuiHelpers.IconButton('\uf04c', "##unselect_animation_for_event"))
            {
                StageHelpers.ToggleSystem(typeof(EventListenerSystem), false);
            }
        }
    }
}