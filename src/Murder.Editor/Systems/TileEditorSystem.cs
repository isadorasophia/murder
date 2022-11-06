using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Services;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;
using static Murder.Editor.Utilities.EditorHook;

namespace Murder.Editor.Systems
{
    [DoNotPause]
    [OnlyShowOnDebugView]
    [Filter(typeof(TileGridComponent))]
    public class TileEditorSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            if (context.World.TryGetUnique<EditorComponent>() is not EditorComponent editor)
            {
                return default;
            }

            foreach (Entity e in context.Entities)
            {
                DrawResizeBox(render, context.World, editor, e);
                DrawTileSelector(render, editor, e);
            }

            return default;
        }

        private bool DrawResizeBox(RenderContext render, World world, EditorComponent editor, Entity e)
        {
            TileGridComponent gridComponent = e.GetTileGrid();

            TileGrid grid = gridComponent.Grid;
            Point position = grid.Origin;

            Color color = Game.Profile.Theme.Accent.ToXnaColor();

            IntRectangle rectangle = new Rectangle(position.X, position.Y, grid.Width, grid.Height);
            RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, rectangle * Grid.CellSize, color);
            RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, (rectangle * Grid.CellSize).Expand(1), Color.Black.WithAlpha(.2f));

            if (DrawHandles(render, world, editor, e.EntityId, rectangle, color) is IntRectangle newRectangle)
            {
                grid.Resize(newRectangle);
                e.SetTileGrid(grid);

                return true;
            }

            return false;
        }

        private Rectangle? _resize;

        /// <summary>
        /// Draw a box and listen to user input.
        /// </summary>
        /// <param name="render">Render context.</param>
        /// <param name="editor">Editor component.</param>
        /// <param name="id">Unique identifier for this box.</param>
        /// <param name="gridRectangle">Rectangle position in the grid.</param>
        /// <param name="color">Color which will be displayed.</param>
        private IntRectangle? DrawHandles(RenderContext render, World world, EditorComponent editor, int id, IntRectangle gridRectangle, Color color)
        {
            if (_resize is not null && !Game.Input.Down(MurderInputButtons.LeftClick))
            {
                ChangeCursorTo(world, CursorStyle.Normal);

                // If there was a preview, it seems that it's time to build it!
                Rectangle result = _resize.Value;

                _resize = null;
                return result;
            }

            Vector2 worldHalf = gridRectangle.Size * Grid.CellSize / 2f;
            Vector2 worldPosition = gridRectangle.TopLeft * Grid.CellSize;
            Vector2 worldBottomRight = gridRectangle.BottomRight * Grid.CellSize;

            // Start by drawing the middle circle.
            if (EditorServices.DrawHandle($"offset_{id}", render,
                    editor.EditorHook.CursorWorldPosition, position: worldPosition + worldHalf, color: color, out Vector2 newWorldPosition))
            {
                Point newGridTopLeftPosition = (newWorldPosition - worldHalf).ToGridPoint();

                // Clamp at zero.
                if (newGridTopLeftPosition.X >= 0 && newGridTopLeftPosition.Y >= 0)
                {
                    _resize = new(position: newGridTopLeftPosition, gridRectangle.Size);
                }
            }
            
            // Now, draw the bottom right handle.
            if (EditorServices.DrawHandle($"offset_{id}_BR", render,
                editor.EditorHook.CursorWorldPosition, position: worldBottomRight, color, out Vector2 newWorldBottomRight))
            {
                Point gridDelta = newWorldBottomRight.ToGridPoint() - gridRectangle.BottomRight;

                _resize = new(gridRectangle.TopLeft, gridRectangle.Size + gridDelta);
            }

            // We are fancy and also draw the top left handle.
            if (EditorServices.DrawHandle($"offset_{id}_TL", render,
                editor.EditorHook.CursorWorldPosition, position: worldPosition, color, out Vector2 newWorldTopLeft))
            {
                Point gridDelta = newWorldTopLeft.ToGridPoint() - gridRectangle.TopLeft;
 
                Point newGridTopLeftPosition = newWorldTopLeft.ToGridPoint();

                // Clamp at zero.
                if (newGridTopLeftPosition.X >= 0 && newGridTopLeftPosition.Y >= 0)
                {
                    _resize = new(newGridTopLeftPosition, gridRectangle.Size - gridDelta);
                }
            }

            // Let's add the preview to the user.
            if (_resize is not null)
            {
                RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, _resize.Value * Grid.CellSize, Color.Green);

                ChangeCursorTo(world, CursorStyle.Hand);
            }

            return default;
        }

        private static void ChangeCursorTo(World world, CursorStyle style)
        {
            if (world.TryGetUnique<EditorComponent>() is EditorComponent editorComponent)
            {
                editorComponent.EditorHook.Cursor = style;
            }
        }

        private Vector2? _startedShiftDragging;
        private Color? _dragColor;

        /// <summary>
        /// This is the logic for capturing input for new tiles.
        /// </summary>
        private bool DrawTileSelector(RenderContext render, EditorComponent editor, Entity e)
        {
            if (_resize is not null)
            {
                // We are currently resizing, we have no business building tiles for now.
                return false;
            }

            bool modified = false;

            TileGridComponent gridComponent = e.GetTileGrid();
            TileGrid grid = gridComponent.Grid;

            Point cursorWorldPosition = editor.EditorHook.CursorWorldPosition;
            if (cursorWorldPosition.X < 0 || cursorWorldPosition.Y < 0)
            {
                return false;
            }

            Point cursorGridPosition = cursorWorldPosition.FromWorldToLowerBoundGridPosition();
            if (!gridComponent.Rectangle.Contains(cursorGridPosition))
            {
                return false;
            }

            // We are actually applying operation over an area.
            if (_startedShiftDragging == null &&
                Game.Input.Down(MurderInputButtons.Shift) && 
                (Game.Input.Down(MurderInputButtons.LeftClick) || Game.Input.Down(MurderInputButtons.RightClick)))
            {
                // Start tracking the origin.
                _startedShiftDragging = cursorGridPosition;
                _dragColor = Game.Input.Down(MurderInputButtons.LeftClick) ? Color.Green.WithAlpha(.1f) : Color.Red.WithAlpha(.05f);
            }

            if (_startedShiftDragging != null && _dragColor != null)
            {
                // Draw the rectangle applied over the area.
                IntRectangle draggedRectangle = GridHelper.FromTopLeftToBottomRight(_startedShiftDragging.Value, cursorGridPosition);

                RenderServices.DrawRectangle(render.DebugSpriteBatch, draggedRectangle * Grid.CellSize, _dragColor.Value);
                RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, (draggedRectangle * Grid.CellSize).Expand(1), Color.White.WithAlpha(.5f));

                if (Game.Input.Released(MurderInputButtons.LeftClick))
                {
                    _startedShiftDragging = null;
                    grid.SetGridPosition(draggedRectangle, TilesetGridType.Solid);

                    return true;
                }
                else if (Game.Input.Released(MurderInputButtons.RightClick))
                {
                    _startedShiftDragging = null;
                    grid.SetGridPosition(draggedRectangle, TilesetGridType.Empty);

                    return true;
                }

                return false;
            }

            // We are applying an operation over an individual tile.
            Color color = Game.Profile.Theme.White.ToXnaColor();
            color = color.WithAlpha(.5f);

            // Otherwise, we are at classical individual tile selection.
            IntRectangle rectangle = new Rectangle(cursorGridPosition.X, cursorGridPosition.Y, 1, 1);
            RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, rectangle * Grid.CellSize, color);

            if (Game.Input.Down(MurderInputButtons.LeftClick))
            {
                if (grid.AtGridPosition(cursorGridPosition) != TilesetGridType.Solid)
                {
                    modified = true;

                    grid.SetGridPosition(cursorGridPosition, TilesetGridType.Solid);
                }

                Game.Input.Consume(MurderInputButtons.LeftClick);
            }
            else if (Game.Input.Down(MurderInputButtons.RightClick))
            {
                if (grid.AtGridPosition(cursorGridPosition) == TilesetGridType.Solid)
                {
                    modified = true;

                    grid.SetGridPosition(cursorGridPosition, TilesetGridType.Empty);
                }

                Game.Input.Consume(MurderInputButtons.RightClick);
            }

            return modified;
        }
    }
}
