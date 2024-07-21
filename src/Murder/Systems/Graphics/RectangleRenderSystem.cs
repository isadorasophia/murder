using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
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
                    box = collider.GetBoundingBox(position);
                }
                else
                {
                    box = new Rectangle(position, Vector2.One * Grid.CellSize);
                }

                if (rect.Fill)
                {
                    RenderServices.DrawRectangle(batch, box, rect.Color * alpha, RenderServices.YSort(box.Bottom + rect.SortingOffset));
                }
                else
                {
                    RenderServices.DrawRectangleOutline(batch, box, rect.Color * alpha, rect.LineWidth, RenderServices.YSort(box.Bottom + rect.SortingOffset));
                }
            }
        }
    }
}