using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Components;
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

        private Point? _startedSelectDragging;
        private Rectangle? _selectedArea; // TODO: Why is this a rectangle.
        private Point? _selectedAreaDragOffset;

        // Draw cursor movements while selecting an area to move to.
        private bool DrawTileSelector(RenderContext render, EditorComponent editor, Entity e)
        {
            if (!GatherMapInfo(render, editor, e,
                out TileGridComponent gridComponent,
                out TileGrid? grid,
                out Point cursorGridPosition,
                out IntRectangle bounds))
            {
                return false;
            }

            Color color = Game.Profile.Theme.White.ToXnaColor();
            color = color * .5f;

            // Otherwise, we are at classical individual tile selection.

            bool hovered = _selectedArea?.Contains(cursorGridPosition) ?? false;

            if (_selectedArea != null)
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
            else if (_startedSelectDragging != null)
            {
                RenderServices.DrawRectangleOutline(render.DebugFxBatch,
                    (GridHelper.FromTopLeftToBottomRight(_startedSelectDragging.Value, cursorGridPosition) * Grid.CellSize)
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
                    if (_selectedArea != null)
                    {
                        _selectedAreaDragOffset = cursorGridPosition - _selectedArea.Value.TopLeft.Point();
                    }
                }
                else if (_startedSelectDragging == null)
                {
                    _targetEntity = e.EntityId;

                    // Start tracking the origin.
                    _selectedArea = null;
                    _startedSelectDragging = cursorGridPosition;
                    _currentRectDraw = (GridHelper.FromTopLeftToBottomRight(_startedSelectDragging.Value, cursorGridPosition) * Grid.CellSize);
                    _dragColor = Game.Input.Down(MurderInputButtons.LeftClick) ? Color.Green * .1f : Color.Red * .05f;
                    _tweenStart = Game.Now;
                }
            }

            if (!Game.Input.Down(MurderInputButtons.LeftClick))
            {
                // Movement is over!
                if (_startedSelectDragging is not null && _selectedArea is not null)
                {
                    grid.MoveFromTo(_startedSelectDragging.Value, cursorGridPosition, _selectedArea.Value.Size.ToPoint());
                }

                if (_startedSelectDragging != null)
                {
                    _selectedArea = GridHelper.FromTopLeftToBottomRight(_startedSelectDragging.Value, cursorGridPosition);

                    if (_selectedArea.Value.Size.X <= 1 || _selectedArea.Value.Size.Y <= 1)
                    {
                        _selectedArea = null;
                    }
                }

                _startedSelectDragging = null;
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
                _startedSelectDragging = null;
                _selectedAreaDragOffset = null;
            }

            return true;
        }

        // Draw the gesture for switching tile mode.
        private bool DrawToolbox(RenderContext render, EditorComponent editor)
        {
            var atlas = Game.Data.FetchAtlas(Murder.Data.AtlasId.Editor);
            var icon = atlas.Get(_editorMode == EditorMode.Cut ? "cursor_cut" : "cursor_pencil");

            Rectangle buttonRect = Rectangle.CenterRectangle(new Vector2(render.Camera.HalfWidth, 30), 39, 39);
            bool hovered = buttonRect.Contains(editor.EditorHook.CursorScreenPosition);
            bool down = false;

            if (hovered)
            {
                down = Game.Input.Down(MurderInputButtons.LeftClick);

                if (down)
                {
                    if (!_pressedToolboxCut)
                    {
                        down = true;
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

            if ((down || _pressedToolboxCut) && !Game.Input.Down(MurderInputButtons.LeftClick))
            {
                down = false;
                _pressedToolboxCut = false;
            }

            render.UiBatch.DrawRectangle(buttonRect, down ? Color.White : hovered ? Color.Gray * 0.8f : Color.Gray * 0.5f, 0.2f);
            render.UiBatch.DrawRectangleOutline(buttonRect, Color.Black, 1, 0.12f);
            icon.Draw(render.UiBatch, buttonRect.Center,
                new(Vector2.Zero, icon.Size), Color.White, Vector2.One * 2, 0, icon.Size / 2f - new Vector2(0, 0), ImageFlip.None, RenderServices.BLEND_NORMAL, 0);

            return hovered || _pressedToolboxCut;
        }
    }
}