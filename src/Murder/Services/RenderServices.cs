using Microsoft.Xna.Framework.Graphics;
using Bang.Entities;

using Murder.Core.Graphics;
using Murder.Core.Geometry;
using Murder.Assets.Graphics;
using Murder.Utilities;
using Murder.Components;
using Murder.Messages;

using Matrix = Microsoft.Xna.Framework.Matrix;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Murder.Diagnostics;
using Murder.Core;
using Murder.Services.Info;
using Murder.Core.Input;
using System.Collections.Immutable;

namespace Murder.Services
{
    /// Some of the code based on https://github.com/z2oh/C3.MonoGame.Primitives2D/blob/master/Primitives2D.cs
    public static class RenderServices
    {
        const int Y_SORT_RANGE = 10000;
        const int Y_SORT_RANGE_X2 = Y_SORT_RANGE * 2;

        public static DrawMenuInfo DrawVerticalMenu(
            Batch2D batch,
            in Point position,
            in DrawMenuStyle style,
            in MenuInfo menuInfo) =>
            DrawVerticalMenu(batch, position, position, style, menuInfo);

        /// <summary>
        /// TODO: Pass around a "style" for background color, sounds, etc.
        /// </summary>
        public static DrawMenuInfo DrawVerticalMenu(
            Batch2D batch,
            in Point position,
            in Point textPosition,
            in DrawMenuStyle style,
            in MenuInfo menuInfo)
        {
            var font = Game.Data.GetFont(style.Font);
            int lineHeight = font.LineHeight + style.ExtraVerticalSpace;

            Point finalPosition = new Point(Math.Max(position.X, 0), Math.Max(position.Y, 0));
            Point textFinalPosition = new Point(Math.Max(textPosition.X, 0), Math.Max(textPosition.Y, 0));

            Vector2 CalculateText(int index) => new Point(0, lineHeight * (index + 1)) + textFinalPosition;
            Vector2 CalculateSelector(int index) => new Point(0, lineHeight * (index + 1)) + finalPosition;
            
            for (int i = 0; i < menuInfo.Length; i++)
            {
                var label = menuInfo.GetOptionText(i);
                var labelPosition = CalculateText(i);

                Color currentColor;
                Color? currentShadow;

                if (menuInfo.IsEnabled(i))
                {
                    currentColor = i == menuInfo.Selection ? style.SelectedColor : style.Color;
                    currentShadow = style.Shadow;
                }
                else
                {
                    currentColor = style.Shadow;
                    currentShadow = null;
                }

                Point textSize = DrawText(batch, style.Font, label ?? string.Empty, labelPosition, new DrawInfo(0.1f)
                {
                    Origin = style.Origin,
                    Color = currentColor,
                    Shadow = currentShadow
                });

                // We did not implement vertical icon menu with other offsets.
                if (i < menuInfo.Icons.Length && style.Origin == Vector2.Zero)
                {
                    Portrait portrait = menuInfo.Icons[i];
                    if (MurderAssetHelpers.GetSpriteAssetForPortrait(portrait) is (SpriteAsset sprite, string animation))
                    {
                        DrawSprite(
                            batch,
                            sprite,
                            labelPosition - new Point(15, -2),
                            new DrawInfo(sort: 0f),
                            new AnimationInfo(animation));
                    }
                }
            }

            Vector2 selectorPosition = CalculateSelector(menuInfo.Selection) + new Vector2(0, lineHeight/2f - 3);
            Vector2 previousSelectorPosition = CalculateSelector(menuInfo.PreviousSelection) + new Vector2(0, lineHeight / 2f - 2);

            Vector2 easedPosition;
            if (style.SelectorMoveTime == 0)
                easedPosition = selectorPosition;
            else
                easedPosition = Vector2.Lerp(previousSelectorPosition, selectorPosition,
                Ease.Evaluate(Calculator.ClampTime(Game.NowUnescaled - menuInfo.LastMoved, style.SelectorMoveTime),style.Ease));

            return new DrawMenuInfo() {
                SelectorPosition = selectorPosition,
                PreviousSelectorPosition = previousSelectorPosition,
                SelectorEasedPosition = easedPosition.Point
            };
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
                var left = position.Point + new Vector2(-size.X * origin.X, -size.Y * origin.Y).Point;

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
                    sort
                    );

