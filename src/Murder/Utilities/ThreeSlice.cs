using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using System.Numerics;

namespace Murder.Utilities
{
    public readonly struct ThreeSliceInfo
    {
        public readonly Rectangle Core = Rectangle.Empty;

        [GameAssetId(typeof(SpriteAsset))]
        public readonly Guid Image = Guid.Empty;

        public ThreeSliceInfo() { }

        public ThreeSliceInfo(Rectangle core, Guid image)
        {
            Core = core;
            Image = image;
        }

        public static ThreeSliceInfo Empty => new();
    }

    public readonly struct ThreeSlice
    {
        public readonly Rectangle Core = Rectangle.Empty;
        public readonly SpriteAsset Image = null!;
        
        public ThreeSlice(ThreeSliceInfo info)
        {
            Core = info.Core;
            Image = Game.Data.GetAsset<SpriteAsset>(info.Image);
        }

        public void Draw(Batch2D batch, Rectangle target, Vector2 origin, Orientation orientation, float sort)
        {
            var frame = Image.Animations.FirstOrDefault().Value.Evaluate(0, Game.NowUnscaled, true);
            RenderServices.Draw3Slice(batch, Image.GetFrame(frame.Frame), Core, target.TopLeft, target.Size, origin, orientation, sort);
        }
    }
}
