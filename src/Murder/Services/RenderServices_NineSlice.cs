using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Services
{
    /// Some of the code based on https://github.com/z2oh/C3.MonoGame.Primitives2D/blob/master/Primitives2D.cs
    public static partial class RenderServices
    {

        public static void Draw9SliceWithText(Batch2D batch, Guid sprite, string text, int font, Rectangle target, DrawInfo textDrawInfo, DrawInfo sliceDrawInfo, AnimationInfo sliceAnimationInfo)
        {
            RenderServices.Draw9Slice(batch, sprite, target, sliceDrawInfo, sliceAnimationInfo);

            // Batch2D spriteBatch, string text, Vector2 position, Vector2 alignment, float sort, Color color, Color? strokeColor, Color? shadowColor, int maxWidth
            RenderServices.DrawText(batch, font, text, target.Center, (int)target.Width, textDrawInfo);
        }
        public static void Draw3Slice(
            Batch2D batch,
            AtlasCoordinates texture,
            Rectangle core,
            Vector2 position,
            Vector2 size,
            Vector2 origin,
            Orientation orientation,
            float sort)
        {
            if (orientation == Orientation.Horizontal)
            {
                var left = position.Point() + new Vector2(-size.X * origin.X, -size.Y * origin.Y).Point();

                // Left
                texture.Draw(
                    batch,
                    left,
                    clip: new IntRectangle(0, core.Y, core.X, core.Height),
                    Color.White,
                    Vector2.One,
                    0,
                    Vector2.Zero,
                    ImageFlip.None,
                    RenderServices.BLEND_NORMAL,
                    MurderBlendState.AlphaBlend,
                    sort
                    );

                var midPosition = left + new Vector2(core.X, 0).Point();
                var rightSliceSize = new Point(texture.Size.X - core.X - core.Width, core.Y);
                var midSize = new Point(size.X - core.X - rightSliceSize.X, core.Height);
                // Mid
                texture.Draw(
                    batch,
                    clip: new IntRectangle(core.X, core.Y, core.Width, core.Height),
                    target: new IntRectangle(midPosition.X, midPosition.Y, midSize.X, midSize.Y),
                    Color.White,
                    sort,
                    RenderServices.BLEND_NORMAL
                    );

                // Right
                texture.Draw(
                    batch,
                    position: new Vector2(Calculator.RoundToInt(midPosition.X + midSize.X), position.Y - Calculator.RoundToInt(size.Y * origin.Y)).Round(),
                    clip: new IntRectangle(core.X + core.Width, core.Y, core.X, core.Height),
                    Color.White,
                    Vector2.One,
                    0,
                    Vector2.Zero,
                    ImageFlip.None,
                    RenderServices.BLEND_NORMAL,
                    MurderBlendState.AlphaBlend,
                    sort
                    );
            }
            else // Vertical code is correct, but a mess. Sorry I just used copilot and prayed
            {
                // Top
                texture.Draw(
                    batch,
                    position: position - new Vector2(size.X * origin.X, size.Y * origin.Y).Round(),
                    clip: new IntRectangle(0, 0, core.Width, core.Y),
                    Color.White,
                    Vector2.One,
                    0,
                    Vector2.Zero,
                    ImageFlip.None,
                    RenderServices.BLEND_NORMAL,
                    MurderBlendState.AlphaBlend,
                    sort
                );

                // Mid
                texture.Draw(
                    batch,
                    clip: new IntRectangle(0, core.Y, core.Width, core.Height),
                    target: new Rectangle(position.X - size.X * origin.X, position.Y - size.Y * origin.Y + core.Y, size.X, size.Y - core.Y - (texture.Size.Y - core.Y - core.Height)),
                    Color.White,
                    sort,
                    RenderServices.BLEND_NORMAL
                );

                // Bottom
                texture.Draw(
                    batch,
                    position: position - new Vector2(size.X * origin.X, size.Y * origin.Y).Round() + new Vector2(0, +size.Y - (texture.Size.Y - core.Y - core.Height)),
                    clip: new IntRectangle(0, core.Y + core.Height, core.Width, texture.Size.Y - core.Y - core.Height),
                    Color.White,
                    Vector2.One,
                    0,
                    Vector2.Zero,
                    ImageFlip.None,
                    RenderServices.BLEND_NORMAL,
                    MurderBlendState.AlphaBlend,
                    sort
                );
            }

            //batch.DrawRectangleOutline(new IntRectangle(position.X - size.X * origin.X, position.Y - size.Y * origin.Y, size.X, size.Y), Color.Red);
        }


        /// <summary>
        /// Draws a 9-slice using the given texture and target rectangle. The core rectangle is specified in the Aseprite file
        /// </summary>
        public static void Draw9Slice(Batch2D batch, Guid guid, Rectangle target, DrawInfo drawInfo) =>
            Draw9Slice(batch, guid, target, NineSliceStyle.Stretch, drawInfo, AnimationInfo.Default);

        /// <summary>
        /// Draws a 9-slice using the given texture and target rectangle. The core rectangle is specified in the Aseprite file
        /// </summary>
        public static void Draw9Slice(Batch2D batch, Guid guid, Rectangle target, DrawInfo drawInfo, AnimationInfo animationInfo) =>
            Draw9Slice(batch, guid, target, NineSliceStyle.Stretch, drawInfo, animationInfo);

        /// <summary>
        /// Draws a 9-slice using the given texture and target rectangle. The core rectangle is specified in the Aseprite file
        /// </summary>
        public static void Draw9Slice(Batch2D batch, Guid guid, Rectangle target, NineSliceStyle style, DrawInfo drawInfo, AnimationInfo animationInfo)
        {
            var asset = Game.Data.GetAsset<SpriteAsset>(guid);
            if (asset.Animations.ContainsKey(animationInfo.Name))
            {
                FrameInfo anim;
                if (animationInfo.UseScaledTime)
                {
                    anim = asset.Animations[animationInfo.Name].Evaluate(Game.Now, true);
                    RenderServices.Draw9Slice(batch, asset.GetFrame(anim.Frame),
                        core: asset.NineSlice,
                        target: target,
                        style,
                        drawInfo);
                }
                else
                {
                    anim = asset.Animations[animationInfo.Name].Evaluate(Game.NowUnscaled, true);
                    RenderServices.Draw9Slice(batch, asset.GetFrame(anim.Frame),
                        core: asset.NineSlice,
                        target: target,
                        style,
                        drawInfo);
                }
            }
            else
            {
                GameLogger.Log($"animation {animationInfo.Name} doesn't exist for aseprite {asset.Name}.");
            }
        }

        public static void Draw9Slice(Batch2D batch, AtlasCoordinates texture, Rectangle core, Rectangle target, float sort) =>
            Draw9Slice(batch, texture, core, target, NineSliceStyle.Stretch, new DrawInfo() { Sort = sort });

        public static void Draw9Slice(
            Batch2D batch,
            AtlasCoordinates texture,
            IntRectangle core,
            IntRectangle target,
            NineSliceStyle style,
            DrawInfo info)
        {
            var fullSize = texture.Size;
            // The size of the bottom right rectangle of the 9slice. Cached here for speed
            var bottomRightSize = new Vector2(fullSize.X - core.X - core.Width, fullSize.Y - core.Y - core.Height);
            var sort = info.Sort;

            IntRectangle finalTarget = target - (target.Size * info.Origin).Point();
            if (info.Outline != null)
            {
                Draw9SliceImpl(batch, texture, core, finalTarget + new Point(0, 1), fullSize, bottomRightSize, info.Outline.Value, sort + 0.001f, true, style);
                Draw9SliceImpl(batch, texture, core, finalTarget + new Point(1, 0), fullSize, bottomRightSize, info.Outline.Value, sort + 0.001f, true, style);
                Draw9SliceImpl(batch, texture, core, finalTarget + new Point(0, -1), fullSize, bottomRightSize, info.Outline.Value, sort + 0.001f, true, style);
                Draw9SliceImpl(batch, texture, core, finalTarget + new Point(-1, 0), fullSize, bottomRightSize, info.Outline.Value, sort + 0.001f, true, style);

                if (info.Shadow != null)
                {
                    Draw9SliceImpl(batch, texture, core, finalTarget + new Point(0, 2), fullSize, bottomRightSize, info.Shadow.Value, sort + 0.002f, true, style);
                }
            }
            else if (info.Shadow != null)
            {
                Draw9SliceImpl(batch, texture, core, finalTarget + new Point(0, 1), fullSize, bottomRightSize, info.Shadow.Value, sort + 0.002f, true, style);
            }

            Draw9SliceImpl(batch, texture, core, finalTarget, fullSize, bottomRightSize, info.Color, sort, false, style);

            if (info.Debug)
            {
                DrawRectangleOutline(batch, finalTarget, Color.Red);
            }
        }

        private static void Draw9SliceImpl(Batch2D batch, AtlasCoordinates texture, Rectangle core, Rectangle target, Point fullTextureSize, Vector2 bottomRightSize, Color color, float sort, bool wash, NineSliceStyle style)
        {
            var blend = wash ? RenderServices.BLEND_WASH : RenderServices.BLEND_NORMAL;
            // TopLeft
            texture.Draw(
                batch,
                clip: new IntRectangle(0, 0, core.X, core.Y),
                target: new Rectangle(target.Left, target.Top, core.X, core.Y),
                color,
                sort,
                blend
                );

            // Top
            switch (style)
            {
                case NineSliceStyle.Stretch or NineSliceStyle.StrechHollow:
                    texture.Draw(
                        batch,
                        clip: new IntRectangle(core.X, 0, core.Width, core.Y),
                        target: new Rectangle(target.Left + core.X, target.Top, target.Width - (fullTextureSize.X - core.Width), core.Y),
                        color,
                        sort,
                        blend
                        );
                    break;
                case NineSliceStyle.Tile or NineSliceStyle.TileHollow:
                    float totalWidth = target.Width - (core.X + bottomRightSize.Width());
                    int tiles = Calculator.FloorToInt(totalWidth / core.Width);
                    int remainder = Calculator.CeilToInt(core.Width * ((totalWidth / core.Width) - tiles));
                    for (int i = 0; i < tiles; i++)
                    {
                        texture.Draw(
                        batch,
                        clip: new IntRectangle(core.X, 0, core.Width, core.Y),
                        target: new Rectangle(target.Left + core.X + i * core.Width, target.Top, core.Width, core.Y),
                        color,
                        sort,
                        blend
                        );
                    }
                    if (remainder > 0)
                    {
                        texture.Draw(
                            batch,
                            clip: new IntRectangle(core.X, 0, remainder, core.Y),
                            target: new Rectangle(target.Left + core.X + tiles * core.Width, target.Top, remainder, core.Y),
                            color,
                            sort,
                            blend
                            );
                    }
                    break;
                default:
                    GameLogger.Error("Unknown 9 Slice style");
                    return;
            }


            // TopRight
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X + core.Width, 0, bottomRightSize.Width(), core.Y),
                target: new Rectangle(target.Right - bottomRightSize.Width(), target.Top, bottomRightSize.Width(), core.Y),
                color,
                sort,
                blend
                );

            // Left
            switch (style)
            {
                case NineSliceStyle.Stretch or NineSliceStyle.StrechHollow:
                    texture.Draw(
                        batch,
                        clip: new IntRectangle(0, core.Y, core.X, core.Height),
                        target: new Rectangle(target.Left, target.Top + core.Y, core.X, target.Height - (fullTextureSize.Y - core.Height)),
                        color,
                        sort,
                        blend
                        );
                    break;
                case NineSliceStyle.Tile or NineSliceStyle.TileHollow:
                    float totalHeight = target.Height - (fullTextureSize.Y - core.Height);
                    int tiles = Calculator.FloorToInt(totalHeight / core.Height);
                    int remainder = Calculator.RoundToInt(core.Height * ((totalHeight / core.Height) - tiles));
                    for (int i = 0; i < tiles; i++)
                    {
                        texture.Draw(
                        batch,
                        clip: new IntRectangle(0, core.Y, core.X, core.Height),
                        target: new Rectangle(target.Left, target.Top + core.Y + i * core.Height, core.X, core.Height),
                        color,
                        sort,
                        blend
                        );
                    }
                    if (remainder > 0)
                    {
                        texture.Draw(
                        batch,
                        clip: new IntRectangle(0, core.Y, core.X, remainder),
                        target: new Rectangle(target.Left, target.Top + core.Y + tiles * core.Height, core.X, remainder),
                        color,
                        sort,
                        blend
                        );
                    }
                    break;
                default:
                    GameLogger.Error("Unknown 9 Slice style");
                    return;
            }

            // Center
            if (style != NineSliceStyle.TileHollow && style != NineSliceStyle.StrechHollow)
            {
                texture.Draw(
                    batch,
                    clip: new IntRectangle(core.X, core.Y, core.Width, core.Height),
                    target: new Rectangle(target.Left + core.X, target.Top + core.Y, target.Width - (fullTextureSize.X - core.Width), target.Height - (fullTextureSize.Y - core.Height)),
                    color,
                    sort,
                    blend
                    );
            }

            // Right
            switch (style)
            {
                case NineSliceStyle.Stretch or NineSliceStyle.StrechHollow:
                    texture.Draw(
                        batch,
                        clip: new IntRectangle(core.X + core.Width, core.Y, bottomRightSize.Width(), core.Height),
                        target: new Rectangle(target.Right - bottomRightSize.Width(), target.Top + core.Y, bottomRightSize.Width(), target.Height - (fullTextureSize.Y - core.Height)),
                        color,
                        sort,
                        blend
                        );
                    break;
                case NineSliceStyle.Tile or NineSliceStyle.TileHollow:
                    float totalHeight = target.Height - (fullTextureSize.Y - core.Height);
                    int tiles = Calculator.FloorToInt(totalHeight / core.Height);
                    int remainder = Calculator.RoundToInt(core.Height * ((totalHeight / core.Height) - tiles));
                    for (int i = 0; i < tiles; i++)
                    {
                        texture.Draw(
                            batch,
                            clip: new IntRectangle(core.X + core.Width, core.Y, bottomRightSize.Width(), core.Height),
                            target: new Rectangle(target.Right - bottomRightSize.Width(), target.Top + core.Y + i * core.Height, bottomRightSize.Width(), core.Height),
                            color,
                            sort,
                            blend
                            );
                    }

                    if (remainder > 0)
                    {
                        texture.Draw(
                        batch,
                            clip: new IntRectangle(core.X + core.Width, core.Y, bottomRightSize.Width(), remainder),
                        target: new Rectangle(target.Right - bottomRightSize.Width(), target.Top + core.Y + tiles * core.Height, bottomRightSize.Width(), remainder),
                        color,
                        sort,
                        blend
                        );
                    }
                    break;
                default:
                    GameLogger.Error("Unknown 9 Slice style");
                    return;
            }

            // BottomLeft
            texture.Draw(
                batch,
                clip: new IntRectangle(0, fullTextureSize.Y - bottomRightSize.Y, core.X, bottomRightSize.Y),
                target: new Rectangle(target.Left, target.Bottom - bottomRightSize.Y, core.X, bottomRightSize.Y),
                color,
                sort,
                blend
                );

            // Bottom
            switch (style)
            {
                case NineSliceStyle.Stretch or NineSliceStyle.StrechHollow:
                    texture.Draw(
                        batch,
                        clip: new IntRectangle(core.X, fullTextureSize.Y - bottomRightSize.Y, core.Width, bottomRightSize.Y),
                        target: new Rectangle(target.Left + core.X, target.Bottom - bottomRightSize.Y, target.Width - (fullTextureSize.X - core.Width), bottomRightSize.Y),
                        color,
                        sort,
                        blend
                        );
                    break;

                case NineSliceStyle.Tile or NineSliceStyle.TileHollow:
                    float totalWidth = target.Width - (core.X + bottomRightSize.Width());
                    int tiles = Calculator.FloorToInt(totalWidth / core.Width);
                    int remainder = Calculator.CeilToInt(core.Width * ((totalWidth / core.Width) - tiles));

                    for (int i = 0; i < tiles; i++)
                    {
                        texture.Draw(
                        batch,
                        clip: new IntRectangle(core.X, fullTextureSize.Y - bottomRightSize.Y, core.Width, core.Height),
                        target: new Rectangle(target.Left + core.X + i * core.Width, target.Bottom - bottomRightSize.Y, core.Width, core.Height),
                        color,
                        sort,
                        blend
                        );
                    }
                    if (remainder > 0)
                    {
                        texture.Draw(
                        batch,
                        clip: new IntRectangle(core.X, fullTextureSize.Y - bottomRightSize.Y, remainder, core.Height),
                        target: new Rectangle(target.Left + core.X + tiles * core.Width, target.Bottom - bottomRightSize.Y, remainder, core.Height),
                        color,
                        sort,
                        blend
                        );
                    }
                    break;
                default:
                    GameLogger.Error("Unknown 9 Slice style");
                    return;
            }

            // BottomRight
            texture.Draw(
                batch,
                clip: new IntRectangle(fullTextureSize.X - bottomRightSize.X, fullTextureSize.Y - bottomRightSize.Y, bottomRightSize.X, bottomRightSize.Y),
                target: new Rectangle(target.Right - bottomRightSize.X, target.Bottom - bottomRightSize.Y, bottomRightSize.X, bottomRightSize.Y),
                color,
                sort,
                blend
                );
        }

    }
}