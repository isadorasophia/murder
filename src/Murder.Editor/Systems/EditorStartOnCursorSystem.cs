using Bang.Contexts;
using Bang.Systems;
using Murder.Editor.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Core;
using Murder.Services;
using Bang;
using Murder.Utilities;
using ImGuiNET;
using Murder.Editor.Services;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Editor.EditorCore;

namespace Murder.Editor.Systems
{
    [OnlyShowOnDebugView]
    [DoNotPause]
    [WorldEditor(startActive: true)]
    [Filter(ContextAccessorFilter.None)]
    public class EditorStartOnCursorSystem : IStartupSystem, IUpdateSystem, IMurderRenderSystem
    {
        private bool _pressedControl = false;

        /// <summary>
        /// Tracks tween for <see cref="_selectPosition"/>.
        /// </summary>
        private float _tweenStart;

        /// <summary>
        /// This is the position currently selected by the cursor.
        /// </summary>
        private Point? _selectPosition;

        public void Start(Context context)
        {
            Guid guid = context.World.Guid();
            if (guid == Guid.Empty)
            {
                // Deactivate itself if this not belongs to a world asset.
                context.World.DeactivateSystem<EditorStartOnCursorSystem>();
            }
        }

        public void Update(Context context)
        {
            MonoWorld world = (MonoWorld)context.World;
            
            _pressedControl = Game.Input.Down(MurderInputButtons.Ctrl);
            if (_pressedControl)
            {
                _tweenStart = Game.Now;
                _selectPosition = EditorCameraServices.GetCursorWorldPosition(world);
            }
            
            if (_pressedControl && Game.Input.Pressed(MurderInputButtons.RightClick))
            {
                Architect.EditorSettings.TestWorldPosition = _selectPosition;
                Architect.Instance.PlayGame(quickplay: false, startingScene: world.WorldAssetGuid);
            }
        }

        public void Draw(RenderContext render, Context context)
        {
            DrawSelectionTween(render);
            DrawStartHere(context.World);
        }

        private void DrawSelectionTween(RenderContext render)
        {
            if (_selectPosition is Point position)
            {
                float tween = Ease.ZeroToOne(Ease.BackOut, 2f, _tweenStart);
                if (tween == 1)
                {
                    _selectPosition = null;
                }
                else
                {
                    float expand = (1 - tween) * 2;

                    float startAlpha = .9f;
                    Color color = Game.Profile.Theme.HighAccent * (startAlpha - startAlpha * tween);

                    float size = 3 + expand;

                    RenderServices.DrawCircle(render.DebugBatch, position, size, 10, color);
                }
            }
        }

        /// <summary>
        /// This draws and create a new empty entity if the user prompts.
        /// </summary>
        private bool DrawStartHere(World world)
        {
            EditorHook hook = world.GetUnique<EditorComponent>().EditorHook;

            ImGui.PushID("start_from_cursor");
            if (ImGui.BeginPopupContextItem())
            {
                hook.IsPopupOpen = true;

                if (ImGui.Selectable("Start playing here!"))
                {
                    hook.Cursor = CursorStyle.Normal;

                    Architect.EditorSettings.TestWorldPosition = EditorCameraServices.GetCursorWorldPosition((MonoWorld)world);
                    Architect.Instance.PlayGame(quickplay: false, startingScene: world.Guid());
                }

                ImGui.EndPopup();
            }
            else
            {
                hook.IsPopupOpen = false;
            }

            ImGui.PopID();

            return true;
        }
    }
}