                var midPosition = left + new Vector2(core.X, 0).Point;
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
                    sort
                );
            }
            
            //batch.DrawRectangleOutline(new IntRectangle(position.X - size.X * origin.X, position.Y - size.Y * origin.Y, size.X, size.Y), Color.Red);
        }


        /// <summary>
        /// Draws a 9-slice using the given texture and target rectangle. The core rectangle is specified in the Aseprite file
        /// </summary>
        public static void Draw9Slice(Batch2D batch, Guid guid, Rectangle target, DrawInfo drawInfo) =>
            Draw9Slice(batch, guid, target, drawInfo, AnimationInfo.Default);

        /// <summary>
        /// Draws a 9-slice using the given texture and target rectangle. The core rectangle is specified in the Aseprite file
        /// </summary>
        public static void Draw9Slice(Batch2D batch, Guid guid, Rectangle target, DrawInfo drawInfo, AnimationInfo animationInfo)
        {
            var asset = Game.Data.GetAsset<SpriteAsset>(guid);
            if (asset.Animations.ContainsKey(animationInfo.Name))
            {
                var anim = asset.Animations[animationInfo.Name].Evaluate(0, animationInfo.UseScaledTime ? Game.Now : Game.NowUnescaled);
                RenderServices.Draw9Slice(batch, asset.GetFrame(anim.Frame),
                    core: asset.NineSlice,
                    target: target,
                    drawInfo);
            }
            else
            {
                GameLogger.Log($"animation {animationInfo.Name} doesn't exist for aseprite {asset.Name}.");
            }
        }
        
        public static void Draw9Slice(Batch2D batch, AtlasCoordinates texture, Rectangle core, Rectangle target, float sort) =>
            Draw9Slice(batch, texture, core, target, new DrawInfo() { Sort = sort });

        public static void Draw9Slice(
        Batch2D batch,
        AtlasCoordinates texture,
        IntRectangle core,
        IntRectangle target,
        DrawInfo info)
        {
            var fullSize = texture.Size;
            var bottomRightSize = new Vector2(fullSize.X - core.X - core.Width, fullSize.Y - core.Y - core.Height);
            var sort = info.Sort;

            if (info.Outline != null)
            {
                Draw9SliceImpl(batch, texture, core, target + new Point(0, 1), fullSize, bottomRightSize, info.Outline.Value, sort + 0.001f, true);
                Draw9SliceImpl(batch, texture, core, target + new Point(1, 0), fullSize, bottomRightSize, info.Outline.Value, sort + 0.001f, true);
                Draw9SliceImpl(batch, texture, core, target + new Point(0, -1), fullSize, bottomRightSize, info.Outline.Value, sort + 0.001f, true);
                Draw9SliceImpl(batch, texture, core, target + new Point(-1, 0), fullSize, bottomRightSize, info.Outline.Value, sort + 0.001f, true);
                
                if (info.Shadow != null)
                {
                    Draw9SliceImpl(batch, texture, core, target + new Point(0, 2), fullSize, bottomRightSize, info.Shadow.Value, sort + 0.002f, true);
                }
            }
            else if (info.Shadow != null)
            {
                Draw9SliceImpl(batch, texture, core, target + new Point(0,1), fullSize, bottomRightSize, info.Shadow.Value, sort + 0.002f, true);
            }

            Draw9SliceImpl(batch, texture, core, target, fullSize, bottomRightSize, info.Color, sort, false);
        }

        private static void Draw9SliceImpl(Batch2D batch, AtlasCoordinates texture, Rectangle core, Rectangle target, Point fullSize, Vector2 bottomRightSize, Color color, float sort, bool wash)
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
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X, 0, core.Width, core.Y),
                target: new Rectangle(target.Left + core.X, target.Top, target.Width - (fullSize.X - core.Width), core.Y),
                color,
                sort,
                blend
                );

            // TopRight
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X + core.Width, 0, bottomRightSize.Width, core.Y),
                target: new Rectangle(target.Right - bottomRightSize.Width, target.Top, bottomRightSize.Width, core.Y),
                color,
                sort,
                blend
                );

            // Left
            texture.Draw(
                batch,
                clip: new IntRectangle(0, core.Y, core.X, core.Height),
                target: new Rectangle(target.Left, target.Top + core.Y, core.X, target.Height - (fullSize.Y - core.Height)),
                color,
                sort,
                blend
                );

            // Center
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X, core.Y, core.Width, core.Height),
                target: new Rectangle(target.Left + core.X, target.Top + core.Y, target.Width - (fullSize.X - core.Width), target.Height - (fullSize.Y - core.Height)),
                color,
                sort,
                blend
                );

            // Right
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X + core.Width, core.Y, bottomRightSize.Width, core.Height),
                target: new Rectangle(target.Right - bottomRightSize.Width, target.Top + core.Y, bottomRightSize.Width, target.Height - (fullSize.Y - core.Height)),
                color,
                sort,
                blend
                );

            // BottomLeft
            texture.Draw(
                batch,
                clip: new IntRectangle(0, fullSize.Y - bottomRightSize.Y, core.X, bottomRightSize.Y),
                target: new Rectangle(target.Left, target.Bottom - bottomRightSize.Y, core.X, bottomRightSize.Y),
                color,
                sort,
                blend
                );

            // Bottom
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X, fullSize.Y - bottomRightSize.Y, core.Width, bottomRightSize.Y),
                target: new Rectangle(target.Left + core.X, target.Bottom - bottomRightSize.Y, target.Width - (fullSize.X - core.Width), bottomRightSize.Y),
                color,
                sort,
                blend
                );

            // BottomRight
            texture.Draw(
                batch,
                clip: new IntRectangle(fullSize.X - bottomRightSize.X, fullSize.Y - bottomRightSize.Y, bottomRightSize.X, bottomRightSize.Y),
                target: new Rectangle(target.Right - bottomRightSize.X, target.Bottom - bottomRightSize.Y, bottomRightSize.X, bottomRightSize.Y),
                color,
                sort,
                blend
                );
        }

        public static void DrawRepeating
            (
            Batch2D batch,
            AtlasCoordinates texture,
            Rectangle area,
            float sort)
        {
            for (int x = Calculator.RoundToInt(area.X); x < area.Right; x += texture.Size.X) 
            {
                for (int y = Calculator.RoundToInt(area.Y); y < area.Bottom; y += texture.Size.Y) 
                {
                    Vector2 excess = new(MathF.Max(0, x + texture.Size.X - area.Right), MathF.Max(0, y + texture.Size.Y - area.Bottom));
                    texture.Draw(batch, new Vector2(x, y), new Rectangle(Vector2.Zero, texture.Size - excess), Color.White, Vector2.One, 0, Vector2.Zero, ImageFlip.None, RenderServices.BLEND_NORMAL, sort);
                }
            }
        }

        /// <summary>
        /// The Renders a sprite on the screen. This is the most basic rendering method with all paramethers exposed, avoid using this if possible.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
        /// <param name="pos">Position in the render.</param>
        /// <param name="clip">Cliping rectangle. Rectangle.Empty for the whole sprite</param>
        /// <param name="animationId">Animation string id.</param>
        /// <param name="ase">Aseprite asset.</param>
        /// <param name="animationStartedTime">When the animation started.</param>
        /// <param name="animationDuration">The total duration of the animation. Use -1 to use the duration from the aseprite file.</param>
        /// <param name="origin">Offset from <paramref name="pos"/>. From 0 to 1.</param>
        /// <param name="flipped">Whether the image is flipped.</param>
        /// <param name="rotation">Rotation of the image, in radians.</param>
        /// <param name="scale">Scale applied to the sprite.</param>
        /// <param name="color">Color.</param>
        /// <param name="blend">Blend.</param>
        /// <param name="sort">Sort layer. 0 is in front, 1 is behind</param>
        /// <param name="useScaledTime">If true, this will use the scaled time and will pause whenever the game is paused.</param>
        /// <param name="loopAnimation"></param>
        /// <returns>If the animation is complete or not</returns>
        public static FrameInfo DrawSprite(
            Batch2D spriteBatch,
            Vector2 pos,
            Rectangle clip,
            string animationId,
            SpriteAsset ase,
            float animationStartedTime,
            float animationDuration,
            Vector2 origin,
            bool flipped,
            float rotation,
            Vector2 scale,
            Color color,
            Vector3 blend,
            float sort,
            bool useScaledTime = true,
            bool loopAnimation = true
            )
        {
            ImageFlip imageFlip = flipped ? ImageFlip.Horizontal : ImageFlip.None;

            if (animationId != null)
            {
                if (!ase.Animations.TryGetValue(animationId, out var animation))
                {
                    GameLogger.Log($"Couldn't find animation {animationId}.");
                    return FrameInfo.Fail;
                }

                float time = (useScaledTime ? Game.Now : Game.NowUnescaled) - animationStartedTime;
                var anim = animation.Evaluate(loopAnimation ? time : Calculator.Clamp01(time), animationDuration);

                var image = ase.GetFrame(anim.Frame);
                Vector2 offset = ase.Origin + origin * image.Size;
                Vector2 position = Vector2.Round(pos);

                image.Draw(
                    spriteBatch: spriteBatch,
                    position: position,
                    clip: clip,
                    color: color,
                    offset: offset,
                    scale: scale,
                    rotation: rotation,
                    imageFlip: imageFlip,
                    blend: blend,
                    sort: sort);
                    
                
                return anim;
            }

            return FrameInfo.Fail;
        }
        
        public static void MessageCompleteAnimations(Entity e, SpriteComponent s)
        {
            if (s.NextAnimations.Length > 1)
            {
                if (!string.IsNullOrWhiteSpace(s.CurrentAnimation))
                    e.PlayAsepriteAnimation(s.NextAnimations.RemoveAt(0));

                e.SendMessage(new AnimationCompleteMessage());
                e.RemoveAnimationComplete();
            }
            else
            {
                if (!e.HasAnimationComplete())
                {
                    e.SetAnimationComplete();
                    e.SendMessage(new AnimationCompleteMessage());
                }
            }
        }

        public static void MessageCompleteAnimations(Entity e)
        {
            if (e.HasAnimationOverload())
            {
                e.RemoveAnimationOverload();

                e.SetAnimationComplete();
                e.SendMessage(new AnimationCompleteMessage());
            }

            if (!e.HasAnimationComplete())
            {
                e.SetAnimationComplete();
                e.SendMessage(new AnimationCompleteMessage());
            }
        }

        public static (SpriteAsset asset, string animation)? FetchPortraitAsSprite(Portrait portrait)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(portrait.Aseprite) is SpriteAsset aseprite)
            {
                return (aseprite, portrait.AnimationId);
            }

            return null;
        }
        
        public static FrameInfo DrawSprite(Batch2D batch, Guid assetGuid, Vector2 position, DrawInfo drawInfo) => DrawSprite(batch, assetGuid, position, drawInfo, AnimationInfo.Default);
        public static FrameInfo DrawSprite(Batch2D batch, SpriteAsset assetGuid, Vector2 position, DrawInfo drawInfo) => DrawSprite(batch, assetGuid, position, drawInfo, AnimationInfo.Default);

        public static FrameInfo DrawSprite(Batch2D batch, Guid assetGuid, Vector2 position, DrawInfo drawInfo, AnimationInfo animationInfo)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(assetGuid) is SpriteAsset asset)
            {
                return DrawSprite(batch, asset, position, drawInfo, animationInfo);
            }
            return FrameInfo.Fail;
        }
        public static FrameInfo DrawSprite(Batch2D batch, Guid assetGuid, float x, float y, DrawInfo drawInfo, AnimationInfo animationInfo)
        {
            return DrawSprite(batch, assetGuid, new Vector2(x, y), drawInfo, animationInfo);
        }
        public static FrameInfo DrawSprite(Batch2D batch, SpriteAsset asset, Vector2 position, DrawInfo drawInfo, AnimationInfo animationInfo)
        {
            FrameInfo drawAt(Vector2 position, Color color, bool wash, float sort)
            {
                return DrawSprite(
                batch,
                position,
                drawInfo.Clip,
                animationInfo.Name,
                asset,
                animationInfo.Start,
                animationInfo.Duration,
                drawInfo.Origin,
                drawInfo.FlippedHorizontal,
                drawInfo.Rotation,
                drawInfo.Scale,
                color,
                wash ? RenderServices.BLEND_WASH : drawInfo.GetBlendMode(),
                sort,
                animationInfo.UseScaledTime,
                animationInfo.Loop);
            }

            if (drawInfo.Outline.HasValue)
            {
                drawAt(position + new Vector2(0, 1), drawInfo.Outline.Value, true, drawInfo.Sort + 0.0001f);
                drawAt(position + new Vector2(0, -1), drawInfo.Outline.Value, true, drawInfo.Sort + 0.0001f);
                drawAt(position + new Vector2(1, 0), drawInfo.Outline.Value, true, drawInfo.Sort + 0.0001f);
                drawAt(position + new Vector2(-1, 0), drawInfo.Outline.Value, true, drawInfo.Sort + 0.0001f);

            }

            if (drawInfo.Shadow.HasValue)
            {
                drawAt(position + new Vector2(0, 1), drawInfo.Shadow.Value, true, drawInfo.Sort + 0.0002f);

            }

            return drawAt(position, drawInfo.Color, false, drawInfo.Sort);
        }

        /// <summary>
        /// Draws a list of connecting points
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// /// <param name="position">Where to position the points</param>
        /// <param name="points">The points to connect with lines</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the lines</param>
        public static void DrawPoints(Batch2D spriteBatch, Vector2 position, Vector2[] points, Color color, float thickness)
        {
            if (points.Length < 2)
                return;

            for (int i = 1; i < points.Length; i++)
            {
                DrawLine(spriteBatch, points[i - 1] + position, points[i] + position, color, thickness);
            }
            DrawLine(spriteBatch, points[points.Length - 1] + position, points[0] + position, color, thickness);
        }

        /// <summary>
		/// Draws a list of connecting points
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// /// <param name="position">Where to position the points</param>
		/// <param name="points">The points to connect with lines</param>
		/// <param name="color">The color to use</param>
		/// <param name="thickness">The thickness of the lines</param>
		public static void DrawPoints(Batch2D spriteBatch, Vector2 position, ReadOnlySpan<Vector2> points, Color color, float thickness)
        {
            if (points.Length < 2)
                return;

            for (int i = 1; i < points.Length; i++)
            {
                DrawLine(spriteBatch, points[i - 1] + position, points[i] + position, color, thickness);
            }
            DrawLine(spriteBatch, points[points.Length - 1] + position, points[0] + position, color, thickness);
        }

        public static void DrawRectangleOutline(this Batch2D spriteBatch, Rectangle rectangle, Color color) =>
            DrawRectangleOutline(spriteBatch, rectangle, color, 1, 0);
        public static void DrawRectangleOutline(this Batch2D spriteBatch, Rectangle rectangle, Color color, int lineWidth) =>
            DrawRectangleOutline(spriteBatch, rectangle, color, lineWidth, 0);

        public static void DrawRectangleOutline(this Batch2D spriteBatch, Rectangle rectangle, Color color, int lineWidth, float sorting)
        {
            DrawRectangle(spriteBatch, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth - 1), color, sorting);
            DrawRectangle(spriteBatch, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth - 1, lineWidth), color, sorting);
            DrawRectangle(spriteBatch, new Rectangle(rectangle.X + rectangle.Width - 1, rectangle.Y, lineWidth, rectangle.Height + lineWidth - 1), color, sorting);
            DrawRectangle(spriteBatch, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - 1, rectangle.Width + lineWidth  - 1, lineWidth), color, sorting);
        }

        public static float YSort(float y)
        {
            // TODO: Solve a better ySort that takes in consideration the camera position
            return (Y_SORT_RANGE - y) / Y_SORT_RANGE_X2;
        }

        public static void DrawHorizontalLine(this Batch2D spriteBatch, int x, int y, int length, Color color, float sorting = 0)
        {
            const int line = 1;

            DrawRectangle(spriteBatch, new Rectangle(x, y, length, line), color, sorting);
        }

        public static void DrawVerticalLine(this Batch2D spriteBatch, int x, int y, int length, Color color, float sorting = 0)
        {
            const int line = 1;

            DrawRectangle(spriteBatch, new Rectangle(x, y, line, length), color, sorting);
        }

        public static void DrawRectangle(this Batch2D batch, Rectangle rectangle, Color color, float sorting = 0)
        {
            batch.Draw(
                texture: SharedResources.GetOrCreatePixel(batch),
                position: rectangle.TopLeft,
                targetSize: Point.One,
                sourceRectangle: default,
                sort: sorting,
                rotation: 0,
                scale: rectangle.Size,
                flip: ImageFlip.None,
                color: color,
                offset: Vector2.Zero,
                BLEND_COLOR_ONLY
                );
        }

        #region Lines
        public static void DrawLine(this Batch2D spriteBatch, Point point1, Point point2, Color color, float sort = 1f) =>
            DrawLine(spriteBatch, point1.ToVector2(), point2.ToVector2(), color, 1.0f, sort);

        public static void DrawLine(this Batch2D spriteBatch, Vector2 point1, Vector2 point2, Color color, float sort = 1f) =>
            DrawLine(spriteBatch, point1, point2, color, 1.0f, sort);

        public static void DrawLine(this Batch2D spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness, float sort = 1f)
        {
            // calculate the distance between the two vectors
            float distance = Vector2.Distance(point1, point2);

            // calculate the angle between the two vectors
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(spriteBatch, point1, distance, angle, color, thickness, sort);
        }

        public static void DrawLine(this Batch2D spriteBatch, Vector2 point, float length, float angle, Color color, float sort = 1f) =>
            DrawLine(spriteBatch, point, length, angle, color, 1f, sort);


        public static void DrawLine(this Batch2D spriteBatch, Vector2 point, float length, float angle, Color color, float thickness, float sort = 1f)
        {
            // stretch the pixel between the two vectors
            spriteBatch.Draw(SharedResources.GetOrCreatePixel(spriteBatch),
                             point,
                             Vector2.One,
                             default,
                             sort,
                             angle,
                             new Vector2(length, thickness),
                             ImageFlip.None,
                             color,
                             Vector2.Zero,
                             BLEND_NORMAL
                             );
        }
        #endregion

        #region Circle and Arcs
        public static void DrawCircle(this Batch2D spriteBatch, Point center, float radius, int sides, Color color, float sort = 1f) =>
            DrawCircle(spriteBatch, center.ToVector2(), radius, sides, color, sort);


        /// <summary>
		/// Draw a circle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="center">The center of the circle</param>
		/// <param name="radius">The radius of the circle</param>
		/// <param name="sides">The number of sides to generate</param>
		/// <param name="color">The color of the circle</param>
		/// <param name="sort">The sorting value</param>
		public static void DrawCircle(this Batch2D spriteBatch, Vector2 center, float radius, int sides, Color color, float sort = 1f)
        {
            DrawPoints(spriteBatch, center, GeometryServices.CreateCircle(radius, sides), color, sort);
        }

        public static void DrawFlatenedCircle(this Batch2D spriteBatch, Vector2 center, float radius, float scaleY, int sides, Color color)
        {
            DrawPoints(spriteBatch, center, GeometryServices.CreateOrGetFlatenedCircle(radius, scaleY, sides), color, 1.0f);
        }

        public static void DrawFlatenedCircle(this Batch2D spriteBatch, Vector2 center, float radius, float scaleY, int sides, Color color, float sort)
        {
            DrawPoints(spriteBatch, center, GeometryServices.CreateOrGetFlatenedCircle(radius, scaleY, sides), color, sort);
        }
        
        

        public static void DrawPoint(Batch2D spriteBatch, Point pos, Color color, float sorting = 0)
        {
            DrawRectangle(spriteBatch, new Rectangle(pos, Point.One), color, sorting);
        }
        #endregion

        #region Drawing

        public static Microsoft.Xna.Framework.Vector3 BLEND_NORMAL = new(1, 0, 0);
        public static Microsoft.Xna.Framework.Vector3 BLEND_WASH = new(0, 1, 0);
        public static Microsoft.Xna.Framework.Vector3 BLEND_COLOR_ONLY = new(0, 0, 1);

        public static void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, Effect effect, BlendState blend, bool smoothing)
        {
            (VertexInfo[] verts, short[] indices) = MakeTexturedQuad(destination, source, new Vector2(texture.Width, texture.Height), color, BLEND_NORMAL);
            Game.GraphicsDevice.SamplerStates[0] = smoothing ? SamplerState.PointClamp : SamplerState.AnisotropicClamp;
            DrawIndexedVertices(matrix, Game.GraphicsDevice, verts, verts.Length, indices, indices.Length / 3, effect, blend, texture);
        }

        public static void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, BlendState blend)
        {
            (VertexInfo[] verts, short[] indices) = MakeTexturedQuad(destination, source, new Vector2(texture.Width, texture.Height), color, BLEND_NORMAL);

            if (blend == BlendState.Additive)
                Game.Data.ShaderSprite.SetTechnique("Add");
            else
                Game.Data.ShaderSprite.SetTechnique("Alpha");

            DrawIndexedVertices(
                matrix,
                Game.GraphicsDevice, verts, verts.Length, indices, indices.Length / 3, Game.Data.ShaderSprite,
                blend,
                texture);
        }

        public static void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, BlendState blend, Effect shaderEffect)
        {
            (VertexInfo[] verts, short[] indices) = MakeTexturedQuad(destination, source, new Vector2(texture.Width, texture.Height), color, BLEND_NORMAL);

            DrawIndexedVertices(
                matrix,
                Game.GraphicsDevice, verts, verts.Length, indices, indices.Length / 3, shaderEffect,
                blend,
                texture);
        }

        public static void DrawQuadOutline(Rectangle rect, Color color)
        {
            DrawQuad(new Rectangle(rect.TopLeft, new Vector2(rect.Width, 1)), color);
            DrawQuad(new Rectangle(rect.TopLeft, new Vector2(1, rect.Height)), color);
            DrawQuad(new Rectangle(rect.BottomLeft + new Vector2(0, -1), new Vector2(rect.Width, 1)), color);
            DrawQuad(new Rectangle(rect.TopRight + new Vector2(-1, 0), new Vector2(1, rect.Height)), color);
        }

        public static void DrawQuad(Rectangle rect, Color color)
        {
            (VertexInfo[] verts, short[] indices) = MakeRegularQuad(rect, color, BLEND_COLOR_ONLY);

            Game.Data.ShaderSprite.SetTechnique("Alpha");
            DrawIndexedVertices(
                Microsoft.Xna.Framework.Matrix.CreateTranslation(Microsoft.Xna.Framework.Vector3.Zero),
                Game.GraphicsDevice, verts, verts.Length, indices, indices.Length/3, Game.Data.ShaderSprite,
                BlendState.AlphaBlend,
                null);
        }

        static readonly VertexInfo[] _cachedVertices = new VertexInfo[4];
        static readonly short[] _cachedIndices = new short[6];

        static RenderServices()
        {
            for (int i = 0; i < 4; i++)
            {
                _cachedVertices[i] = new VertexInfo();
            }
        }

        private static (VertexInfo[] vertices, short[] indices) MakeQuad(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color, Vector3 BlendType)
        {
            // 0---1
            // |\  |
            // | \ |
            // |  \|
            // 3---2

            _cachedVertices[0].Position = p1.ToVector3();
            _cachedVertices[0].Color = color;
            _cachedVertices[0].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 0);
            _cachedVertices[0].BlendType = BlendType;

            _cachedVertices[1].Position = p2.ToVector3();
            _cachedVertices[1].Color = color;
            _cachedVertices[1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 0);
            _cachedVertices[1].BlendType = BlendType;

            _cachedVertices[2].Position = p3.ToVector3();
            _cachedVertices[2].Color = color;
            _cachedVertices[2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 1);
            _cachedVertices[2].BlendType = BlendType;

            _cachedVertices[3].Position = p4.ToVector3();
            _cachedVertices[3].Color = color;
            _cachedVertices[3].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 1);
            _cachedVertices[3].BlendType = BlendType;

            _cachedIndices[0] = 0;
            _cachedIndices[1] = 1;
            _cachedIndices[2] = 2;
            _cachedIndices[3] = 0;
            _cachedIndices[4] = 2;
            _cachedIndices[5] = 3;


            return (_cachedVertices, _cachedIndices);
        }

        private static (VertexInfo[] vertices,short[] indices) MakeRegularQuad(Rectangle rect, Color color, Vector3 BlendType)
        {
            // 0---1
            // |\  |
            // | \ |
            // |  \|
            // 3---2

            _cachedVertices[0].Position = rect.TopLeft.ToVector3();
            _cachedVertices[0].Color = color;
            _cachedVertices[0].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 0);
            _cachedVertices[0].BlendType = BlendType;

            _cachedVertices[1].Position = rect.TopRight.ToVector3();
            _cachedVertices[1].Color = color;
            _cachedVertices[1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 0);
            _cachedVertices[1].BlendType = BlendType;

            _cachedVertices[2].Position = rect.BottomRight.ToVector3();
            _cachedVertices[2].Color = color;
            _cachedVertices[2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 1);
            _cachedVertices[2].BlendType = BlendType;

            _cachedVertices[3].Position = rect.BottomLeft.ToVector3();
            _cachedVertices[3].Color = color;
            _cachedVertices[3].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 1);
            _cachedVertices[3].BlendType = BlendType;

            _cachedIndices[0] = 0;
            _cachedIndices[1] = 1;
            _cachedIndices[2] = 2;
            _cachedIndices[3] = 0;
            _cachedIndices[4] = 2;
            _cachedIndices[5] = 3;


            return (_cachedVertices, _cachedIndices);
        }

        private static (VertexInfo[] vertices, short[] indices) MakeTexturedQuad(Rectangle destination, Rectangle source, Vector2 sourceSize, Color color, Vector3 BlendType)
        {
            // 0---1
            // |\  |
            // | \ |
            // |  \|
            // 3---2

            Vector2 uvTopLeft = new(source.X / sourceSize.X, source.Y / sourceSize.Y);
            Vector2 uvTopRight = new((source.X + source.Width) / sourceSize.X, source.Y / sourceSize.Y);
            Vector2 uvBottomRight = new((source.X + source.Width) / sourceSize.X, (source.Y + source.Height) / sourceSize.Y);
            Vector2 uvBottomLeft = new(source.X / sourceSize.X, (source.Y + source.Height) / sourceSize.Y);

            _cachedVertices[0].Position = destination.TopLeft.ToVector3();
            _cachedVertices[0].Color = color;
            _cachedVertices[0].TextureCoordinate = uvTopLeft;
            _cachedVertices[0].BlendType = BlendType;

            _cachedVertices[1].Position = destination.TopRight.ToVector3();
            _cachedVertices[1].Color = color;
            _cachedVertices[1].TextureCoordinate = uvTopRight;
            _cachedVertices[1].BlendType = BlendType;

            _cachedVertices[2].Position = destination.BottomRight.ToVector3();
            _cachedVertices[2].Color = color;
            _cachedVertices[2].TextureCoordinate = uvBottomRight;
            _cachedVertices[2].BlendType = BlendType;

            _cachedVertices[3].Position = destination.BottomLeft.ToVector3();
            _cachedVertices[3].Color = color;
            _cachedVertices[3].TextureCoordinate = uvBottomLeft;
            _cachedVertices[3].BlendType = BlendType;

            _cachedIndices[0] = 0;
            _cachedIndices[1] = 1;
            _cachedIndices[2] = 2;
            _cachedIndices[3] = 0;
            _cachedIndices[4] = 2;
            _cachedIndices[5] = 3;


            return (_cachedVertices, _cachedIndices);
        }

        public static void DrawIndexedVertices<T>(Matrix matrix, GraphicsDevice graphicsDevice, T[] vertices, int vertexCount, short[] indices, int primitiveCount, Effect effect, BlendState? blendState = null, Texture2D? texture = null) where T : struct, IVertexType
        {
            var b = blendState ?? BlendState.AlphaBlend;
            
            var size = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            matrix *= Matrix.CreateScale((1f / size.X) * 2, -(1f / size.Y) * 2, 1f); // scale to relative points
            matrix *= Matrix.CreateTranslation(-1f, 1f, 0); // translate to relative points

            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = b;
            Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            effect.Parameters["MatrixTransform"].SetValue(matrix);

            if (texture != null)
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.Textures[0] = texture;
                    graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertexCount, indices, 0, primitiveCount);
                }
            }
            else // Saving that 1 check for performance
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertexCount, indices, 0, primitiveCount);
                }
            }
        }
        public static void DrawPolygon(Batch2D batch, ImmutableArray<Vector2> vertices, DrawInfo drawInfo)
        {
            batch.DrawPolygon(SharedResources.GetOrCreatePixel(batch), vertices, drawInfo);
        }

        public static void DrawFilledCircle(Batch2D batch, Vector2 center, float radius, int steps, DrawInfo drawInfo)
        {
            Vector2[] circleVertices = GeometryServices.CreateOrGetFlatenedCircle(1f, 1f, steps);

            // Scale and translate the vertices
            var scaledTranslatedVertices = circleVertices.Select(p => new Vector2(p.X * radius + center.X, p.Y * radius + center.Y)).ToArray();

            batch.DrawPolygon(SharedResources.GetOrCreatePixel(batch), scaledTranslatedVertices, drawInfo);
        }

        public static void DrawFilledCircle(Batch2D batch, Rectangle circleRect, int steps, DrawInfo drawInfo)
        {
            Vector2[] circleVertices = GeometryServices.CreateOrGetFlatenedCircle(1f, 1f, steps);
            
            // Scale and translate the vertices
            batch.DrawPolygon(SharedResources.GetOrCreatePixel(batch), circleVertices, drawInfo.WithScale(circleRect.Size).WithOffset(circleRect.Center));
        }


        public static Point DrawText(Batch2D uiBatch, int font, string text, Vector2 position, DrawInfo drawInfo)
            => DrawText(uiBatch, font, text, position, -1, -1, drawInfo);
        public static Point DrawText(Batch2D uiBatch, int font, string text, Vector2 position, int maxWidth, DrawInfo drawInfo)
            => DrawText(uiBatch, font, text, position, maxWidth, -1, drawInfo);
        public static Point DrawText(Batch2D uiBatch, MurderFonts font, string text, Vector2 position, DrawInfo drawInfo)
            => DrawText(uiBatch, (int)font, text, position, -1, -1, drawInfo);
        public static Point DrawText(Batch2D uiBatch, MurderFonts font, string text, Vector2 position, int maxWidth, DrawInfo drawInfo)
            => DrawText(uiBatch, (int)font, text, position, maxWidth, -1, drawInfo);
        public static Point DrawText(Batch2D uiBatch, MurderFonts font, string text, Vector2 position, int maxWidth, int visibleCharacters, DrawInfo drawInfo)
            => DrawText(uiBatch, (int)font, text, position, maxWidth, visibleCharacters, drawInfo);

        public static Point DrawText(Batch2D uiBatch, int pixelFont, string text, Vector2 position, int maxWidth, int visibleCharacters, DrawInfo drawInfo)
        {
            var font = Game.Data.GetFont((int)pixelFont);
            return font.Draw(uiBatch, text, position + drawInfo.Origin, drawInfo.Origin, drawInfo.Sort, drawInfo.Color, drawInfo.Outline, drawInfo.Shadow, maxWidth, visibleCharacters, drawInfo.Debug);
        }
        
        #endregion

    }
}