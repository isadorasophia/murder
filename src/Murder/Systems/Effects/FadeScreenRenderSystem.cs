using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Components;
using Murder.Services;
using Murder.Utilities;
using System.Drawing;

namespace Murder.Systems
{
    [DoNotPause]
    [Filter(kind: ContextAccessorKind.Read, typeof(FadeScreenComponent))]
    public class FadeScreenRenderSystem : IMonoRenderSystem
    {
        private Microsoft.Xna.Framework.Graphics.RenderTarget2D? target = null;
        public void Draw(RenderContext render, Context context)
        {
            var fadeTexture = Game.Data.FetchTexture((Path.Join("images", "full-screen-wipe")));
            if (target is null)
            {
                target = new(Game.GraphicsDevice, render.Camera.Width, render.Camera.Height);
            }

            foreach (var e in context.Entities)
            {
                FadeScreenComponent fade = e.GetFadeScreen();

                float current = 0;
                float fullTime = (Game.NowUnescaled) - fade.StartedTime;
                float ratio =  fullTime / fade.Duration;
                
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
                float delta = Calculator.Clamp01(current);

                if (delta < 1)
                {
                    Game.GraphicsDevice.SetRenderTarget(target);
                    Game.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);
                    Game.Data.CustomGameShader[1].SetParameter("cutout", delta);
                    RenderServices.DrawTextureQuad(fadeTexture, fadeTexture.Bounds, new Core.Geometry.Rectangle(0, 0, target.Width, target.Height),
                        Microsoft.Xna.Framework.Matrix.Identity, fade.Color, Microsoft.Xna.Framework.Graphics.BlendState.NonPremultiplied, Game.Data.CustomGameShader[1]);
                    Game.GraphicsDevice.SetRenderTarget(null);

                    render.UiBatch.Draw(target, Microsoft.Xna.Framework.Vector2.Zero, target.Bounds.Size.ToVector2(), target.Bounds,
                        0.001f, 0, Microsoft.Xna.Framework.Vector2.One, ImageFlip.None, Microsoft.Xna.Framework.Color.White, Microsoft.Xna.Framework.Vector2.Zero,
                        RenderServices.BLEND_NORMAL);
                }
                else
                {
                    render.UiBatch.DrawRectangle(new Core.Geometry.Rectangle(0, 0, render.Camera.Width, render.Camera.Height), fade.Color, 0.001f);
                }

                if (fade.DestroyAfterFinished && (fullTime > fade.Duration + Game.FixedDeltaTime)) 
                {
                    e.Destroy();
                }
            }

            // for debugging collors
            //if (Game.Input.Down(MurderInputButtons.Submit))
            //{
            //    RenderServices.DrawRectangle(render.UiBatch,
            //        new(0, 0, render.ScreenSize.X, render.ScreenSize.Y),
            //        Color.FromHex("2c2c4d").WithAlpha(0.51f), 0.0001f); // Not zero because the letterbox borders have priority
            //}
        }
    }
}
