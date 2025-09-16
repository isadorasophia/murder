using Bang.Entities;
using Bang.Contexts;
using Murder.Core.Graphics;
using Murder.Utilities;
using Bang.Systems;
using Murder.Components.Effects;

namespace Murder.Systems.Effects;

[Filter(ContextAccessorFilter.AllOf, typeof(FadeSpriteComponent))]
public class FadeSpriteSystem : IMonoPreRenderSystem
{
    public void BeforeDraw(Context context)
    {
        foreach (var e in context.Entities)
        {
            var fade = e.GetFadeSprite();

            float delta = Calculator.ClampTime(Game.Now - fade.FadeStartTime, fade.FadeEndTime - fade.FadeStartTime);

            float alpha = Calculator.Lerp(fade.StartAlpha, fade.EndAlpha, delta);

            e.SetTint(Color.White * alpha);

            if (delta >= 1)
            {
                if (fade.DestroyOnEnd)
                {
                    e.Destroy();
                }
                else
                {
                    e.RemoveFadeSprite();
                }
            }
        }
    }
}
