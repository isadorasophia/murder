using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core.Graphics;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.NoneOf, typeof(AnimationStartedComponent))]
    [Filter(ContextAccessorFilter.AllOf, typeof(SpriteComponent))]
    internal class SpriteAnimationStarterSystem : IMonoPreRenderSystem
    {
        public void BeforeDraw(Context context)
        {
            foreach (var e in context.Entities)
            {
                var sprite = e.GetSprite();
                e.SetAnimationStarted(sprite.UseUnscaledTime ? Game.UnscaledDeltaTime : Game.Now);
            }
        }
    }
}