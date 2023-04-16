using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Services;

namespace Murder.Core.Graphics
{
    public readonly struct NineSliceInfo
    {
        [Tooltip("Measurement of the central rectangle of the image that will be stretched.")]
        public readonly Rectangle Core = Rectangle.Empty;

        [GameAssetId(typeof(SpriteAsset))]
        public readonly Guid Image = Guid.Empty;

        public NineSliceInfo() { }

        public NineSliceInfo(Rectangle core, Guid image)
        {
            Core = core;
            Image = image;
        }
        public NineSliceInfo(Guid image)
        {
            Image = image;
        }

        public static NineSliceInfo Empty => new NineSliceInfo();

        public void Draw(Batch2D batch, Rectangle target, float sort)
        {
            SpriteAsset image = Game.Data.GetAsset<SpriteAsset>(Image);
            var frame = image.Animations.FirstOrDefault().Value.Evaluate(0, Game.NowUnescaled);
            RenderServices.Draw9Slice(batch, image.GetFrame(frame.animationFrame), Core, target, sort);
        }
        public void Draw(Batch2D batch, Rectangle target, DrawInfo info)
        {
            SpriteAsset image = Game.Data.GetAsset<SpriteAsset>(Image);
            var frame = image.Animations.FirstOrDefault().Value.Evaluate(0, info.UseScaledTime? Game.Now : Game.NowUnescaled);
            RenderServices.Draw9Slice(batch, image.GetFrame(frame.animationFrame), target, Core.IsEmpty ? image.NineSlice : Core, info);
        }

        public void Draw(Batch2D batch, Rectangle target, Color color, float sort)
        {
            SpriteAsset image = Game.Data.GetAsset<SpriteAsset>(Image);
            var frame = image.Animations.FirstOrDefault().Value.Evaluate(0, Game.NowUnescaled);
            RenderServices.Draw9Slice(batch, image.GetFrame(frame.animationFrame), Core, target, color, sort);
        }
        
        public void Draw(Batch2D batch, Rectangle target, string animation, float sort)
        {
            SpriteAsset image = Game.Data.GetAsset<SpriteAsset>(Image);
            if (image.Animations.ContainsKey(animation))
            {
                var frame = image.Animations[animation].Evaluate(0, Game.NowUnescaled);
                RenderServices.Draw9Slice(batch, image.GetFrame(frame.animationFrame), Core, target, sort);
            }
        }
        public void Draw(Batch2D batch, Rectangle target, string animation, Color color, float sort)
        {
            SpriteAsset image = Game.Data.GetAsset<SpriteAsset>(Image);
            if (image.Animations.ContainsKey(animation))
            {
                var frame = image.Animations[animation].Evaluate(0, Game.NowUnescaled);
                RenderServices.Draw9Slice(batch, image.GetFrame(frame.animationFrame), Core, target, color, sort);
            }
        }
        public void DrawWithText(Batch2D batch, string text, PixelFont font, Color textColor, Color? textStrokeColor, Color? textShadowColor, Rectangle target, float sort)
        {
            SpriteAsset image = Game.Data.GetAsset<SpriteAsset>(Image);
            var frame = image.Animations.FirstOrDefault().Value.Evaluate(0, Game.NowUnescaled);
            RenderServices.Draw9Slice(batch, image.GetFrame(frame.animationFrame), Core, target, sort);
            
            font.Draw(batch, text, target.Center, Vector2.Center, sort - 0.01f, textColor, textStrokeColor, textShadowColor, (int)target.Width);
        }

        public CachedNineSlice Cache()=>new(this);
    }

    public readonly struct CachedNineSlice
    {
        public readonly Rectangle _core = Rectangle.Empty;
        public readonly SpriteAsset _image = null!;
        private readonly Animation _animation;


        public CachedNineSlice(Guid SpriteAsset)
        {
            _image = Game.Data.GetAsset<SpriteAsset>(SpriteAsset);
            _core = _image.NineSlice;
            _animation = _image.Animations.FirstOrDefault().Value;
        }

        public CachedNineSlice(NineSliceInfo info)
        {
            _image = Game.Data.GetAsset<SpriteAsset>(info.Image);
            
            if (!_image.NineSlice.IsEmpty)
                _core = _image.NineSlice;
            else
                _core = info.Core;
            _animation = _image.Animations.FirstOrDefault().Value;
        }

        public void Draw(Batch2D batch, Rectangle target, float sort)
        {
            var frame = _animation.Evaluate(0, Game.NowUnescaled);
            RenderServices.Draw9Slice(batch, _image.GetFrame(frame.animationFrame), _core, target, sort);
        }
        public void Draw(Batch2D batch, Rectangle target, Color color, float sort)
        {
            var frame = _animation.Evaluate(0, Game.NowUnescaled);
            RenderServices.Draw9Slice(batch, _image.GetFrame(frame.animationFrame), _core, target, color, sort);
        }
        public void Draw(Batch2D batch, Rectangle target, string animation, float sort)
        {
            var frame = _image.Animations[animation].Evaluate(0, Game.NowUnescaled);
            RenderServices.Draw9Slice(batch, _image.GetFrame(frame.animationFrame), _core, target, sort);
        }

        public void Draw(Batch2D batch, Rectangle target, string animation, Color color, float sort)
        {
            var frame = _image.Animations[animation].Evaluate(0, Game.NowUnescaled);
            RenderServices.Draw9Slice(batch, _image.GetFrame(frame.animationFrame), _core, target, color, sort);
        }
        public void DrawWithText(Batch2D batch, string text, PixelFont font, Color textColor, Color? textStrokeColor, Color? textShadowColor, Rectangle target, float sort)
        {
            var frame = _animation.Evaluate(0, Game.NowUnescaled);
            RenderServices.Draw9Slice(batch, _image.GetFrame(frame.animationFrame), _core, target, sort);

            // Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth
            font.Draw(batch, text, target.Center, Vector2.Center, sort - 0.01f, textColor, textStrokeColor, textShadowColor, (int)target.Width);
        }
    }
}
