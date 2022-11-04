using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Editor.Systems
{
    [Filter(filter: ContextAccessorFilter.AnyOf, kind: ContextAccessorKind.Read, typeof(TilesetComponent), typeof(MapComponent))]
    public class EditorFloorRenderSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            var bounds = render.Camera.Bounds;
            int gridSize = Grid.CellSize;
            if (render.Camera.Zoom < 1)
            {
                gridSize = Calculator.RoundToInt(gridSize / render.Camera.Zoom);
            }
            (int minX, int minY, int maxX, int maxY) = (
                 Calculator.RoundToInt(bounds.Left / gridSize),
                 Calculator.RoundToInt(bounds.Top / gridSize),
                 Calculator.RoundToInt(bounds.Right / gridSize),
                 Calculator.RoundToInt(bounds.Bottom / gridSize));
            for (int y = minY-1; y <= maxY; y++)
            {
                for (int x = minX-1; x <= maxX; x++)
                {
                    float colorLerp = 0.5f - MathF.Sin(Architect.Instance.ElapsedTime) * 0.5f;
                    if (render.Camera.Zoom < 1)
                    {
                        Color color = ((x + y) % 2 == 0 ?
                        Color.Lerp(Color.WarmGray, Color.WarmGray * 1.05f, Calculator.Clamp01(colorLerp)) :
                        Color.Lerp(Color.WarmGray * 0.95f, Color.WarmGray, Calculator.Clamp01(colorLerp)));
                        render.FloorSpriteBatch.DrawRectangle(new Rectangle(
                            x * gridSize,
                            y * gridSize, gridSize, gridSize), color, 1);
                    }
                    else
                    {
                        Color color = ((x + y) % 2 == 0 ?
                        Color.Lerp(Color.ColdGray, Color.ColdGray * 1.05f, Calculator.Clamp01(colorLerp)) :
                        Color.Lerp(Color.ColdGray * 0.95f, Color.ColdGray * 0.90f, Calculator.Clamp01(colorLerp)));
                        render.FloorSpriteBatch.DrawRectangle(new Rectangle(
                            x * gridSize - Grid.HalfCell,
                            y * gridSize - Grid.HalfCell, gridSize, gridSize), color, 1);
                    }
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
                render.FloorSpriteBatch.DrawRectangle(new Rectangle(bounds.X, 0, bounds.Width, 2 / render.Camera.Zoom), Color.Black.WithAlpha(0.3f), 0);
                render.FloorSpriteBatch.DrawRectangle(new Rectangle(0, bounds.Y, 2 / render.Camera.Zoom, bounds.Height), Color.Black.WithAlpha(0.3f), 0);
            }

            return default;
        }
    }
}
