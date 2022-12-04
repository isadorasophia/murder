using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Components;
using Murder.Services;
using Murder;
using Murder.Utilities;

namespace InstallWizard.Systems
{
    [DoNotPause]
    [Filter(kind: ContextAccessorKind.Read, typeof(FadeScreenComponent))]
    public class FadeScreenRenderSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                FadeScreenComponent fade = e.GetFadeScreen();

                float current = 0;
                float fullTime = (Game.NowUnescaled - fade.StartedTime);
                float ratio = Math.Min(1, fullTime / fade.Duration);
                
                switch (fade.Fade)
                {
                    case FadeType.In:
                        current = ratio;
                        break;

                    case FadeType.Out:
                        current = 1f - ratio;
                        break;

                    case FadeType.Flash:
                        current = 1 - Math.Abs(ratio - 0.5f) * 2;
                        break;
                }

                RenderServices.DrawRectangle(render.UiBatch, 
                    new(0, 0, render.ScreenSize.X, render.ScreenSize.Y),
                    fade.Color.WithAlpha(fade.Color.A * Ease.CubeInOut(current)),0.0001f); // Not zero because the letterbox borders have priority

                if (fullTime > fade.Duration && (fade.Fade == FadeType.Flash || fade.Fade == FadeType.Out))
                {
                    e.Destroy();
                }
            }

            return default;
        }
    }
}
