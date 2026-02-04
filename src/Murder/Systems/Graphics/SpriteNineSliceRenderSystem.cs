using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(NineSliceComponent), typeof(ITransformComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(InvisibleComponent))]
    public class SpriteNineSliceRenderSystem : IMurderRenderSystem
    {
        /// <summary>
        /// This draws an sprite nine slice component.
        /// </summary>
        public void Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                NineSliceComponent nineSlice = e.GetNineSlice();

                Vector2 position = e.GetGlobalPosition();

                // This is as early as we can to check for out of bounds
                if (nineSlice.TargetSpriteBatch != Batches2D.UiBatchId &&
                    !render.Camera.Bounds.Touches(new Rectangle(position - nineSlice.Target.TopLeft, nineSlice.Target.Size)))
                {
                    continue;
                }

                Batch2D targetBatch = render.GetBatch((int)nineSlice.TargetSpriteBatch);

                float ySort = RenderServices.YSort(position.Y + nineSlice.YSortOffset);
                RenderServices.Draw9Slice(targetBatch, nineSlice.Sprite, nineSlice.Target.AddPosition(position), nineSlice.Style, new DrawInfo() { Sort = ySort }, AnimationInfo.Default);
            }
        }
    }
}