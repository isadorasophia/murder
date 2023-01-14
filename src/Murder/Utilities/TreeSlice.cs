using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;

namespace Murder.Utilities
{
    public readonly struct TreeSliceInfo
    {
        public readonly Rectangle Core = Rectangle.Empty;

        [GameAssetId(typeof(AsepriteAsset))]
        public readonly Guid Image = Guid.Empty;

        public TreeSliceInfo() { }

        public TreeSliceInfo(Rectangle core, Guid image)
        {
            Core = core;
            Image = image;
        }

        public static TreeSliceInfo Empty => new TreeSliceInfo();
    }

    public readonly struct TreeSlice
    {
        public readonly Rectangle Core = Rectangle.Empty;
        public readonly AsepriteAsset Image = null!;
        
        public TreeSlice(TreeSliceInfo info)
        {
            Core = info.Core;
            Image = Game.Data.GetAsset<AsepriteAsset>(info.Image);
        }

        public void Draw(Batch2D batch, Rectangle target, Vector2 origin, Orientation orientation, float sort)
        {
            var frame = Image.Animations.FirstOrDefault().Value.Evaluate(0, Game.NowUnescaled);
            RenderServices.Render3Slice(batch, Image.GetFrame(frame.animationFrame), Core, target.TopLeft, target.Size, origin, orientation, sort);
        }
    }
}
