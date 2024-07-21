using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems.Graphics
{
    [EditorSystem]
    [Filter(ContextAccessorFilter.AllOf, typeof(ThreeSliceComponent), typeof(ITransformComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(InvisibleComponent))]
    public class SpriteThreeSliceRenderSystem : IMurderRenderSystem
    {
        /// <summary>
        /// This draws an aseprite three slice component.
        /// TODO: Support animations?
        /// </summary>
        public void Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                SpriteComponent s = e.GetSprite();

                if (Game.Data.TryGetAsset<SpriteAsset>(s.AnimationGuid) is not SpriteAsset ase)
                {
                    // Animation not found.
                    continue;
                }

                IMurderTransformComponent transform = e.GetGlobalTransform();
                Vector2 position = transform.Vector2;

                // This is as early as we can to check for out of bounds
                if (s.TargetSpriteBatch != Batches2D.UiBatchId &&
                    !render.Camera.Bounds.Touches(new Rectangle(position - ase.Size * s.Offset - ase.Origin, ase.Size)))
                {
                    continue;
                }

                if (!ase.Animations.TryGetValue(s.CurrentAnimation, out Animation animation))
                {
                    GameLogger.Log($"Couldn't find animation {s.CurrentAnimation}.");
                    continue;
                }

                var frame = animation.Evaluate(Game.Now, true);
                var texture = ase.GetFrame(frame.Frame);

                ThreeSliceComponent threeSlice = e.GetThreeSlice();
                float ySort = RenderServices.YSort(transform.Y + s.YSortOffset);

                RenderServices.Draw3Slice(
                    render.GetBatch(s.TargetSpriteBatch),
                    texture,
                    threeSlice.CoreSliceRectangle,
                    position: position,
                    size: new(threeSlice.Size, threeSlice.Size),
                    origin: Vector2.Zero,
                    orientation: threeSlice.Orientation,
                    sort: ySort
                );
            }
        }
    }
}