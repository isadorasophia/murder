using Bang.Contexts;
using Bang.Systems;
using Murder.Attributes;
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
    [EditorSystem]
    [Filter(filter: ContextAccessorFilter.AnyOf, kind: ContextAccessorKind.Read, typeof(MapComponent))]
    public class EditorFloorRenderSystem : IMurderRenderSystem
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
                    float colorLerp = 0.5f - MathF.Sin(Architect.NowUnscaled) * 0.5f;

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

                    render.FloorBatch.DrawRectangle(new Rectangle(x, y, 1, 1) * cellSize, color, 1);
                }
            }

            // Draw center of the editor.
            render.FloorBatch.DrawRectangle(new Rectangle(bounds.X, 0, bounds.Width, 2 / render.Camera.Zoom), Color.Black * 0.3f, 0);
            render.FloorBatch.DrawRectangle(new Rectangle(0, bounds.Y, 2 / render.Camera.Zoom, bounds.Height), Color.Black * 0.3f, 0);
        }
    }
}