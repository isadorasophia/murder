using Bang.Contexts;
using Bang.Systems;
using Murder.Editor.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Core;
using Murder.Services;
using Bang;
using ImGuiNET;
using Murder.Editor.Services;
using Murder.Editor.ImGuiExtended;
using Bang.StateMachines;
using Murder.Editor.CustomComponents;
using Bang.Components;
using Murder.Utilities;

namespace Murder.Editor.Systems
{
    [OnlyShowOnDebugView]
    [DoNotPause]
    [StoryEditor]
    [Filter(ContextAccessorFilter.None)]
    public class EditorStartOnCursorWithCutsceneSystem : IStartupSystem, IUpdateSystem, IMonoRenderSystem
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

            var testStartEntityAndComponent = Architect.EditorSettings.TestStartWithEntityAndComponent;
            if (testStartEntityAndComponent is not null && testStartEntityAndComponent.Value.Component is IStateMachineComponent sm)
            {
                _cutsceneGuid = testStartEntityAndComponent.Value.Entity;
                _stateMachine = sm;
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

        private float _time = 0;
        private Guid _cutsceneGuid = Guid.Empty;
        private IStateMachineComponent? _stateMachine = null;

        /// <summary>
        /// This draws and create a new empty entity if the user prompts.
        /// </summary>
        private bool DrawStartHere(World world)
        {
            if (ImGui.BeginPopupContextItem())
            {
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
                size: new System.Numerics.Vector2(ImGui.GetFontSize() * 20, ImGui.GetFontSize() * (_stateMachine is null ? 11.5f : 30)), 
                border: false);

            bool start = ImGuiHelpers.PrettySelectableWithIcon(label: "Play from here!", true);

            ImGui.Text("\uf017 Time");

            ImGui.PushItemWidth(-1);
            _ = ImGui.SliderFloat("", ref _time, 0, 1);
            ImGui.PopItemWidth();

            ImGui.Text("\uf57d Target");
            SearchBox.SearchInstanceInWorld(ref _cutsceneGuid, EditorStoryServices.GetWorldAsset(world));

            ImGui.Text("\uf03d Cutscene");

            string? stateMachineName = _stateMachine?.GetType().GetGenericArguments().FirstOrDefault()?.Name;
            if (SearchBox.SearchStateMachines(initialValue: stateMachineName) is Type sm)
            {
                _stateMachine = Activator.CreateInstance(sm) as IStateMachineComponent;
            }

            if (_stateMachine is not null)
            {
                CustomComponent.ShowEditorOf(_stateMachine);
            }

            ImGui.EndChild();

            if (start)
            {
                // start playing!
                Architect.EditorSettings.TestWorldPosition = _selectedPosition.Point;
                Architect.EditorSettings.TestStartTime = _time;
                Architect.EditorSettings.TestStartWithEntityAndComponent = (_cutsceneGuid, _stateMachine);

                Architect.EditorSettings.UseCustomCutscene = true;

                Architect.Instance.PlayGame(quickplay: false, startingScene: world.Guid());

                return true;
            }

            return false;
        }
    }
}
