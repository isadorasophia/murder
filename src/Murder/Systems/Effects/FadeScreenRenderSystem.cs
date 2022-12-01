using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Core;
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
            if (context.HasAnyEntity)
            {
                FadeScreenComponent fade = context.Entity.GetFadeScreen();

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
                }

                RenderServices.DrawRectangle(render.UiBatch, 
                    new(0, 0, render.ScreenSize.X, render.ScreenSize.Y), 
                    new(0, 0, 0, Ease.CubeIn(current)));
            }

            return default;
        }
    }
}
