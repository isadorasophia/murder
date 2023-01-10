using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Services;
using SharpFont;

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

        public static bool DrawPolygonHandles(Polygon polygon, RenderContext render, Vector2 position, Vector2 cursorPosition, string id, Color color, out Polygon result)
        {
            result = polygon;
            bool modified = false;  
            
            Point center = polygon.GetBoundingBox().Center.Point;

            for (int i = 0; i < polygon.Vertices.Length - 1; i++)
            {
                Point pointA = polygon.Vertices[i];
                Point pointB = polygon.Vertices[i + 1];
                RenderServices.DrawLine(render.DebugSpriteBatch, pointA + position, pointB + position, color);
                if (EditorServices.DrawHandle($"{id}_point_{i}", render, cursorPosition, position + pointA, color, out Vector2 newPosition))
                {
                    modified = true;
                    var newVertices = polygon.Vertices.ToArray();
                    newVertices[i] = newPosition.Point;
                    result = new Polygon(newVertices);
                }
            }
            {
                var lastVert = polygon.Vertices[polygon.Vertices.Length - 1];
                RenderServices.DrawLine(render.DebugSpriteBatch, lastVert + position, polygon.Vertices[0] + position, color);

                if (EditorServices.DrawHandle($"{id}_point_{polygon.Vertices.Length}", render, cursorPosition, position + lastVert, color, out Vector2 newPosition))
                {
                    modified = true;
                    var newVertices = polygon.Vertices.ToArray();
                    newVertices[polygon.Vertices.Length-1] = newPosition.Point;
                    result = new Polygon(newVertices);
                }
            }



            return modified;
        }
    }
}
