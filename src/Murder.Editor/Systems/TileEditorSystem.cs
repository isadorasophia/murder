using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Services;
using Murder.Services;
using Murder.Utilities;

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
                DrawResizeBox(render, editor, e);
                DrawTileSelector(render, editor, e);
            }

            return default;
        }

        private void DrawTileSelector(RenderContext render, EditorComponent editor, Entity e)
        {
            TileGridComponent gridComponent = e.GetTileGrid();

            Point cursorWorldPosition = editor.EditorHook.CursorWorldPosition.FromWorldToLowerBoundGridPosition();

            Color color = Game.Profile.Theme.White.ToXnaColor();
            color = color.WithAlpha(.5f);

            IntRectangle rectangle = new Rectangle(cursorWorldPosition.X, cursorWorldPosition.Y, 1, 1);
            RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, rectangle * Grid.CellSize, color);

            TileGrid grid = gridComponent.Grid;
        }

        private bool DrawResizeBox(RenderContext render, EditorComponent editor, Entity e)
        {
            TileGridComponent gridComponent = e.GetTileGrid();

            TileGrid grid = gridComponent.Grid;
            Point position = grid.Origin;

            Color color = Game.Profile.Theme.Faded.ToXnaColor();

            IntRectangle rectangle = new Rectangle(position.X, position.Y, grid.Width, grid.Height);
            RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, rectangle * Grid.CellSize, color);

            if (DrawBoxAndListenInput(render, editor, e.EntityId, rectangle, color) is IntRectangle newRectangle)
            {
                grid.Resize(newRectangle);
                e.SetTileGrid(grid);

                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Draw a box and listen to user input.
        /// </summary>
        /// <param name="render">Render context.</param>
        /// <param name="editor">Editor component.</param>
        /// <param name="id">Unique identifier for this box.</param>
        /// <param name="gridRectangle">Rectangle position in the grid.</param>
        /// <param name="color">Color which will be displayed.</param>
        private IntRectangle? DrawBoxAndListenInput(RenderContext render, EditorComponent editor, int id, IntRectangle gridRectangle, Color color)
        {
            Vector2 worldHalf = gridRectangle.Size * Grid.CellSize / 2f;
            Vector2 worldPosition = gridRectangle.TopLeft * Grid.CellSize;
            Vector2 worldSize = gridRectangle.Size * Grid.CellSize;

            if (EditorServices.DrawHandle($"offset_{id}", render,
                    editor.EditorHook.CursorWorldPosition, position: worldPosition + worldHalf, color: color, out Vector2 newWorldPosition))
            {
                Point newGridTopLeftPosition = (newWorldPosition - worldHalf).ToGridPoint();
                if (newGridTopLeftPosition != gridRectangle.TopLeft)
                {
                    return new(position: newGridTopLeftPosition, gridRectangle.Size);
                }
            }

            if (EditorServices.DrawHandle($"offset_{id}_BR", render,
                editor.EditorHook.CursorWorldPosition, position: worldSize, color, out Vector2 newWorldBottomRight))
            {
                Point gridDelta = newWorldBottomRight.ToGridPoint() - gridRectangle.BottomRight;

                return new(gridRectangle.TopLeft, gridRectangle.Size + gridDelta);
            }

            return default;
        }
    }
}
