using Bang.Contexts;
using Bang.Systems;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;

namespace Murder.Systems
{
    [Filter(ContextAccessorFilter.AllOf, typeof(PositionComponent), typeof(AgentSpriteComponent), typeof(FacingComponent))]
    [ShowInEditor]
    public class AgentAnimatorSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            // TODO: Generate extensions.
            //foreach (var e in context.Entities)
            //{
            //    PositionComponent pos = e.GetGlobalPosition();
            //    if (!render.Camera.SafeBounds.Contains(pos))
            //        continue;

            //    AgentSpriteComponent sprite = e.GetAgentSprite();
            //    FacingComponent facing = e.GetFacing();
                 
            //    var ySort = RenderServices.YSort(pos.Y + sprite.YSortOffset);
            //    Vector2 impulse = Vector2.Zero;

            //    if (e.TryGetAgentImpulse() is AgentImpulseComponent imp) impulse = imp.Impulse;

            //    float start = NoiseHelper.Simple01(e.EntityId * 10) * 5f;
            //    var prefix = sprite.IdlePrefix;
            //    AnimationOverloadComponent? overload = null;
            //    if (e.TryGetAnimationOverload() is AnimationOverloadComponent o)
            //    {
            //        overload = o;
            //        prefix = o.CurrentAnimation;
            //        start = o.Start;
            //    }

            //    if (impulse.HasValue)
            //        prefix = sprite.WalkPrefix;

            //    if (Game.Data.GetAsset<AsepriteAsset>(sprite.AnimationGuid) is AsepriteAsset asepriteAsset)
            //    {
            //        var suffix = string.Empty;
            //        if (facing.LookingUp)
            //        {
            //            if (asepriteAsset.Animations.ContainsKey(prefix + sprite.UpSuffix))
            //                suffix = sprite.UpSuffix;
            //        }

            //        bool flip = false;
            //        if (e.TryGetFacing() is FacingComponent f)
            //        {
            //            flip = f.Flipped;
            //        }

            //        if (!VisionServices.GetFadeAlpha(e, out var fadeAlpha))
            //            continue;

            //        float speed = overload?.Duration ?? -1;
            //        AnimationSpeedOverload? speedOverload = e.TryGetAnimationSpeedOverload();
                    
            //        if (speedOverload is not null)
            //        {
            //            if (speed > 0)
            //                speed = speed * speedOverload.Value.Rate;
            //            else
            //            {
            //                if (asepriteAsset.Animations.TryGetValue(prefix + suffix, out var animation))
            //                {
            //                    speed = animation.AnimationDuration / speedOverload.Value.Rate;
            //                }
            //            }
            //        }
                    
            //        var complete = RenderServices.RenderSprite(
            //        render.SpriteBatch,
            //        render.Camera,
            //        pos,
            //        prefix + suffix,
            //        asepriteAsset,
            //        start,
            //        speed,
            //        Vector2.Zero,
            //        flip,
            //        0,
            //        Color.White.WithAlpha(fadeAlpha),
            //        (e.HasTemporaryInvulnerability() && fadeAlpha >= 1) ? RenderServices.BlendWash : RenderServices.BlendNormal,
            //        ySort);

            //        if (complete && overload!=null)
            //        {
            //            if (!overload.Value.Loop)
            //            {
            //                e.RemoveAnimationOverload();
            //                e.SendMessage<AnimationCompleteMessage>();
            //            }
            //            else
            //            {
            //                e.ReplaceComponent(overload.Value.PlayNext());
            //            }

            //            if (speedOverload is not null && speedOverload.Value.Persist)
            //            {
            //                e.RemoveAnimationSpeedOverload();
            //            }
            //        }
            //    }
            //}

            return default;
        }
    }
}
