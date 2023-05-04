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
using static System.Net.Mime.MediaTypeNames;
using Murder.Services.Info;
using Murder.Core.Input;

namespace Murder.Services
{
    /// Some of the code based on https://github.com/z2oh/C3.MonoGame.Primitives2D/blob/master/Primitives2D.cs
    public static class RenderServices
    {
        const int Y_SORT_RANGE = 10000;
        const int Y_SORT_RANGE_X2 = Y_SORT_RANGE * 2;
        
        /// <summary>
        /// TODO: Pass around a "style" for background color, sounds, etc.
        /// </summary>
        public static DrawMenuInfo DrawVerticalMenu(
            in RenderContext render,
            in Point position,
            in DrawMenuStyle style,
            in MenuInfo menuInfo)
        {

            int lineHeight = style.Font.LineHeight + 2;
            Point finalPosition = new Point(Math.Max(position.X, 0), Math.Max(position.Y, 0));

            Vector2 CalculateSelector(int index) => new Point(0, lineHeight * (index + 1)) + finalPosition;
            
            for (int i = 0; i < menuInfo.Length; i++)
            {
                var label = menuInfo.GetOptionText(i);
                var labelPosition = CalculateSelector(i);

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

                style.Font.Draw(render.UiBatch, label ?? string.Empty, 1, labelPosition, style.Origin, 0.1f,
                    currentColor, null, currentShadow);
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
                SelectorEasedPosition = easedPosition
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
                    sort,
                    RenderServices.BLEND_NORMAL
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
                    sort,
                    RenderServices.BLEND_NORMAL
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
                    sort,
                    RenderServices.BLEND_NORMAL
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
                    sort,
                    RenderServices.BLEND_NORMAL
                );
            }
            
            //batch.DrawRectangleOutline(new IntRectangle(position.X - size.X * origin.X, position.Y - size.Y * origin.Y, size.X, size.Y), Color.Red);
        }

        /// <summary>
        /// Draws a 9-slice using the given texture and target rectangle. The core rectangle is specified in the Aseprite file
        /// </summary>
        public static void Draw9Slice(Batch2D batch, Guid guid, Rectangle target, string animation, DrawInfo info)
        {
            var asset = Game.Data.GetAsset<SpriteAsset>(guid);
            if (asset.Animations.ContainsKey(animation))
            {
                var frame = asset.Animations[animation].Evaluate(0, info.UseScaledTime ? Game.Now : Game.NowUnescaled);
                RenderServices.Draw9Slice(batch, asset.GetFrame(frame.animationFrame), target, asset.NineSlice, info);
            }
            else
            {
                GameLogger.Log($"animation {animation} doesn't exist for aseprite {asset.Name}.");
            }
        }
        public static void Draw9Slice(Batch2D batch, Guid guid, Rectangle target, DrawInfo info)
        {
            var asset = Game.Data.GetAsset<SpriteAsset>(guid);
            var frame = asset.Animations.FirstOrDefault().Value.Evaluate(0, info.UseScaledTime ? Game.Now : Game.NowUnescaled);
            RenderServices.Draw9Slice(batch, asset.GetFrame(frame.animationFrame), target, asset.NineSlice, info);
        }

