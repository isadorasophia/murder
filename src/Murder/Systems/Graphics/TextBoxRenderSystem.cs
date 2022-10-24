using Bang.Contexts;
using Bang.Systems;
using Microsoft.Xna.Framework.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Entities;
using Murder.Utilities;

namespace Murder.Systems.Graphics
{
    [Filter(typeof(RectPositionComponent), typeof(TextBoxComponent))]
    public  class TextBoxRenderSystem : IMonoRenderSystem
    { 
        public ValueTask Draw(RenderContext render, Context context)
        {
            var _textBatch = new Batch2D(Game.GraphicsDevice);

            foreach (var e in context.Entities)
            {
                TextBoxComponent textBox = e.GetTextBox();

                if (string.IsNullOrWhiteSpace(textBox.Text))
                    continue;

                float scale = Math.Min(
                    (float)render.ScreenSize.X / render.UiReferenceScale.X,
                    (float)render.ScreenSize.Y / render.UiReferenceScale.Y);
                float FontSize = (float)textBox.FontSize * scale;
                Rectangle box = e.GetRectPosition().GetBox(e, render.ScreenSize, render.UiReferenceScale);
                var key = $"{textBox.Text}x{FontSize}({textBox.VisibleCharacters})";
                Texture2D texture;

                if (render.CachedTextTextures.ContainsKey(key))
                {
                    texture = render.CachedTextTextures[key];
                }
                else
                {
                    Point boundingBoxSize = new(
                        Calculator.RoundToInt(Game.Data.LargeFont.GetLineWidth(FontSize, textBox.Text)),
                        Calculator.CeilToInt(FontSize + 2)
                        );

                    if (boundingBoxSize.X == 0 || boundingBoxSize.Y == 0)
                    {
                        continue;
                    }

                    var target = new RenderTarget2D(Game.GraphicsDevice, boundingBoxSize.X, boundingBoxSize.Y);

                    Game.GraphicsDevice.SetRenderTarget(target);
                    Game.GraphicsDevice.Clear(Color.Transparent);

                    _textBatch.Begin(
                        effect: Game.Data.SimpleShader,
                        blendState: BlendState.AlphaBlend,
                        sampler: SamplerState.AnisotropicWrap
                        );
                    Game.Data.LargeFont.Draw(FontSize, _textBatch, textBox.Text, textBox.VisibleCharacters, Vector2.Zero, Vector2.Zero, Color.White);
                    _textBatch.End();

                    texture = target;
                    render.CachedTextTextures[key] = texture;

                    //Game.Data.FontShader.CurrentTechnique.Passes[0].Apply();
                    //Game.Data.FontShader.Parameters["ScreenPixelRange"].SetValue(8f);
                    //Game.Data.FontShader.Parameters["DiffuseColor"].SetValue(Vector4.One);

                    //Matrix.CreateOrthographicOffCenter(
                    //    left: 0,
                    //    right: size.Width,
                    //    bottom: size.Height,
                    //    top: 0,
                    //    zNearPlane: 0,
                    //    zFarPlane: -1,
                    //    out Matrix projection);

                    //Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.Forward, Vector3.Up);
                    //Matrix.Multiply(ref view, ref projection, out Matrix worldViewProjection);
                    //Game.Data.FontShader.Parameters["WorldViewProj"].SetValue(worldViewProjection);
                }

                var size = new Vector2(texture.Width, texture.Height);
                var position = Vector2.Round(
                    box.TopLeft
                    - new Vector2(size.X * textBox.Offset.X, size.Y * textBox.Offset.Y)
                    + new Vector2(box.Width * textBox.Offset.X, box.Height * textBox.Offset.Y)
                    );
                render.UiBatch.Draw(texture, new Rectangle(position, size), textBox.Color, textBox.Sorting);
            }

            return default;
        }
    }
}
