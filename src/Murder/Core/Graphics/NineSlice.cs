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
        
        public void Draw(Batch2D batch, Rectangle target, DrawInfo info, AnimationInfo animationInfo)
        {
            SpriteAsset image = Game.Data.GetAsset<SpriteAsset>(Image);
            var frame = image.Animations.FirstOrDefault().Value.Evaluate(0, animationInfo.UseScaledTime? Game.Now : Game.NowUnscaled, true);
            RenderServices.Draw9Slice(batch, image.GetFrame(frame.Frame),Core.IsEmpty ? image.NineSlice : Core, target, info);
        }
        
        public void Draw(Batch2D batch, Rectangle target, string animation, Color color, float sort)
        {
            SpriteAsset image = Game.Data.GetAsset<SpriteAsset>(Image);
            if (image.Animations.ContainsKey(animation))
            {
                var anim = image.Animations[animation].Evaluate(0, Game.NowUnscaled, true);
                RenderServices.Draw9Slice(batch, image.GetFrame(anim.Frame), Core, target, new DrawInfo(color, sort));
            }
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

        public void Draw(Batch2D batch, Rectangle target, DrawInfo drawInfo)
        {
            var anim = _animation.Evaluate(0, Game.NowUnscaled, true);
            RenderServices.Draw9Slice(batch, _image.GetFrame(anim.Frame), _core, target, drawInfo);
        }
        public void Draw(Batch2D batch, Rectangle target, DrawInfo drawInfo, AnimationInfo animationInfo)
        {
            if (!_image.Animations.TryGetValue(animationInfo.Name, out Animation animation))
            {
                animation = _animation;
            }

            FrameInfo frameInfo = animation.Evaluate(animationInfo.Start, animationInfo.UseScaledTime ? Game.Now : Game.NowUnscaled, animationInfo.Loop);
            RenderServices.Draw9Slice(batch, _image.GetFrame(frameInfo.Frame), _core, target, drawInfo);
        }
        
        public void DrawWithText(Batch2D batch, string text, int font, Color textColor, Color? textOutlineColor, Color? textShadowColor, Rectangle target, float sort)
        {
            var anim = _animation.Evaluate(0, Game.NowUnscaled, true);
            RenderServices.Draw9Slice(batch, _image.GetFrame(anim.Frame), _core, target, sort);

            // Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth
            RenderServices.DrawText(batch, font, text, target.Center, (int)target.Width, new DrawInfo(sort - 0.001f)
            {
                Origin = Vector2.Center,
                Color = textColor,
                Outline = textOutlineColor,
                Shadow = textShadowColor
            });
        }
    }
}
