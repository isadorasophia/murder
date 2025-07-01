using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems.Graphics
{
    [EditorSystem]
    [Filter(typeof(ITransformComponent), typeof(DrawRectangleComponent))]
    public class RectangleRenderSystem : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                DrawRectangleComponent rect = e.GetDrawRectangle();
                var batch = render.GetBatch(rect.TargetSpriteBatch);

                Vector2 position = e.GetGlobalTransform().Vector2;
                Rectangle box;

                float alpha = 1.0f;
                if (e.TryGetAlpha() is AlphaComponent alphaComponent)
                {
                    alpha = alphaComponent.Alpha;
                }

                if (e.TryGetCollider() is ColliderComponent collider)
                {
                    box = collider.GetBoundingBox(position, e.FetchScale());
                }
                else
                {
                    box = new Rectangle(position, Vector2.One * Grid.CellSize);
                }

                if (rect.Fill)
                {
                    RenderServices.DrawRectangle(batch, box, rect.Color * alpha, RenderServices.YSort(box.Bottom + rect.SortingOffset));

                    for (int i = 1; i <= rect.SmoothingLayers; i++)
                    {
                        float step = i / ((float)rect.SmoothingLayers + 1);
                        float size = (i / (float)rect.SmoothingLayers) * rect.SmoothSize;
                        RenderServices.DrawRectangle(batch, box.Expand(size), rect.Color * alpha * (1 - step), RenderServices.YSort(box.Bottom + rect.SortingOffset));
                    }
                }
                else
                {
                    RenderServices.DrawRectangleOutline(batch, box, rect.Color * alpha, rect.LineWidth, RenderServices.YSort(box.Bottom + rect.SortingOffset));
                }
            }
        }
    }
}