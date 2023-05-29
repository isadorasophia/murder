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
        private Microsoft.Xna.Framework.Graphics.RenderTarget2D? _target = null;
        public void Draw(RenderContext render, Context context)
        {
            if (_target is null)
            {
                _target = new(Game.GraphicsDevice, render.Camera.Width, render.Camera.Height);
            }

            foreach (var e in context.Entities)
            {
                FadeScreenComponent fade = e.GetFadeScreen();

                float current = 0;
                float fullTime = (Game.NowUnescaled) - fade.StartedTime;
                float ratio =  (fullTime / fade.Duration) * 1.2f;
                
                switch (fade.Fade)
                {
                    case FadeType.In:
                        Game.Data.CustomGameShader[1].SetParameter("invert", 1f);
                        current = ratio;
                        break;

                    case FadeType.Out:
                        Game.Data.CustomGameShader[1].SetParameter("invert", -1f);
                        current = 1f - ratio;
                        break;

                    case FadeType.Flash:
                        Game.Data.CustomGameShader[1].SetParameter("invert", -1f);
                        current = 1 - Math.Abs(ratio - 0.5f) * 2;
                        break;
                }
                float delta = Calculator.Clamp01(current);

                if (delta < 1)
                {
                    string textureName = string.IsNullOrWhiteSpace(fade.CustomTexture)? "images\\full-screen-wipe" : fade.CustomTexture;
                    var fadeTexture = Game.Data.FetchTexture(textureName);


                    Game.GraphicsDevice.SetRenderTarget(_target);
                    Game.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);
                    Game.Data.CustomGameShader[1].SetParameter("cutout", delta);
                    RenderServices.DrawTextureQuad(fadeTexture, fadeTexture.Bounds, new Core.Geometry.Rectangle(0, 0, _target.Width, _target.Height),
                        Microsoft.Xna.Framework.Matrix.Identity, fade.Color, Microsoft.Xna.Framework.Graphics.BlendState.NonPremultiplied, Game.Data.CustomGameShader[1]);
                    Game.GraphicsDevice.SetRenderTarget(null);

                    render.UiBatch.Draw(_target, Microsoft.Xna.Framework.Vector2.Zero, _target.Bounds.Size.ToVector2(), _target.Bounds,
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
