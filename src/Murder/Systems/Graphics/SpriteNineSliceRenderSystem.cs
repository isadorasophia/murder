using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(NineSliceComponent), typeof(ITransformComponent))]
    public class SpriteNineSliceRenderSystem : IMonoRenderSystem
    {
        /// <summary>
        /// This draws an sprite nine slice component.
        /// </summary>
        public void Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                NineSliceComponent nineSlice = e.GetNineSlice();

                IMurderTransformComponent transform = e.GetGlobalTransform();
                Vector2 position = transform.Vector2;

                // This is as early as we can to check for out of bounds
                if (nineSlice.TargetSpriteBatch != TargetSpriteBatches.Ui && 
                    !render.Camera.Bounds.Touches(new Rectangle(position - nineSlice.Target.TopLeft, nineSlice.Target.Size)))
                {
                    continue;
                }

                Batch2D targetBatch = render.GetSpriteBatch(nineSlice.TargetSpriteBatch);

                float ySort = RenderServices.YSort(transform.Y + nineSlice.YSortOffset);
                RenderServices.Draw9Slice(targetBatch, nineSlice.Sprite, nineSlice.Target.AddPosition(position), new DrawInfo() { Sort = ySort }, AnimationInfo.Default);
            }
        }
    }
}