        public static void Draw9Slice(Batch2D batch, AtlasCoordinates texture, Rectangle target, Rectangle core, DrawInfo info) =>
            Draw9Slice(batch, texture, core, target, info.Color, info.Sort);
        public static void Draw9Slice(Batch2D batch, AtlasCoordinates texture, Rectangle core, Rectangle target, float sort) =>
            Draw9Slice(batch, texture, core, target, Color.White, sort);
        public static void Draw9Slice(
        Batch2D batch,
        AtlasCoordinates texture,
        Rectangle core,
        Rectangle target,
        Color color,
        float sort)
        {
            var fullSize = texture.Size;
            var bottomRightSize = new Vector2(fullSize.X - core.X - core.Width, fullSize.Y - core.Y - core.Height);

            // TopLeft
            texture.Draw(
                batch,
                clip: new IntRectangle(0, 0, core.X, core.Y),
                target: new Rectangle(target.Left, target.Top, core.X, core.Y),
                color,
                sort,
                RenderServices.BLEND_NORMAL
                );

            // Top
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X, 0, core.Width, core.Y),
                target: new Rectangle(target.Left + core.X, target.Top, target.Width - (fullSize.X - core.Width), core.Y),
                color,
                sort,
                RenderServices.BLEND_NORMAL
                );

            // TopRight
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X + core.Width, 0, bottomRightSize.Width, core.Y),
                target: new Rectangle(target.Right - bottomRightSize.Width, target.Top, bottomRightSize.Width, core.Y),
                color,
                sort,
                RenderServices.BLEND_NORMAL
                );

            // Left
            texture.Draw(
                batch,
                clip: new IntRectangle(0, core.Y, core.X, core.Height),
                target: new Rectangle(target.Left, target.Top + core.Y, core.X, target.Height - (fullSize.Y - core.Height)),
                color,
                sort,
                RenderServices.BLEND_NORMAL
                );

            // Center
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X , core.Y, core.Width, core.Height),
                target: new Rectangle(target.Left + core.X, target.Top + core.Y, target.Width - (fullSize.X - core.Width), target.Height - (fullSize.Y - core.Height)),
                color,
                sort,
                RenderServices.BLEND_NORMAL
                );

            // Right
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X + core.Width, core.Y, bottomRightSize.Width, core.Height),
                target: new Rectangle(target.Right - bottomRightSize.Width, target.Top + core.Y, bottomRightSize.Width, target.Height - (fullSize.Y - core.Height)),
                color,
                sort,
                RenderServices.BLEND_NORMAL
                );

            // BottomLeft
            texture.Draw(
                batch,
                clip: new IntRectangle(0, fullSize.Y - bottomRightSize.Y, core.X, bottomRightSize.Y),
                target: new Rectangle(target.Left, target.Bottom - bottomRightSize.Y, core.X, bottomRightSize.Y),
                color,
                sort,
                RenderServices.BLEND_NORMAL
                );

            // Bottom
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X, fullSize.Y - bottomRightSize.Y, core.Width, bottomRightSize.Y),
                target: new Rectangle(target.Left + core.X, target.Bottom - bottomRightSize.Y, target.Width - (fullSize.X -core.Width), bottomRightSize.Y),
                color,
                sort,
                RenderServices.BLEND_NORMAL
                );

            // BottomRight
            texture.Draw(
                batch,
                clip: new IntRectangle(fullSize.X - bottomRightSize.X, fullSize.Y - bottomRightSize.Y, bottomRightSize.X, bottomRightSize.Y),
                target: new Rectangle(target.Right - bottomRightSize.X, target.Bottom - bottomRightSize.Y, bottomRightSize.X, bottomRightSize.Y),
                color,
                sort,
                RenderServices.BLEND_NORMAL
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
                    texture.Draw(batch, new Vector2(x, y), new Rectangle(Vector2.Zero, texture.Size - excess), Color.White, sort, RenderServices.BLEND_NORMAL);
                }
            }
        }

        /// <summary>
        /// Renders a sprite on the screen
        /// </summary>
        /// <param name="spriteBatch">Sprite batch.</param>
        /// <param name="pos">Position in the render.</param>
        /// <param name="animationId">Animation string id.</param>
        /// <param name="ase">Aseprite asset.</param>
        /// <param name="animationStartedTime">When the animation started.</param>
        /// <param name="animationDuration">The total duration of the animation. Use -1 to use the duration from the aseprite file.</param>
        /// <param name="offset">Offset from <paramref name="pos"/>. From 0 to 1.</param>
        /// <param name="flipped">Whether the image is flipped.</param>
        /// <param name="rotation">Rotation of the image, in radians.</param>
        /// <param name="scale">Scale applied to the sprite.</param>
        /// <param name="color">Color.</param>
        /// <param name="blend">Blend.</param>
        /// <param name="sort">Sort layer. 0 is in front, 1 is behind</param>
        /// <param name="useScaledTime">If true, this will use the escaled time and will pause whenever the game is paused.</param>
        /// <returns>If the animation is complete or not</returns>
        public static bool DrawSprite(
            Batch2D spriteBatch,
            Vector2 pos,
            string animationId,
            SpriteAsset ase,
            float animationStartedTime,
            float animationDuration,
            Vector2 offset,
            bool flipped,
            float rotation,
            Vector2 scale,
            Color color,
            Vector3 blend,
            float sort,
            bool useScaledTime = true)
        {
            ImageFlip spriteEffects = flipped ? ImageFlip.Horizontal : ImageFlip.None;

            if (animationId != null)
            {
                if (!ase.Animations.TryGetValue(animationId, out var animation))
                {
                    GameLogger.Log($"Couldn't find animation {animationId}.");
                    return false;
                }

                var (frame, complete) = animation.Evaluate(animationStartedTime, useScaledTime? Game.Now : Game.NowUnescaled, animationDuration);

                var image = ase.GetFrame(frame);
                Vector2 imageOffset = ase.Origin.ToVector2() + new Vector2(image.Size.X * offset.X, image.Size.Y * offset.Y);
                Vector2 position = Vector2.Round(pos);

                image.Draw(
                    spriteBatch,
                    position,
                    scale,
                    imageOffset.Point,
                    rotation, 
                    spriteEffects, 
                    color, 
                    blend, 
                    sort);
                
                return complete;
            }

            return false;
        }

        /// <summary>
        /// Renders a sprite on the screen
        /// </summary>
        /// <param name="spriteBatch">Target sprite batch.</param>
        /// <param name="pos">Position of the animation.</param>
        /// <param name="animationId">Animation unique identifier.</param>
        /// <param name="ase">Target aseprite asset.</param>
        /// <param name="animationStartedTime">When the animation has started within the game.</param>
        /// <param name="animationDuration">The total duration of the animation. Use -1 to use the duration from the aseprite file.</param>
        /// <param name="offset">Offset from <paramref name="pos"/>.</param>
        /// <param name="flipped">Whether the animation is flipped.</param>
        /// <param name="rotation">Rotation of the sprite.</param>
        /// <param name="color">Color to apply in the sprite.</param>
        /// <param name="blend">The blend style to be used by the shader. Use the constants in <see cref="RenderServices"/>.</param>
        /// <param name="sort">Sorting order when displaying the sprite.</param>
        /// <param name="useScaledTime">If true, this will use the escaled time and will pause whenever the game is paused.</param>
        /// <returns>If the animation is complete or not</returns>
        public static bool DrawSpriteWithOutline(
            Batch2D spriteBatch,
            Vector2 pos,
            string animationId,
            SpriteAsset ase,
            float animationStartedTime,
            float animationDuration,
            Vector2 offset,
            bool flipped,
            float rotation,
            Color color,
            Vector3 blend,
            float sort,
            bool useScaledTime = true)
        {
            ImageFlip spriteEffects = flipped ? ImageFlip.Horizontal : ImageFlip.None;

            if (animationId != null)
            {
                if (!ase.Animations.TryGetValue(animationId, out var animation))
                {
                    return false;
                }

                var (frame, complete) = animation.Evaluate(animationStartedTime, useScaledTime ? Game.Now : Game.NowUnescaled, animationDuration);
                var image = ase.GetFrame(frame);

                Vector2 imageOffset = (ase.Origin.ToVector2() + new Vector2(image.Size.X * offset.X, image.Size.Y * offset.Y)).Point;
                Vector2 position = Vector2.Round(pos);
                
                var sortOffset = -0.0001f;
                image.Draw(spriteBatch, position + new Vector2(0, 1), Vector2.One, imageOffset, rotation, spriteEffects,
                    Color.White * 0.8f, RenderServices.BLEND_WASH, sort - sortOffset);
                image.Draw(spriteBatch, position + new Vector2(0, -1), Vector2.One, imageOffset, rotation, spriteEffects,
                    Color.White * 0.8f, RenderServices.BLEND_WASH, sort - sortOffset);
                image.Draw(spriteBatch, position + new Vector2(1, 0), Vector2.One, imageOffset, rotation, spriteEffects,
                    Color.White * 0.8f, RenderServices.BLEND_WASH, sort - sortOffset);
                image.Draw(spriteBatch, position + new Vector2(-1, 0), Vector2.One, imageOffset, rotation, spriteEffects,
                    Color.White * 0.8f, RenderServices.BLEND_WASH, sort - sortOffset);

                image.Draw(spriteBatch, position, Vector2.One, imageOffset, rotation,spriteEffects, color, blend, sort);
                
                return complete;
            }

            return false;
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

        public static bool DrawSprite(
            Batch2D spriteBatch,
            Vector2 pos,
            float rotation,
            string animationId,
            SpriteAsset ase,
            float animationStartedTime,
            Color color,
            Vector3 blend,
            float sort = 1,
            bool useScaledTime = true)
            => DrawSprite(spriteBatch, pos, rotation, Vector2.One, animationId, ase, animationStartedTime, color, blend, sort, useScaledTime);
        
        public static bool DrawSprite(
            Batch2D spriteBatch,
            Vector2 pos,
            float rotation,
            string animationId,
            SpriteAsset ase,
            float animationStartedTime,
            Color color,
            float sort = 1,
            bool useScaledTime = true)
            => DrawSprite(spriteBatch, pos, rotation, Vector2.One, animationId, ase, animationStartedTime, color, RenderServices.BLEND_NORMAL, sort, useScaledTime);

        public static bool DrawSprite(
            Batch2D spriteBatch,
            Vector2 pos,
            float rotation,
            Vector2 scale,
            string animationId,
            SpriteAsset ase,
            float animationStartedTime,
            Color color,
            Vector3 blend,
            float sort = 1,
            bool useScaledTime = true,
            ImageFlip spriteEffects = ImageFlip.None)
        {
            if (animationId != null)
            {
                if (ase is null)
                {
                    return false;
                }
                
                if (!ase.Animations.TryGetValue(animationId, out var animation))
                {
                    return false;
                }

                var (frame, complete) = animation.Evaluate(animationStartedTime, useScaledTime ? Game.Now : Game.NowUnescaled, animation.AnimationDuration);
                var image = ase.GetFrame(frame);
                
                var imageSize = image.SourceRectangle.Size;

                Vector2 position = Vector2.Round(pos);

                image.Draw(spriteBatch, position, scale, ase.Origin, rotation, spriteEffects, color, blend, sort);

                return complete;
            }

            return false;
        }

        public static (SpriteAsset asset, string animation)? FetchPortraitAsSprite(Portrait portrait)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(portrait.Aseprite) is SpriteAsset aseprite)
            {
                return (aseprite, portrait.AnimationId);
            }

            return null;
        }

        public static bool DrawSprite(Batch2D batch, Guid assetGuid, Vector2 position, string animation, float startTime, DrawInfo drawInfo)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(assetGuid) is SpriteAsset asset)
            {
                if (drawInfo.Outline.HasValue)
                {
                    return DrawSpriteWithOutline(
                        batch,
                        position, animation, asset, 0, -1, drawInfo.Origin, drawInfo.FlippedHorizontal, drawInfo.Rotation,
                        drawInfo.Outline.Value, drawInfo.GetBlendMode(), drawInfo.Sort, drawInfo.UseScaledTime);
                }
                else
                {
                    return DrawSprite(
                    batch,
                    position,
                    animation,
                    asset,
                    startTime,
                    -1,
                    drawInfo.Origin,
                    drawInfo.FlippedHorizontal,
                    drawInfo.Rotation,
                    drawInfo.Scale,
                    drawInfo.Color,
                    drawInfo.GetBlendMode(),
                    drawInfo.Sort,
                    drawInfo.UseScaledTime);
                }
            }
            return false;
        }
        public static bool DrawSprite(Batch2D batch, Guid assetGuid, float x, float y, string animation, float startTime, DrawInfo drawInfo)
        {
            return DrawSprite(batch,assetGuid, new Vector2(x,y), animation,startTime, drawInfo);
        }
        public static bool DrawSprite(Batch2D batch, Guid assetGuid, float x, float y, string animation, DrawInfo drawInfo)
        {
            return DrawSprite(batch, assetGuid, new Vector2(x, y), animation, 0, drawInfo); ;
        }
        
        /// <summary>
		/// Draws a list of connecting points
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// /// <param name="position">Where to position the points</param>
		/// <param name="points">The points to connect with lines</param>
		/// <param name="color">The color to use</param>
		/// <param name="thickness">The thickness of the lines</param>
		private static void DrawPoints(Batch2D spriteBatch, Vector2 position, Vector2[] points, Color color, float thickness)
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
                origin: Vector2.Zero,
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

        public static void DrawSprite(Batch2D batch, Vector2 position, SpriteAsset ase, string animation, float sort, bool useScaledTime)
        {
            DrawSprite(batch, position, 0, animation, ase, 0, Color.White, sort, useScaledTime);
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

        #endregion

    }
}