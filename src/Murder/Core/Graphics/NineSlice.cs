using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Core.Graphics;

public enum NineSliceStyle
{
    Stretch,
    Tile,
    TileHollow,
    StrechHollow
}

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
        if (!image.Animations.TryGetValue(animationInfo.Name, out Animation animation))
        {
            animation = image.Animations.FirstOrDefault().Value;
        }

        FrameInfo frame = animation.Evaluate(animationInfo.UseScaledTime ? Game.Now : Game.NowUnscaled, true);
        RenderServices.Draw9Slice(batch, image.GetFrame(frame.Frame), Core.IsEmpty ? image.NineSlice : Core, target, NineSliceStyle.Stretch, info);
    }

    public void Draw(Batch2D batch, Rectangle target, string animation, Color color, float sort)
    {
        SpriteAsset image = Game.Data.GetAsset<SpriteAsset>(Image);
        if (image.Animations.ContainsKey(animation))
        {
            var anim = image.Animations[animation].Evaluate(Game.NowUnscaled, true);
            RenderServices.Draw9Slice(batch, image.GetFrame(anim.Frame), Core, target, NineSliceStyle.Stretch, new DrawInfo(color, sort));
        }
    }

    public CachedNineSlice Cache() => new(this);
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
        var anim = _animation.Evaluate(Game.NowUnscaled, true);
        RenderServices.Draw9Slice(batch, _image.GetFrame(anim.Frame), _core, target, NineSliceStyle.Stretch, drawInfo);
    }
    public void Draw(Batch2D batch, Rectangle target, DrawInfo drawInfo, AnimationInfo animationInfo)
    {
        if (!_image.Animations.TryGetValue(animationInfo.Name, out Animation animation))
        {
            animation = _animation;
        }

        FrameInfo frameInfo = animation.Evaluate((animationInfo.UseScaledTime ? Game.Now : Game.NowUnscaled) - animationInfo.Start, animationInfo.Loop);
        RenderServices.Draw9Slice(batch, _image.GetFrame(frameInfo.Frame), _core, target, NineSliceStyle.Stretch, drawInfo);
    }
}