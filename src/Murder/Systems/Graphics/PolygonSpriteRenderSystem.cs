using Bang.Components;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using Bang.Entities;
using Murder.Core.Geometry;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.AllOf, typeof(PolygonSpriteComponent), typeof(ITransformComponent))]
public class PolygonSpriteRenderSystem : IMurderRenderSystem
{
    public void Draw(RenderContext render, Context context)
    {
        foreach (var e in context.Entities)
        {
            var polygonComponent = e.GetPolygonSprite();
            var batch = render.GetBatch((int)polygonComponent.Batch);
            var position = e.GetGlobalTransform().Point;
            var info = new DrawInfo(RenderServices.YSort(position.Y + polygonComponent.SortOffset))
            {
                Color = polygonComponent.Color
            };
            
            foreach (var shape in polygonComponent.Shapes)
            {
                switch (shape)
                {
                    case CircleShape circle:
                        RenderServices.DrawFilledCircle(batch, position + circle.Offset, circle.Radius,
                            Circle.EstipulateSidesFromRadius(circle.Radius), info);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}