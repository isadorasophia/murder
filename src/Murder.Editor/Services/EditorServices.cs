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
            var circle = new Circle(position.X - 2, position.Y - 2, 3 + 6 * (1 / render.Camera.Zoom));

            if (_draggingHandle == id)
            {
                if (!Game.Input.Down(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = String.Empty;
                }

                RenderServices.DrawRectangle(render.DebugSpriteBatch, new Rectangle(position - new Vector2(circle.Radius/2f), new Vector2(circle.Radius)), color);

                newPosition = cursorPosition + _dragOffset;
                return true;
            }

            if (circle.Contains(cursorPosition))
            {
                RenderServices.DrawRectangle(render.DebugSpriteBatch, new Rectangle(position - new Vector2(circle.Radius), new Vector2(circle.Radius*2)), color);

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _dragOffset = position - cursorPosition;
                }
            }
            else
            {
                RenderServices.DrawRectangle(render.DebugSpriteBatch, new Rectangle(position- new Vector2(circle.Radius / 2f), new Vector2(circle.Radius)), color);
            }

            newPosition = position;
            return false;
        }

        /// <summary>
        /// Drags a rectangle area around.
        /// </summary>
        public static bool DragArea(string id, Vector2 cursorPosition, Rectangle area, Color color, out Vector2 newPosition)
        {
            if (_draggingHandle == id)
            {
                if (!Game.Input.Down(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = string.Empty;
                }

                newPosition = cursorPosition + _dragOffset;
                return true;
            }

            if (area.Contains(cursorPosition))
            {
                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _dragOffset = area.TopLeft - cursorPosition;
                }
            }

            newPosition = area.TopLeft;
            return false;
        }
    }
}
