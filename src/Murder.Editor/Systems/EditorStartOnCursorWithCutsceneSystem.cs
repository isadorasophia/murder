using Bang;
using Bang.Contexts;
using Bang.StateMachines;
using Bang.Systems;
using ImGuiNET;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Attributes;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Services;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.Systems
{
    [OnlyShowOnDebugView]
    [DoNotPause]
    [StoryEditor]
    [Filter(ContextAccessorFilter.None)]
    public class EditorStartOnCursorWithCutsceneSystem : IStartupSystem, IUpdateSystem, IMurderRenderSystem
    {
        public void Start(Context context)
        {
            Guid guid = context.World.Guid();
            if (guid == Guid.Empty)
            {
                // Deactivate itself if this not belongs to a world asset.
                context.World.DeactivateSystem<EditorStartOnCursorSystem>();
                return;
            }

            // Hook settings from previous session.
            if (Architect.EditorSettings.TestStartTime is float time)
            {
                _time = time;
            }
        }

        public void Update(Context context)
        {
            if (Game.Input.Pressed(MurderInputButtons.RightClick))
            {
                _selectedPosition = EditorCameraServices.GetCursorWorldPosition((MonoWorld)context.World);
            }
        }

        public void Draw(RenderContext render, Context context)
        {
            DrawStartHere(context.World);
        }

        private Vector2 _selectedPosition = new();

        private int _day = 1;
        private float _time = 0;

        /// <summary>
        /// This draws and create a new empty entity if the user prompts.
        /// </summary>
        private bool DrawStartHere(World world)
        {
            if (ImGui.BeginPopupContextItem("GameplayContextMenu", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoReopen))
            {
                ImGui.Separator();
                if (ImGui.Selectable("\ue131 Start playing with..."))
                {
                    ImGui.CloseCurrentPopup();
                    ImGui.EndPopup();

                    ImGui.OpenPopup("play_from_cutscene");
                }
                else
                {
                    ImGui.EndPopup();
                }
            }

            if (ImGui.BeginPopup("play_from_cutscene"))
            {
                if (DrawSelectCutscenePopup(world))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            return true;
        }

        private bool DrawSelectCutscenePopup(World world)
        {
            ImGui.BeginChild("play_from_cutscene_popup",
                size: new Vector2(ImGui.GetFontSize() * 20, ImGui.GetFontSize() * 11.5f), ImGuiChildFlags.None);

            bool start = ImGuiHelpers.PrettySelectableWithIcon(label: "Play from here!", true);

            ImGui.PushItemWidth(-1);
            ImGui.Text("\uf017 Day");

            bool modified = false;
            modified |= ImGui.SliderInt("day", ref _day, 1, 10);
            ImGui.Text("\uf017 Time");

            modified |= ImGui.SliderFloat("", ref _time, 0, 1);
            ImGui.PopItemWidth();

            ImGui.EndChild();

            if (modified)
            {
                Architect.EditorSettings.TestWorldPosition = _selectedPosition.Point();
                Architect.EditorSettings.TestStartDay = _day;
                Architect.EditorSettings.TestStartTime = _time;
            }

            if (start)
            {
                // start playing!
                Architect.Instance.QueueStartPlayingGame(quickplay: false, startingScene: world.Guid());
                return true;
            }

            return false;
        }
    }
}