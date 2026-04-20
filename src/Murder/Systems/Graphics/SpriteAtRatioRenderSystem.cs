using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems;

/// <summary>
/// Keep it simple, for now. Very similar to SpriteRenderSystem, but applies for ratio.
/// </summary>
[Filter(typeof(ForceSpriteDrawAtRatioComponent), typeof(SpriteComponent), typeof(PositionComponent), typeof(InCameraComponent))]
public class SpriteAtRatioRenderSystem : IMurderRenderSystem
{
    public void Draw(RenderContext render, Context context)
    {
        foreach (Entity e in context.Entities)
        {
            ForceSpriteDrawAtRatioComponent ratio = e.GetForceSpriteDrawAtRatio();

            Vector2 position = e.GetGlobalPosition();
            SpriteComponent s = e.GetSprite();

            if (Game.Data.TryGetAsset<SpriteAsset>(s.AnimationGuid) is not SpriteAsset spriteAsset)
            {
                GameLogger.Error($"Sprite GUID not found {s.AnimationGuid}");
                continue;
            }

            float ySortOffsetRaw = position.Y + s.YSortOffset;
            float ySort = RenderServices.YSort(ySortOffsetRaw + 0.01f * (e.EntityId % 20));

            DrawInfo drawInfo = new(ySort)
            {
                Origin = s.Offset,
                Sort = ySort,
                OutlineStyle = s.HighlightStyle
            };

            Batch2D batch2D = render.GetBatch(s.TargetSpriteBatch);

            FrameInfo frameInfo = batch2D.DrawSpriteAtRatio(spriteAsset, position, drawInfo, s.CurrentAnimation, ratio.Ratio);
            if (frameInfo.Complete)
            {
                RenderServices.DealWithCompleteAnimations(e, s);
            }
        }
    }
}
