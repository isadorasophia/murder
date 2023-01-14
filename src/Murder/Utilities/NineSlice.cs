using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;

namespace Murder.Utilities
{
    public readonly struct NineSliceInfo
    {
        [Tooltip("Measurement of the central rectangle of the image that will be streched.")]
        public readonly Rectangle Core = Rectangle.Empty;

        [GameAssetId(typeof(AsepriteAsset))]
        public readonly Guid Image = Guid.Empty;

        public NineSliceInfo() { }

        public NineSliceInfo(Rectangle core, Guid image)
        {
            Core = core;
            Image = image;
        }

        public static NineSliceInfo Empty => new NineSliceInfo();
    }

    public readonly struct NineSlice
    {
        public readonly Rectangle Core = Rectangle.Empty;
        public readonly AsepriteAsset Image = null!;

        public NineSlice(NineSliceInfo info)
        {
            Core = info.Core;
            Image = Game.Data.GetAsset<AsepriteAsset>(info.Image);
        }

        public void Draw(Batch2D batch, Rectangle target, float sort)
        {
            var frame = Image.Animations.FirstOrDefault().Value.Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, Image.GetFrame(frame.animationFrame), Core, target, sort);
        }
        public void DrawWithText(Batch2D batch, string text, PixelFont font, Color textColor, Color? textStrokeColor, Color? textShadowColor, Rectangle target, float sort)
        {
            var frame = Image.Animations.FirstOrDefault().Value.Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, Image.GetFrame(frame.animationFrame), Core, target, sort);

            // Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth
            font.Draw(batch, text, target.Center, Vector2.Center, sort - 0.01f, textColor, textStrokeColor, textShadowColor, (int)target.Width);
        }
    }
}
