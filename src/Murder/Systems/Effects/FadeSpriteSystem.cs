using Bang.Entities;
using Bang.Contexts;
using Murder.Core.Graphics;
using Murder.Utilities;
using Bang.Systems;
using Murder.Components.Effects;
using Murder.Components;
using System.Numerics;

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

            if (fade.Flags.HasFlag(FadeSpriteFlags.Quantize8))
            {
                alpha = Calculator.Quantize(alpha, 8);
            }
            else if (fade.Flags.HasFlag(FadeSpriteFlags.Quantize16))
            {
                alpha = Calculator.Quantize(alpha, 16);
            }

            // e.SetTint(Color.White * alpha);
            if (fade.Flags.HasFlag(FadeSpriteFlags.Alpha))
            {
                e.SetAlpha(alpha);
            }

            if (fade.Flags.HasFlag(FadeSpriteFlags.Scale))
            {
                e.SetScale(Vector2.One * alpha);
            }

            if (delta >= 1)
            {
                if (fade.Flags.HasFlag(FadeSpriteFlags.DestroyOnEnd))
                {
                    e.Destroy();
                }
                else if (fade.Flags.HasFlag(FadeSpriteFlags.DeactivateOnEnd))
                {
                    e.Deactivate();
                }
                else
                {
                    e.RemoveFadeSprite();
                }
            }
        }
    }
}
