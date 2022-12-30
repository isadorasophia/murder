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
using Murder.Services;
using Murder.Utilities;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(ThreeSliceComponent), typeof(ITransformComponent))]
    public class AsepriteThreeSliceRenderSystem : IMonoRenderSystem
    {
        /// <summary>
        /// This draws an aseprite three slice component.
        /// TODO: Support animations?
        /// </summary>
        public void Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                AsepriteComponent s = e.GetAseprite();

                if (Game.Data.TryGetAsset<AsepriteAsset>(s.AnimationGuid) is not AsepriteAsset ase)
                {
                    // Animation not found.
                    continue;
                }

                IMurderTransformComponent transform = e.GetGlobalTransform();
                Vector2 position = transform.Vector2;

                // This is as early as we can to check for out of bounds
                if (s.TargetSpriteBatch != TargetSpriteBatches.Ui && 
                    !render.Camera.Bounds.Touches(new Rectangle(position - ase.Size * s.Offset - ase.Origin, ase.Size)))
                {
                    continue;
                }

                if (!ase.Animations.TryGetValue(s.CurrentAnimation, out Animation animation))
                {
                    GameLogger.Log($"Couldn't find animation {s.CurrentAnimation}.");
                    continue;
                }

                var (imgPath, _) = animation.Evaluate(0, 0);
                if (Game.Data.TryFetchAtlas(Data.AtlasId.Gameplay)?.Get(imgPath) is not AtlasTexture texture)
                {
                    GameLogger.Log($"Couldn't find animation {s.CurrentAnimation}:{imgPath}.");
                    continue;
                }

                ThreeSliceComponent threeSlice = e.GetThreeSlice();
                float ySort = RenderServices.YSort(transform.Y + s.YSortOffset);

                RenderServices.Render3Slice(
                    render.GetSpriteBatch(s.TargetSpriteBatch),
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
