using Murder.Components;
using Murder.Editor.Attributes;
using Murder.Editor.Utilities;
using System.Reflection;
using System.Collections.Immutable;
using Murder.Editor.CustomFields;
using ImGuiNET;
using Murder.Editor.ImGuiExtended;
using Murder.Utilities;

namespace Murder.Editor.CustomComponents
{
    [CustomComponentOf(typeof(EventListenerEditorComponent))]
    internal class EventListenerEditor : CustomComponent
    {
        private static string _eventInput = string.Empty;

        protected override bool DrawAllMembersWithTable(ref object target)
        {
            bool fileChanged = false;

            FieldInfo field = typeof(EventListenerEditorComponent).GetField(nameof(EventListenerEditorComponent.Events))!;

            var entityProperties = StageHelpers.GetSpriteEventsForSelectedEntity();

            HashSet<string>? eventNames = entityProperties?.Events;
            string[]? animations = entityProperties?.Animations;

            EventListenerEditorComponent listener = (EventListenerEditorComponent)target;
            ImmutableArray<SpriteEventInfo> events = listener.Events;

            // ======
            // Initialize events, if necessary.
            // ======
            if (listener.Events.Length == 0 && eventNames is not null && eventNames.Count > 0)
            {
                var builder = ImmutableArray.CreateBuilder<SpriteEventInfo>();
                foreach (string e in eventNames)
                {
                    builder.Add(new(e));
                }

                events = builder.ToImmutable();
                fileChanged = true;
            }

            // ======
            // Add new event.
            // ======
            ImGui.SameLine();
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
            if (animations is not null)
            {
                SelectAnimation(listener, animations);
            }


            if (animations is string[] allAnimations)

            // ======
            // Draw current events.
            // ======
            {
                using TableMultipleColumns table = new($"events_editor_component",
                    flags: ImGuiTableFlags.NoBordersInBody,
                    (-1, ImGuiTableColumnFlags.WidthFixed), (50, ImGuiTableColumnFlags.WidthFixed), (-1, ImGuiTableColumnFlags.WidthStretch));

                for (int i = 0; i < listener.Events.Length; ++i)
                {
                    SpriteEventInfo info = listener.Events[i];

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    if (eventNames is not null && eventNames.Contains(info.Id))
                    {
                        ImGuiHelpers.SelectedIconButton('\uf2ed');
                    }
                    else
                    {
                        if (ImGuiHelpers.IconButton('\uf2ed', "delete_event"))
                        {
                            events = events.RemoveAt(i);
                            fileChanged = true;
                        }
                    }

                    ImGui.TableNextColumn();

                    ImGui.Text(info.Id);
                    ImGui.TableNextColumn();

                    ImGui.PushID($"##dropdown_listener_{i}");

                    if (CustomField.DrawValue(ref info, fieldName: nameof(SpriteEventInfo.Sound)))
                    {
                        events = events.SetItem(i, info);
                        fileChanged = true;
                    }

                    ImGui.PopID();
                }
            }

            if (fileChanged)
            {
                field.SetValue(target, events);
            }

            return fileChanged;
        }

        private static int _lastAnimationSelected = 0;

        private void SelectAnimation(EventListenerEditorComponent listener, string[] animations)
        {
            ImGui.SameLine();
            ImGui.Text("\uf03d");

            ImGui.SameLine();
            if (_lastAnimationSelected >=  animations.Length)
            {
                _lastAnimationSelected = 0;
            }

            ImGui.Combo("##select_animation", ref _lastAnimationSelected, animations, animations.Length);
            ImGui.SameLine();
            
            if (ImGuiHelpers.IconButton('\uf04b', "##select_animation_for_event"))
            {
                StageHelpers.AddComponentsOnSelectedEntityForWorldOnly(
                    new AnimationOverloadComponent(animations[_lastAnimationSelected], loop: false, ignoreFacing: true),
                    listener.ToEventListener());
            }
        }
    }
}
