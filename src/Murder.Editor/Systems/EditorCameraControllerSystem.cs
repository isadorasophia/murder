using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Input;
using Murder.Editor.Components;
using Murder.Editor.Core;
using Murder.Systems;
using System.Numerics;

namespace Murder.Editor.Systems
{
    [EditorSystem]
    [Requires(typeof(CursorSystem), typeof(EditorSystem))]
    [Filter(ContextAccessorFilter.None)]
    public class EditorCameraControllerSystem : IUpdateSystem
    {
        /// <summary>
        /// Track cursor position.
        /// </summary>
        private Point _previousCursorPosition = Point.Zero;

        public void Update(Context context)
        {
            var hook = context.World.GetUnique<EditorComponent>().EditorHook;
            if (!hook.ShowDebug)
                return;

            var camera = ((MonoWorld)context.World).Camera;

            camera.Zoom = hook.ScrollPositions[hook.CurrentZoomLevel];

            // Only when hovered
            if (!Game.Input.MouseConsumed && hook.IsMouseOnStage)
            {
                if (Game.Input.ScrollWheel != 0)
                {
                    hook.CurrentZoomLevel = Math.Clamp(hook.CurrentZoomLevel + MathF.Sign(-Game.Input.ScrollWheel), 0, hook.ScrollPositions.Length - 1);
                }

                if (IsDragging())
                {
                    foreach (var e in context.World.GetEntitiesWith(typeof(CameraFollowComponent)))
                    {
                        e.SetCameraFollow(false);
                    }

                    Vector2 delta = (_previousCursorPosition - hook.CursorScreenPosition).ToVector2() / (camera.Zoom * Architect.EditorSettings.DpiScale);

                    camera.Position += delta;
                    hook.Cursor = CursorStyle.Eye;
                }
                else
                {
                    bool noEntitiesSelected = hook.AllSelectedEntities.IsEmpty;

                    if (ImGui.GetIO().WantTextInput)
                    {
                        // Handled by ImGui
                    }
                    else if (!Game.Input.Down(Keys.LeftControl))
                    {
                        Vector2 cameraMovement = Architect.Input.GetAxis(MurderInputAxis.EditorCamera).Value * Game.DeltaTime * Architect.EditorSettings.WasdCameraSpeed * Math.Clamp((1f / hook.CurrentZoomLevel), .75f, 10f);
                        if (Game.Input.Down(Keys.LeftShift))
                        {
                            cameraMovement *= 0.25f;
                        }
                        camera.Position += cameraMovement;
                    }
                }

                _previousCursorPosition = hook.CursorScreenPosition;
            }
        }

        public static bool IsDragging() => Game.Input.Down(MurderInputButtons.MiddleClick) || Game.Input.Down(MurderInputButtons.Space);
    }
}