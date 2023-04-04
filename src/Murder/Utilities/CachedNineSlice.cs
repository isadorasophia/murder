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

        public void Draw(Batch2D batch, Rectangle target, float sort)
        {
            AsepriteAsset image = Game.Data.GetAsset<AsepriteAsset>(Image);
            var frame = image.Animations.FirstOrDefault().Value.Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, image.GetFrame(frame.animationFrame), Core, target, sort);
        }
        public void Draw(Batch2D batch, Rectangle target, Color color, float sort)
        {
            AsepriteAsset image = Game.Data.GetAsset<AsepriteAsset>(Image);
            var frame = image.Animations.FirstOrDefault().Value.Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, image.GetFrame(frame.animationFrame), Core, target, color, sort);
        }
        
        public void Draw(Batch2D batch, Rectangle target, string animation, float sort)
        {
            AsepriteAsset image = Game.Data.GetAsset<AsepriteAsset>(Image);
            var frame = image.Animations[animation].Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, image.GetFrame(frame.animationFrame), Core, target, sort);
        }
        public void Draw(Batch2D batch, Rectangle target, string animation, Color color, float sort)
        {
            AsepriteAsset image = Game.Data.GetAsset<AsepriteAsset>(Image);
            var frame = image.Animations[animation].Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, image.GetFrame(frame.animationFrame), Core, target, color, sort);
        }
        public void DrawWithText(Batch2D batch, string text, PixelFont font, Color textColor, Color? textStrokeColor, Color? textShadowColor, Rectangle target, float sort)
        {
            AsepriteAsset image = Game.Data.GetAsset<AsepriteAsset>(Image);
            var frame = image.Animations.FirstOrDefault().Value.Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, image.GetFrame(frame.animationFrame), Core, target, sort);
            
            font.Draw(batch, text, target.Center, Vector2.Center, sort - 0.01f, textColor, textStrokeColor, textShadowColor, (int)target.Width);
        }

        public CachedNineSlice Cache()=>new(this);
    }

    public readonly struct CachedNineSlice
    {
        public readonly Rectangle _core = Rectangle.Empty;
        public readonly AsepriteAsset _image = null!;
        private readonly Animation _animation;
        public CachedNineSlice(NineSliceInfo info)
        {
            _core = info.Core;
            _image = Game.Data.GetAsset<AsepriteAsset>(info.Image);
            _animation = _image.Animations.FirstOrDefault().Value;
        }

        public void Draw(Batch2D batch, Rectangle target, float sort)
        {
            var frame = _animation.Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, _image.GetFrame(frame.animationFrame), _core, target, sort);
        }
        public void Draw(Batch2D batch, Rectangle target, Color color, float sort)
        {
            var frame = _animation.Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, _image.GetFrame(frame.animationFrame), _core, target, color, sort);
        }
        public void Draw(Batch2D batch, Rectangle target, string animation, float sort)
        {
            var frame = _image.Animations[animation].Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, _image.GetFrame(frame.animationFrame), _core, target, sort);
        }

        public void Draw(Batch2D batch, Rectangle target, string animation, Color color, float sort)
        {
            var frame = _image.Animations[animation].Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, _image.GetFrame(frame.animationFrame), _core, target, color, sort);
        }
        public void DrawWithText(Batch2D batch, string text, PixelFont font, Color textColor, Color? textStrokeColor, Color? textShadowColor, Rectangle target, float sort)
        {
            var frame = _animation.Evaluate(0, Game.NowUnescaled);
            RenderServices.Render9Slice(batch, _image.GetFrame(frame.animationFrame), _core, target, sort);

            // Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth
            font.Draw(batch, text, target.Center, Vector2.Center, sort - 0.01f, textColor, textStrokeColor, textShadowColor, (int)target.Width);
        }
    }
}
