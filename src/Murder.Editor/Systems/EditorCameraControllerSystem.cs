using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Input;
using Murder.Editor.Components;
using Murder.Editor.Utilities;

namespace Murder.Editor.Systems
{
    public class EditorCameraControllerSystem : IStartupSystem, IUpdateSystem
    {
        /// <summary>
        /// Track cursor position.
        /// </summary>
        private Point _previousCursorPosition = Point.Zero;

        public ValueTask Start(Context context)
        {
            var hook = context.World.GetUnique<EditorComponent>().EditorHook;
            hook.CurrentZoomLevel = EditorHook.STARTING_ZOOM;
            return default;
        }

        public ValueTask Update(Context context)
        {
            var hook = context.World.GetUnique<EditorComponent>().EditorHook;
            if (!hook.ShowDebug)
                return default;

            var bounds = new Rectangle(hook.Offset, hook.StageSize);
            var camera = ((MonoWorld)context.World).Camera;

            camera.Zoom = hook.ScrollPositions[hook.CurrentZoomLevel];

            // Only when hovered
            if (bounds.Contains(Game.Input.CursorPosition))
            {
                hook.CurrentZoomLevel = Math.Clamp(hook.CurrentZoomLevel + MathF.Sign(-Game.Input.ScrollWheel), 0, hook.ScrollPositions.Length - 1);

                var currentPosition = hook.CursorScreenPosition;
                if (Game.Input.Down(MurderInputButtons.RightClick))
                {
                    foreach (var e in context.World.GetEntitiesWith(typeof(CameraFollowComponent)))
                    {
                        e.SetCameraFollow(false);
                    }

                    Vector2 delta = (_previousCursorPosition - currentPosition).ToVector2()/camera.Zoom;
                    camera.Position = camera.Position + delta;
                    hook.Cursor = EditorHook.CursorStyle.Eye;
                }

                _previousCursorPosition = currentPosition;
            }

            return default;
        }
    }
}
