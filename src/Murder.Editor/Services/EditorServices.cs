using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.Services
{
    public static class EditorServices
    {
        public enum DragStyle
        {
            None,
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft,
            Left,
            Move,
        }
        private static string _draggingHandle = String.Empty;
        private static DragStyle _draggingStyle = DragStyle.None;
        private static int _draggingAnchor = -1;
        private static Vector2 _dragOffset;

        public static bool DrawHandle(string id, RenderContext render, Vector2 cursorPosition, Vector2 position, Color color, out Vector2 newPosition)
        {
            var circle = new Circle(position.X - 2, position.Y - 2, 3 + 6 * (1 / render.Camera.Zoom));

            if (_draggingHandle == id)
            {
                if (!Game.Input.Down(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = String.Empty;
                }

                RenderServices.DrawRectangle(render.DebugFxBatch, new Rectangle(position - new Vector2(circle.Radius / 2f), new Vector2(circle.Radius)), color);

                newPosition = cursorPosition + _dragOffset;
                return true;
            }

            if (circle.Contains(cursorPosition))
            {
                RenderServices.DrawRectangle(render.DebugFxBatch, new Rectangle(position - new Vector2(circle.Radius), new Vector2(circle.Radius * 2)), color);

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _dragOffset = position - cursorPosition;
                }
            }
            else
            {
                RenderServices.DrawRectangle(render.DebugFxBatch, new Rectangle(position - new Vector2(circle.Radius / 2f), new Vector2(circle.Radius)), color);
            }

            newPosition = position;
            return false;
        }
        public static bool BoxHandle(string id, RenderContext render, Vector2 cursorPosition, Rectangle rectangle, Color color, out Vector2 newPosition)
        {
            if (_draggingHandle == id)
            {
                if (!Game.Input.Down(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = String.Empty;
                }

                RenderServices.DrawRectangle(render.DebugFxBatch, rectangle, color);

                newPosition = cursorPosition + _dragOffset;
                return true;
            }

            if (rectangle.Contains(cursorPosition))
            {
                RenderServices.DrawRectangle(render.DebugFxBatch, rectangle, color);

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _dragOffset = rectangle.TopLeft - cursorPosition;
                }
            }
            else
            {
                // RenderServices.DrawRectangle(render.DebugSpriteBatch, rectangle, color);
            }

            newPosition = rectangle.TopLeft;
            return false;
        }

        public static DragStyle LastDragStyle => _draggingHandle == string.Empty ? DragStyle.None : _draggingStyle;

        public static bool BoxHandle(string id, RenderContext render, Vector2 cursorPosition, IntRectangle rectangle, Color color, out IntRectangle newRectangle, bool glow, out bool hover)
        {
            hover = false;

            if (glow)
            {
                RenderServices.DrawRectangle(render.DebugFxBatch, rectangle, color * (0.45f + 0.2f * MathF.Sin(Game.NowUnscaled * 5)));
            }
            else
            {
                RenderServices.DrawRectangle(render.DebugFxBatch, rectangle, color * (0.45f));
            }

            IntRectangle topLeftHandle = new IntRectangle(new Point(rectangle.Left - 1, rectangle.Top - 1), new Point(3, 3));
            IntRectangle topHandle = new IntRectangle(rectangle.TopLeft, new Point(rectangle.Width, 1));
            IntRectangle topRightHandle = new IntRectangle(new Point(rectangle.Right - 2, rectangle.Top - 1), new Point(3, 3));
            IntRectangle rightHandle = new IntRectangle(new Point(rectangle.Right - 1, rectangle.Top), new Point(1, rectangle.Height));
            IntRectangle bottomRightHandle = new IntRectangle(new Point(rectangle.Right - 2, rectangle.Bottom - 2), new Point(3, 3));
            IntRectangle bottomHandle = new IntRectangle(new Point(rectangle.Left, rectangle.Bottom - 1), new Point(rectangle.Width, 1));
            IntRectangle bottomLeftHandle = new IntRectangle(new Point(rectangle.Left - 1, rectangle.Bottom - 2), new Point(3, 3));
            IntRectangle leftHandle = new IntRectangle(rectangle.TopLeft, new Point(1, rectangle.Height));

            if (_draggingHandle == id)
            {
                if (!Game.Input.Down(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = String.Empty;
                }

                var target = (cursorPosition + _dragOffset).Point();
                switch (_draggingStyle)
                {
                    case DragStyle.Move:
                        newRectangle = new IntRectangle(target, rectangle.Size);
                        RenderServices.DrawRectangle(render.DebugFxBatch, rectangle, color);
                        break;

                    case DragStyle.TopLeft:
                        newRectangle = IntRectangle.FromCoordinates(target, rectangle.BottomRight);
                        RenderServices.DrawRectangle(render.DebugBatch, topLeftHandle, Color.White);
                        break;
                    case DragStyle.Top:
                        newRectangle = IntRectangle.FromCoordinates(target.Y, rectangle.Bottom, rectangle.Left, rectangle.Right);
                        RenderServices.DrawRectangle(render.DebugBatch, topHandle, Color.White);
                        break;
                    case DragStyle.TopRight:
                        newRectangle = IntRectangle.FromCoordinates(target.Y, rectangle.Bottom, rectangle.Left, target.X);
                        RenderServices.DrawRectangle(render.DebugBatch, topRightHandle, Color.White);
                        break;
                    case DragStyle.Right:
                        newRectangle = IntRectangle.FromCoordinates(rectangle.Top, rectangle.Bottom, rectangle.Left, target.X);
                        RenderServices.DrawRectangle(render.DebugBatch, rightHandle, Color.White);
                        break;
                    case DragStyle.BottomRight:
                        newRectangle = IntRectangle.FromCoordinates(rectangle.Top, target.Y, rectangle.Left, target.X);
                        RenderServices.DrawRectangle(render.DebugBatch, bottomRightHandle, Color.White);
                        break;
                    case DragStyle.Bottom:
                        newRectangle = IntRectangle.FromCoordinates(rectangle.Top, target.Y, rectangle.Left, rectangle.Right);
                        RenderServices.DrawRectangle(render.DebugBatch, bottomHandle, Color.White);
                        break;
                    case DragStyle.BottomLeft:
                        newRectangle = IntRectangle.FromCoordinates(rectangle.Top, target.Y, target.X, rectangle.Right);
                        RenderServices.DrawRectangle(render.DebugBatch, bottomLeftHandle, Color.White);
                        break;
                    case DragStyle.Left:
                        newRectangle = IntRectangle.FromCoordinates(rectangle.Top, rectangle.Bottom, target.X, rectangle.Right);
                        RenderServices.DrawRectangle(render.DebugBatch, leftHandle, Color.White);
                        break;
                    case DragStyle.None:
                    default:
                        // Undefined move style?
                        newRectangle = rectangle;
                        break;
                }

                return true;
            }

            Point cursor = cursorPosition.Point();
            if (topLeftHandle.Contains(cursor))
            {
                RenderServices.DrawRectangle(render.DebugBatch, topLeftHandle, Color.Lerp(color, Color.White, 0.5f));

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _draggingStyle = DragStyle.TopLeft;
                    _dragOffset = rectangle.TopLeft - cursorPosition;
                }
            }
            else if (topRightHandle.Contains(cursor))
            {
                RenderServices.DrawRectangle(render.DebugBatch, topRightHandle, Color.Lerp(color, Color.White, 0.5f));

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _draggingStyle = DragStyle.TopRight;
                    _dragOffset = rectangle.TopRight - cursorPosition;
                }
            }
            else if (bottomRightHandle.Contains(cursor))
            {
                RenderServices.DrawRectangle(render.DebugBatch, bottomRightHandle, Color.Lerp(color, Color.White, 0.5f));

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _draggingStyle = DragStyle.BottomRight;
                    _dragOffset = rectangle.BottomRight - cursorPosition;
                }
            }
            else if (bottomLeftHandle.Contains(cursor))
            {
                RenderServices.DrawRectangle(render.DebugBatch, bottomLeftHandle, Color.Lerp(color, Color.White, 0.5f));

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _draggingStyle = DragStyle.BottomLeft;
                    _dragOffset = rectangle.BottomLeft - cursorPosition;
                }
            }
            else if (topHandle.Contains(cursor))
            {
                RenderServices.DrawRectangle(render.DebugBatch, topHandle, Color.Lerp(color, Color.White, 0.5f));

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _draggingStyle = DragStyle.Top;
                    _dragOffset = new Vector2(rectangle.Left, cursorPosition.Y) - cursorPosition;
                }
            }
            else if (rightHandle.Contains(cursor))
            {
                RenderServices.DrawRectangle(render.DebugBatch, rightHandle, Color.Lerp(color, Color.White, 0.5f));

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _draggingStyle = DragStyle.Right;
                    _dragOffset = new Vector2(rectangle.Right, rectangle.Top) - cursorPosition;
                }
            }
            else if (bottomHandle.Contains(cursor))
            {
                RenderServices.DrawRectangle(render.DebugBatch, bottomHandle, Color.Lerp(color, Color.White, 0.5f));

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _draggingStyle = DragStyle.Bottom;
                    _dragOffset = new Vector2(rectangle.Left, rectangle.Bottom) - cursorPosition;
                }
            }
            else if (leftHandle.Contains(cursor))
            {
                RenderServices.DrawRectangle(render.DebugBatch, leftHandle, Color.Lerp(color, Color.White, 0.5f));

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _draggingStyle = DragStyle.Left;
                    _dragOffset = new Vector2(rectangle.Left, rectangle.Top) - cursorPosition;
                }
            }
            else if (rectangle.Contains(cursor))
            {
                RenderServices.DrawRectangle(render.DebugFxBatch, rectangle, color * .25f);
                hover = true;
                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _draggingStyle = DragStyle.Move;
                    _dragOffset = rectangle.TopLeft - cursorPosition;
                }
            }

            newRectangle = rectangle;
            return false;
        }

        public static bool PolyHandle(string id, RenderContext render, Vector2 basePosition, Vector2 cursorPosition, Polygon polygon, Color outline, Color color, out Polygon newPolygon, out bool isShapeHovered)
        {
            isShapeHovered = false;
            newPolygon = polygon;
            cursorPosition -= basePosition;
            var polygonWorld = polygon.AddPosition(basePosition.Point());
            if (!polygon.IsConvex())
            {
                if (Calculator.Blink(10, false))
                    RenderServices.DrawPolygon(render.DebugFxBatch, polygonWorld.Vertices, new DrawInfo(color * 0.45f, 0.8f));
            }
            else
            {
                RenderServices.DrawPolygon(render.DebugFxBatch, polygonWorld.Vertices, new DrawInfo(color * (0.45f + 0.2f * MathF.Sin(Game.NowUnscaled)), 0.8f));
            }

            if (_draggingHandle == id)
            {
                var target = (cursorPosition + _dragOffset).Point();
                if (!Game.Input.Down(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = String.Empty;
                }
                else
                {
                    RenderServices.DrawPoints(render.DebugBatch, Vector2.Zero, polygonWorld.Vertices.AsSpan(), outline, 1);
                    RenderServices.DrawPolygon(render.DebugFxBatch, polygonWorld.Vertices, new DrawInfo(color * 0.45f, 0.8f));

                    if (_draggingAnchor < 0)
                    {
                        newPolygon = newPolygon.AtPosition(target);

                        return true;
                    }
                    else
                    {
                        newPolygon = newPolygon.WithVerticeAt(_draggingAnchor, target);

                        return true;
                    }
                }
            }

            Point cursor = cursorPosition.Point();
            float hovered = float.MaxValue;
            Vector2? selectedPoint = null;

            for (int i = 0; i < polygon.Vertices.Length; i++)
            {
                var origin = polygon.Vertices[i];
                var line = new Line2(origin, polygon.Vertices[Calculator.WrapAround(i + 1, 0, polygon.Vertices.Length - 1)]);
                var cursorAnchor = new Rectangle(origin.X - 4, origin.Y - 4, 8, 8);
                var anchor = new Rectangle(origin.X - 2, origin.Y - 2, 4, 4);

                if (cursorAnchor.Contains(cursorPosition))
                {
                    hovered = -1;
                    selectedPoint = null;

                    // Delete Anchor
                    if (Architect.Input.Down(MurderInputButtons.Shift))
                    {
                        RenderServices.DrawRectangle(render.DebugBatch, anchor.AddPosition(basePosition), Calculator.Blink(10, false) ? color : Color.White, 1f);

                        if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                        {
                            newPolygon = polygon.RemoveVerticeAt(Calculator.WrapAround(_draggingAnchor + 1, 0, polygon.Vertices.Length - 1));
                            return true;
                        }
                    }
                    // Move Anchor
                    else
                    {
                        RenderServices.DrawRectangle(render.DebugBatch, anchor.AddPosition(basePosition), Color.White, 1f);

                        if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                        {
                            _draggingHandle = id;
                            _draggingAnchor = i;
                            _dragOffset = polygon.Vertices[i] - cursorPosition;
                        }
                    }
                }
                else
                {
                    // Select new anchor position
                    if (Architect.Input.Down(MurderInputButtons.Shift) && line.GetClosestPoint(cursor, 8, out var closest))
                    {
                        float closestDistance = (cursor - closest).Length();
                        if (closestDistance < hovered)
                        {
                            hovered = closestDistance;
                            selectedPoint = closest;
                            _draggingAnchor = i;
                        }
                    }
                }
                RenderServices.DrawRectangle(render.DebugBatch, new Rectangle(origin.X - 1 + basePosition.X, origin.Y - 1 + basePosition.Y, 2, 2), Color.Lerp(color, Color.White, 0.5f), 1f);
                RenderServices.DrawLine(render.DebugBatch, line.Start + basePosition, line.End + basePosition, Color.Lerp(outline, color, 0.35f), 0.8f);
            }

            if (selectedPoint != null)
            {
                // Create new anchor
                RenderServices.DrawRectangle(render.DebugBatch, new Rectangle(selectedPoint.Value.X - 1.5f + basePosition.X, selectedPoint.Value.Y - 1.5f + basePosition.Y, 3, 3), Color.White, 1f);
                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    newPolygon = polygon.WithNewVerticeAt(_draggingAnchor + 1, cursor);
                    return true;
                }
            }

            if (string.IsNullOrEmpty(_draggingHandle) && polygon.Contains(cursor))
            {
                RenderServices.DrawPolygon(render.DebugFxBatch, polygon.Vertices, new DrawInfo(color * 0.25f, 0.8f));

                if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                {
                    _draggingHandle = id;
                    _draggingAnchor = -1;
                    _draggingStyle = DragStyle.Move;
                    _dragOffset = polygon.Vertices[0] - cursorPosition;
                }

                isShapeHovered = true;
            }

            return false;
        }

        /// <summary>
        /// Drags a rectangle area around.
        /// </summary>
        public static bool DragArea(string id, Vector2 cursorPosition, Rectangle area, out Vector2 newPosition)
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

            for (int i = 0; i < polygon.Vertices.Length - 1; i++)
            {
                Vector2 pointA = polygon.Vertices[i];
                Vector2 pointB = polygon.Vertices[i + 1];
                RenderServices.DrawLine(render.DebugBatch, pointA + position, pointB + position, color);
                if (DrawHandle($"{id}_point_{i}", render, cursorPosition, position + pointA, color, out Vector2 newPosition))
                {
                    modified = true;
                    var newVertices = polygon.Vertices.ToArray();
                    newVertices[i] = newPosition.Point();
                    result = new Polygon(newVertices);
                }
            }

            {
                var lastVert = polygon.Vertices[polygon.Vertices.Length - 1];
                RenderServices.DrawLine(render.DebugBatch, lastVert + position, polygon.Vertices[0] + position, color);

                if (DrawHandle($"{id}_point_{polygon.Vertices.Length}", render, cursorPosition, position + lastVert, color, out Vector2 newPosition))
                {
                    modified = true;
                    var newVertices = polygon.Vertices.ToArray();
                    newVertices[polygon.Vertices.Length - 1] = newPosition.Point();
                    result = new Polygon(newVertices);
                }
            }

            return modified;
        }

        public static bool SaveAssetWhenSelectedAssetIsSaved(Guid guidToTrackOnSelectedAssetSaved)
        {
            if (Architect.Instance.ActiveScene is EditorScene editorScene)
            {
                editorScene.CurrentAsset?.TrackAssetOnSave(guidToTrackOnSelectedAssetSaved);

                return true;
            }

            return false;
        }
    }
}