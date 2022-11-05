using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Services;

namespace Murder.Editor.Services
{
    public static class EditorServices
    {
        static string _draggingHandle = String.Empty;
        static Vector2 _dragOffset;

        public static bool DrawHandle(string id, RenderContext render, Vector2 cursorPosition, Vector2 position, Color color, out Vector2 newPosition)
        {
            var circle = new Circle(position.X - 2, position.Y - 2, 3);

            if (_draggingHandle == id)
            {

                if (!Game.Input.Down(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = String.Empty;
                }

                RenderServices.DrawCircle(render.DebugSpriteBatch, position, 4, 8, Color.White);
                newPosition = cursorPosition + _dragOffset;
                return true;
            }

            if (circle.Contains(cursorPosition))
            {
                RenderServices.DrawCircle(render.DebugSpriteBatch, position, 3, 8, Color.Lerp(color, Color.White, 0.5f));

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _dragOffset = position - cursorPosition;
                }
            }
            else
            {
                RenderServices.DrawCircle(render.DebugSpriteBatch, position, 2, 6, color);
            }

            newPosition = position;
            return false;
        }
    }
}
