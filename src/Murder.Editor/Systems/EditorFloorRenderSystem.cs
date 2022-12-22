using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Editor.Systems
{
    /// <summary>
    /// This is the system used to render the background in the editor system.
    /// </summary>
    [Filter(filter: ContextAccessorFilter.AnyOf, kind: ContextAccessorKind.Read, typeof(MapComponent))]
    public class EditorFloorRenderSystem : IMonoRenderSystem
    {
        /// <summary>
        /// Zoom threshold which starts to operate the editor differentially.
        /// </summary>
        public const int ZoomThreshold = 1;

        public void Draw(RenderContext render, Context context)
        {
            Rectangle bounds = render.Camera.Bounds;

            int cellSize = Grid.CellSize;
            if (render.Camera.Zoom < 1)
            {
                cellSize = Calculator.RoundToInt(cellSize / render.Camera.Zoom);
            }

            (int minX, int minY, int maxX, int maxY) = (
                 Calculator.RoundToInt(bounds.Left / cellSize),
                 Calculator.RoundToInt(bounds.Top / cellSize),
                 Calculator.RoundToInt(bounds.Right / cellSize),
                 Calculator.RoundToInt(bounds.Bottom / cellSize));

            for (int y = minY - 1; y <= maxY; y++)
            {
                for (int x = minX - 1; x <= maxX; x++)
                {
                    float colorLerp = 0.5f - MathF.Sin(Architect.Instance.ElapsedTime) * 0.5f;

                    Color color;
                    if (render.Camera.Zoom < 1)
                    {
                        color = (x + y) % 2 == 0 ?
                            Color.Lerp(Color.WarmGray, Color.WarmGray * 1.05f, Calculator.Clamp01(colorLerp)) :
                            Color.Lerp(Color.WarmGray * 0.95f, Color.WarmGray, Calculator.Clamp01(colorLerp));
                    }
                    else
                    {
                        color = (x + y) % 2 == 0 ?
                            Color.Lerp(Color.ColdGray, Color.ColdGray * 1.05f, Calculator.Clamp01(colorLerp)) :
                            Color.Lerp(Color.ColdGray * 0.95f, Color.ColdGray * 0.90f, Calculator.Clamp01(colorLerp));
                    }

                    render.FloorSpriteBatch.DrawRectangle(new Rectangle(x, y, 1, 1) * cellSize, color, 1);
                }
            }

            if (render.Camera.Zoom >= 1)
            {
                //render.FloorSpriteBatch.DrawRectangle(new Rectangle(bounds.X, -Grid.HalfCell, bounds.Width, 1),Color.Black.WithAlpha(0.3f), 0);
                //render.FloorSpriteBatch.DrawRectangle(new Rectangle(bounds.X, Grid.HalfCell, bounds.Width, 1),Color.Black.WithAlpha(0.3f), 0);

                //render.FloorSpriteBatch.DrawRectangle(new Rectangle(-Grid.HalfCell, bounds.Y,1, bounds.Height), Color.Black.WithAlpha(0.3f), 0);
                //render.FloorSpriteBatch.DrawRectangle(new Rectangle(Grid.HalfCell, bounds.Y, 1, bounds.Height), Color.Black.WithAlpha(0.3f), 0);

                //render.FloorSpriteBatch.DrawRectangle(new Rectangle(
                //    - Grid.HalfCell,
                //    - Grid.HalfCell, gridSize, gridSize), Color.BrightGray, 1);
            }
            else
            {
                // Draw center of the editor.
                render.FloorSpriteBatch.DrawRectangle(new Rectangle(bounds.X, 0, bounds.Width, 2 / render.Camera.Zoom), Color.Black * 0.3f, 0);
                render.FloorSpriteBatch.DrawRectangle(new Rectangle(0, bounds.Y, 2 / render.Camera.Zoom, bounds.Height), Color.Black * 0.3f, 0);
            }
        }
    }
}
