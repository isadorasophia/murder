using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Entities;
using Murder.Services;

namespace Murder.Systems.Graphics
{
    [Filter(typeof(RectPositionComponent), typeof(DrawRectangleComponent))]
    public class RectangleRenderSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                DrawRectangleComponent rect = e.GetDrawRectangle();

                Rectangle box = e.GetRectPosition().GetBox(e, render.ScreenSize, render.UiReferenceScale);
                if (rect.Fill)
                {
                    RenderServices.DrawRectangle(render.UiBatch, box, rect.Color, rect.Sorting);
                }
                else
                {
                    RenderServices.DrawRectangleOutline(render.UiBatch, box, rect.Color, rect.LineWidth, rect.Sorting);
                }
            }
            
            return default;
        }
    }
}
