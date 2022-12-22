using Microsoft.Xna.Framework.Input;
using Bang.Contexts;
using Bang.Systems;
using Murder.Editor.Attributes;
using Murder.Core.Geometry;
using Murder;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor;
using Murder.Core;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Assets;
using System;
using Bang;
using Murder.Utilities;
using ImGuiNET;
using Murder.Components;
using Murder.Editor.Services;

namespace Road.Editor.Systems
{
    [OnlyShowOnDebugView]
    [DoNotPause]
    public class EditorStartOnCursorSystem : IStartupSystem, IUpdateSystem, IMonoRenderSystem
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
                _selectPosition = CameraServices.GetCursorWorldPosition(world);
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

                    RenderServices.DrawCircle(render.DebugSpriteBatch, position, size, 10, color);
                }
            }
        }

        /// <summary>
        /// This draws and create a new empty entity if the user prompts.
        /// </summary>
        private bool DrawStartHere(World world)
        {
            ImGui.PushID("start_from_cursor");

            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.Selectable("Start playing here!"))
                {
                    Architect.EditorSettings.TestWorldPosition = CameraServices.GetCursorWorldPosition((MonoWorld)world);
                    Architect.Instance.PlayGame(quickplay: false, startingScene: world.Guid());
                }

                ImGui.EndPopup();
            }

            ImGui.PopID();

            return true;
        }
    }
}
