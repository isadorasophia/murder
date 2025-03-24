using Bang.Contexts;
using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Data;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.Systems
{
    public partial class TileEditorSystem
    {
        public enum EditorMode
        {
            Draw,
            Cut
        }

        private EditorMode _editorMode = EditorMode.Draw;

        private bool _pressedToolboxCut = false;

        private Point? _startSelectionPoint;
        private IntRectangle? _selectedArea;
        private Point? _selectedAreaDragOffset;

        private Point? _selectedAreaOrigin;

        // Draw cursor movements while selecting an area to move to.
        private bool DrawTileSelector(RenderContext render, Context context, EditorComponent editor)
        {
            if (!CanDrawCursorGestures(render, entityId: -1))
            {
                return false;
            }

            if (editor.EditorHook.CursorWorldPosition is not Point cursorWorldPosition)
            {
                return false;
            }

            if (Game.Input.Pressed(MurderInputButtons.Delete) && _selectedArea is not null)
            {
                Delete(context, _selectedArea.Value);

                _previousArea = null;
            }

            if (Game.Input.Pressed(MurderInputButtons.Esc))
            {
                _selectedArea = null;
                _startSelectionPoint = null;
                _selectedAreaDragOffset = null;

                _previousArea = null;
            }

            Color color = Game.Profile.Theme.White.ToXnaColor();
            color *= .5f;

            // Otherwise, we are at classical individual tile selection.

            Point cursorGridPosition = cursorWorldPosition.FromWorldToLowerBoundGridPosition();
            bool hovered = _selectedArea?.Contains(cursorGridPosition) ?? false;

            if (_selectedArea is not null)
            {
                if (_selectedAreaDragOffset != null)
                {
                    RenderServices.DrawRectangle(render.DebugFxBatch,
                        (_selectedArea.Value * Grid.CellSize)
                        .Expand(4 - 3 * Ease.ZeroToOne(Ease.BackInOut, 0.250f, _tweenStart)), color * .05f);
                }
                else
                {
                    RenderServices.DrawRectangleOutline(render.DebugFxBatch,
                        (_selectedArea.Value * Grid.CellSize)
                        .Expand(4 - 3 * Ease.ZeroToOne(Ease.BackInOut, 0.250f, _tweenStart)), color * (hovered ? 1 : .05f));
                }
            }
            else if (_startSelectionPoint != null)
            {
                RenderServices.DrawRectangleOutline(render.DebugFxBatch,
                    (GridHelper.FromTopLeftToBottomRight(_startSelectionPoint.Value, cursorGridPosition) * Grid.CellSize)
                    .Expand(4 - 3 * Ease.ZeroToOne(Ease.BackInOut, 0.250f, _tweenStart)), color * .15f);
            }
            else
            {
                IntRectangle rectangle = new Rectangle(cursorGridPosition.X, cursorGridPosition.Y, 1, 1);
                RenderServices.DrawRectangleOutline(render.DebugFxBatch, (rectangle * Grid.CellSize).Expand(4 - 3 * Ease.ZeroToOne(Ease.BackInOut, 0.250f, _tweenStart)), color * .15f);
            }

            if (Game.Input.Pressed(MurderInputButtons.LeftClick))
            {
                if (hovered)
                {
                    // Start dragging a rectangle
                    if (_selectedArea != null)
                    {
                        _selectedAreaDragOffset = cursorGridPosition - _selectedArea.Value.TopLeft;
                    }
                }
                else if (_startSelectionPoint == null)
                {
                    // _targetEntity = e.EntityId;

                    // Start tracking the origin.
                    _selectedArea = null;

                    _startSelectionPoint = cursorGridPosition;
                    _currentRectDraw = (GridHelper.FromTopLeftToBottomRight(_startSelectionPoint.Value, cursorGridPosition) * Grid.CellSize);
                    _dragColor = Game.Input.Down(MurderInputButtons.LeftClick) ? Color.Green * .1f : Color.Red * .05f;
                    _tweenStart = Game.Now;

                    _previousArea = null;
                }
            }

            if (!Game.Input.Down(MurderInputButtons.LeftClick))
            {
                // Movement is over!
                if (_selectedArea is not null && _selectedAreaOrigin is not null && _selectedAreaDragOffset is not null)
                {
                    MoveFrom(context, _selectedAreaOrigin.Value, _selectedArea.Value.TopLeft, _selectedArea.Value.Size);
                    _selectedAreaOrigin = _selectedArea.Value.TopLeft;
                }

                if (_startSelectionPoint is not null)
                {
                    _selectedArea = GridHelper.FromTopLeftToBottomRight(_startSelectionPoint.Value, cursorGridPosition);
                    _selectedAreaOrigin = _startSelectionPoint.Value;

                    if (_selectedArea.Value.Size.X <= 1 || _selectedArea.Value.Size.Y <= 1)
                    {
                        _selectedArea = null;
                        _previousArea = null;
                    }
                }

                _startSelectionPoint = null;
                _selectedAreaDragOffset = null;
            }
            else
            {
                // Dragging
                if (_selectedAreaDragOffset != null && _selectedArea != null)
                {
                    _selectedArea = new Rectangle(cursorGridPosition - _selectedAreaDragOffset.Value, _selectedArea.Value.Size);
                }
            }

            if (Game.Input.Pressed(MurderInputButtons.RightClick))
            {
                _selectedArea = null;
                _startSelectionPoint = null;
                _selectedAreaDragOffset = null;

                _previousArea = null;
            }

            return true;
        }

        private void UpdateToolbox(Camera2D camera, EditorHook hook)
        {
            Rectangle buttonRect = Rectangle.CenterRectangle(new Vector2(camera.HalfWidth, 30), 39, 39);

            Rectangle collisionButtonRect = buttonRect * Architect.EditorSettings.DpiScale;
            _hoveredOnToolboxArea = collisionButtonRect.Contains(hook.CursorScreenPosition);

            _downOnToolboxArea = false;

            if (_hoveredOnToolboxArea)
            {
                _downOnToolboxArea = Game.Input.Down(MurderInputButtons.LeftClick);

                if (_downOnToolboxArea)
                {
                    if (!_pressedToolboxCut)
                    {
                        _downOnToolboxArea = true;
                        _pressedToolboxCut = true;
                    }
                }
                else
                {
                    if (_pressedToolboxCut)
                    {
                        // Clicked!
                        if (_editorMode == EditorMode.Cut)
                        {
                            _editorMode = EditorMode.Draw;
                        }
                        else
                        {
                            _editorMode = EditorMode.Cut;
                        }

                        _pressedToolboxCut = false;
                    }
                }
            }

            if ((_downOnToolboxArea || _pressedToolboxCut) && !Game.Input.Down(MurderInputButtons.LeftClick))
            {
                _downOnToolboxArea = false;
                _pressedToolboxCut = false;
            }
        }

        private bool _downOnToolboxArea = false;
        private bool _hoveredOnToolboxArea = false;

        // Draw the gesture for switching tile mode.
        private bool DrawToolbox(RenderContext render)
        {
            var atlas = Game.Data.FetchAtlas(AtlasIdentifiers.Editor);
            var icon = atlas.Get(_editorMode == EditorMode.Cut ? "cursor_cut" : "cursor_pencil");

            Rectangle buttonRect = Rectangle.CenterRectangle(new Vector2(render.Camera.HalfWidth, 30), 39, 39);

            render.UiBatch.DrawRectangle(buttonRect, _downOnToolboxArea ? Color.White : _hoveredOnToolboxArea ? Color.Gray * 0.8f : Color.Gray * 0.5f, 0.2f);
            render.UiBatch.DrawRectangleOutline(buttonRect, Color.Black, 1, 0.12f);
            icon.Draw(render.UiBatch, buttonRect.Center,
                new(Vector2.Zero, icon.Size), Color.White, Vector2.One * 2, 0, icon.Size / 2f - new Vector2(0, 0), ImageFlip.None, RenderServices.BLEND_NORMAL, MurderBlendState.AlphaBlend, 0);

            return _hoveredOnToolboxArea || _pressedToolboxCut;
        }

        private void Delete(Context context, IntRectangle area)
        {
            for (int cy = 0; cy < area.Height; cy++)
            {
                for (int cx = 0; cx < area.Width; cx++)
                {
                    Point pointInMap = area.TopLeft + new Point(cx, cy);

                    TileGrid? grid = FindCollidingGrid(context, pointInMap);
                    if (grid is null)
                    {
                        continue;
                    }

                    grid.Set(pointInMap - grid.Origin, 0, overridePreviousValues: true);
                }
            }
        }

        private int[]? _previousArea = null;

        private void MoveFrom(Context context, Point from, Point to, Point size)
        {
            Span<int> colliders = stackalloc int[size.X * size.Y];

            for (int cy = 0; cy < size.Y; cy++)
            {
                for (int cx = 0; cx < size.X; cx++)
                {
                    Point pointInMap = from + new Point(cx, cy);

                    TileGrid? grid = FindCollidingGrid(context, pointInMap);
                    if (grid is null)
                    {
                        continue;
                    }

                    colliders[(cy * size.X) + cx] = grid.At(pointInMap - grid.Origin);

                    int value = 0;
                    if (_previousArea is not null)
                    {
                        value = _previousArea[(cy * size.X) + cx];
                    }

                    grid.Set(pointInMap - grid.Origin, value, overridePreviousValues: true);
                }
            }

            _previousArea ??= new int[size.X * size.Y];

            for (int cy = 0; cy < size.Y; cy++)
            {
                for (int cx = 0; cx < size.X; cx++)
                {
                    Point pointInMap = to + new Point(cx, cy);

                    TileGrid? grid = FindCollidingGrid(context, pointInMap);
                    if (grid is null)
                    {
                        continue;
                    }

                    _previousArea[(cy * size.X) + cx] = grid.At(pointInMap - grid.Origin);
                    grid.Set(pointInMap - grid.Origin, colliders[(cy * size.X) + cx], overridePreviousValues: true);
                }
            }
        }

        private TileGrid? FindCollidingGrid(Context context, Point cursorGridPosition)
        {
            foreach (Entity e in context.Entities)
            {
                TileGridComponent gridComponent = e.GetTileGrid();
                TileGrid grid = gridComponent.Grid;

                IntRectangle bounds = gridComponent.Rectangle;
                if (bounds.Contains(cursorGridPosition))
                {
                    return grid;
                }
            }

            return null;
        }
    }
}