using Bang.Entities;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Services.Info;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Murder.Services;

/// Some of the code based on https://github.com/z2oh/C3.MonoGame.Primitives2D/blob/master/Primitives2D.cs
public static partial class RenderServices
{
    const int Y_SORT_RANGE = 10000;
    const int Y_SORT_RANGE_X2 = Y_SORT_RANGE * 2;

    public static DrawMenuInfo DrawVerticalMenu(
        Batch2D batch,
        in Point position,
        in DrawMenuStyle style,
        in MenuInfo menuInfo,
        float sort = .1f) =>
        DrawVerticalMenu(batch, position, position, style, menuInfo, sort);

    /// <summary>
    /// TODO: Pass around a "style" for background color, sounds, etc.
    /// </summary>
    public static DrawMenuInfo DrawVerticalMenu(
        Batch2D batch,
        in Point position,
        in Point textPosition,
        in DrawMenuStyle style,
        in MenuInfo menuInfo,
        float sort)
    {
        PixelFont font = Game.Data.GetFont(style.Font);
        int lineHeight = font.LineHeight + style.ExtraVerticalSpace;

        int maxSelectionWidth = 0;

        Point finalPosition = new(Math.Max(position.X, 0), Math.Max(position.Y, 0));
        Point textFinalPosition = new(Math.Max(textPosition.X, 0), Math.Max(textPosition.Y, 0));

        Vector2 CalculateText(int index) => new Point(0, MathF.Floor(lineHeight * (index + 1.25f))) + textFinalPosition;
        Vector2 CalculateSelector(int index) => new Point(0, lineHeight * (index + 1)) + finalPosition;

        for (int i = 0; i < menuInfo.Length; i++)
        {
            var label = menuInfo.GetOptionText(i);
            Vector2 labelPosition = CalculateText(i);

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

            Point textSize = DrawText(batch, style.Font, label ?? string.Empty, labelPosition, new DrawInfo(sort)
            {
                Origin = style.Origin,
                Color = currentColor,
                Shadow = currentShadow,
                Debug = false
            });

            if (textSize.X > maxSelectionWidth)
            {
                maxSelectionWidth = textSize.X;
            }

            // We did not implement vertical icon menu with other offsets.
            if (i < menuInfo.Icons.Length && style.Origin.X == 0)
            {
                float bounceX = i != menuInfo.Selection ? 0 :
                    Ease.BackOut(Calculator.ClampTime(Game.NowUnscaled - menuInfo.LastMoved, 0.5f)) * 3 - 3;

                Portrait portrait = menuInfo.Icons[i];
                if (MurderAssetHelpers.GetSpriteAssetForPortrait(portrait) is (SpriteAsset sprite, string animation))
                {
                    DrawSprite(
                        batch,
                        sprite,
                        labelPosition - new Point(15 - bounceX, 0),
                        new DrawInfo(sort: sort),
                        new AnimationInfo(animation));
                }
            }
        }

        Vector2 selectorPosition = CalculateSelector(menuInfo.Selection) + new Vector2(0, MathF.Floor(lineHeight / 2f) - 3);
        Vector2 previousSelectorPosition = CalculateSelector(menuInfo.PreviousSelection) + new Vector2(0, lineHeight / 2f - 2);

        Vector2 easedPosition;
        if (style.SelectorMoveTime == 0)
            easedPosition = selectorPosition;
        else
            easedPosition = Vector2.Lerp(previousSelectorPosition, selectorPosition,
            Ease.Evaluate(Calculator.ClampTime(Game.NowUnscaled - menuInfo.LastMoved, style.SelectorMoveTime), style.Ease));

        return new DrawMenuInfo()
        {
            SelectorPosition = selectorPosition,
            PreviousSelectorPosition = previousSelectorPosition,
            SelectorEasedPosition = easedPosition.Point(),
            MaximumSelectionWidth = maxSelectionWidth
        };
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
                texture.Draw(batch, new Vector2(x, y), new Rectangle(Vector2.Zero, texture.Size - excess), Color.White, Vector2.One, 0, Vector2.Zero, ImageFlip.None, RenderServices.BLEND_NORMAL, MurderBlendState.AlphaBlend, sort);
            }
        }
    }

    /// <summary>
    /// The Renders a sprite on the screen. This is the most basic rendering method with all parameters exposed, avoid using this if possible.
    /// </summary>
    /// <param name="spriteBatch">Sprite batch.</param>
    /// <param name="pos">Position in the render.</param>
    /// <param name="clip">Clipping rectangle. Rectangle.Empty for the whole sprite</param>
    /// <param name="animationId">Animation string id.</param>
    /// <param name="asset">Sprite asset.</param>
    /// <param name="animationStartedTime">When the animation started.</param>
    /// <param name="animationDuration">The total duration of the animation. Use -1 to use the duration from the aseprite file.</param>
    /// <param name="animationLoop">If the animation should loop or if it's clamped.</param>
    /// <param name="origin">Offset from <paramref name="pos"/>. From 0 to 1.</param>
    /// <param name="imageFlip">Whether the image is flipped.</param>
    /// <param name="rotation">Rotation of the image, in radians.</param>
    /// <param name="scale">Scale applied to the sprite.</param>
    /// <param name="color">Color.</param>
    /// <param name="blend">Blend.</param>
    /// <param name="blendState">Blend state type.</param>
    /// <param name="sort">Sort layer. 0 is in front, 1 is behind</param>
    /// <param name="currentTime">Current time of the game used to render this sprite.</param>
    /// <returns>If the animation is complete or not</returns>
    public static FrameInfo DrawSprite(
        Batch2D spriteBatch,
        Vector2 pos,
        Rectangle clip,
        string animationId,
        SpriteAsset asset,
        float animationStartedTime,
        float animationDuration,
        bool animationLoop,
        Vector2 origin,
        ImageFlip imageFlip,
        float rotation,
        Vector2 scale,
        Color color,
        Vector3 blend,
        MurderBlendState blendState,
        float sort,
        float currentTime)
    {
        if (animationId == null || !asset.Animations.TryGetValue(animationId, out var animation))
        {
            GameLogger.Log($"Couldn't find animation {animationId}.");
            return FrameInfo.Fail;
        }

        float time = currentTime - animationStartedTime;

        var frameInfo = animation.Evaluate(time, animationLoop, animationDuration) with
        {
            Animation = animation
        };

        var image = asset.GetFrame(frameInfo.Frame);
        Vector2 offset = (asset.Origin + origin * image.Size).Round();
        Vector2 position = pos.Round();

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
            blendState: blendState,
            sort: sort);

        return frameInfo;
    }

    public static FrameInfo DrawSprite(
        Batch2D spriteBatch,
        Vector2 pos,
        Rectangle clip,
        string animationId,
        SpriteAsset asset,
        int frame,
        Vector2 origin,
        ImageFlip imageFlip,
        float rotation,
        Vector2 scale,
        Color color,
        Vector3 blend,
        float sort)
    {
        if (!asset.Animations.TryGetValue(animationId, out var animation))
        {
            GameLogger.Log($"Couldn't find animation {animationId}.");
            return FrameInfo.Fail;
        }

        var frameInfo = new FrameInfo()
        {
            Animation = animation,
            Frame = frame
        };

        var image = asset.GetFrame(animation.Frames[frameInfo.Frame]);
        Vector2 offset = (asset.Origin + origin * image.Size).Round();
        Vector2 position = pos.Round();

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
            blendState: MurderBlendState.AlphaBlend,
            sort: sort);

        return frameInfo;
    }


    public static void DealWithCompleteAnimations(Entity e, SpriteComponent s)
    {
        if (s.NextAnimations.Length > 1)
        {
            if (!string.IsNullOrWhiteSpace(s.CurrentAnimation))
            {
                e.PlaySpriteAnimation(s.NextAnimations.RemoveAt(0));
            }

            e.SendAnimationCompleteMessage(Messages.AnimationCompleteStyle.Single);
            e.RemoveAnimationComplete();
        }
        else
        {
            if (!e.HasAnimationComplete())
            {
                e.SetAnimationComplete();
                e.SendAnimationCompleteMessage(Messages.AnimationCompleteStyle.Sequence);
            }
        }
    }

    public static void MessageCompleteAnimations(Entity e)
    {
        if (e.TryGetAnimationOverload() is AnimationOverloadComponent overload)
        {
            if (!overload.Loop)
            {
                e.RemoveAnimationOverload();
            }

            e.SetAnimationComplete();
        }

        if (!e.HasAnimationComplete())
        {
            e.SetAnimationComplete();
        }

        e.SendAnimationCompleteMessage();
    }

    public static (SpriteAsset asset, string animation)? FetchPortraitAsSprite(Portrait portrait)
    {
        if (Game.Data.TryGetAsset<SpriteAsset>(portrait.Sprite) is SpriteAsset aseprite)
        {
            return (aseprite, portrait.AnimationId);
        }

        return null;
    }
    public static void DrawTexture(this Batch2D batch, Texture2D texture, Vector2 position, DrawInfo drawInfo)
    {
        batch.Draw(
            texture,
            position.ToXnaVector2(),
            texture.Bounds.Size(),
            texture.Bounds,
            drawInfo.Sort,
            drawInfo.Rotation,
            drawInfo.Scale.ToXnaVector2(),
            drawInfo.ImageFlip,
            drawInfo.Color,
            drawInfo.Origin.ToXnaVector2(),
            BLEND_NORMAL,
            drawInfo.BlendState
            );
    }

    public static FrameInfo DrawPortrait(this Batch2D batch, Portrait portrait, Vector2 position, DrawInfo drawInfo)
    {
        if (FetchPortraitAsSprite(portrait) is not (SpriteAsset sprite, string animation))
        {
            return FrameInfo.Fail;
        }

        return DrawSprite(batch, sprite, position, drawInfo, new AnimationInfo(animation));
    }

    public static FrameInfo DrawSprite(this Batch2D batch, Guid assetGuid, Vector2 position, DrawInfo? drawInfo = default)
        => DrawSprite(batch, assetGuid, position, drawInfo ?? DrawInfo.Default, AnimationInfo.Default);

    public static FrameInfo DrawSprite(this Batch2D batch, SpriteAsset asset, Vector2 position, DrawInfo? drawInfo = default)
        => DrawSprite(batch, asset, position, drawInfo ?? DrawInfo.Default, AnimationInfo.Default);

    public static FrameInfo DrawSprite(this Batch2D batch, Guid assetGuid, Vector2 position, DrawInfo drawInfo, AnimationInfo animationInfo)
    {
        if (Game.Data.TryGetAsset<SpriteAsset>(assetGuid) is SpriteAsset asset)
        {
            return DrawSprite(batch, asset, position, drawInfo, animationInfo);
        }
        return FrameInfo.Fail;
    }

    public static FrameInfo DrawSprite(this Batch2D batch, Guid assetGuid, float x, float y, DrawInfo drawInfo, AnimationInfo animationInfo)
    {
        return DrawSprite(batch, assetGuid, new Vector2(x, y), drawInfo, animationInfo);
    }

    public static FrameInfo DrawSprite(this Batch2D batch, SpriteAsset asset, Vector2 position, DrawInfo drawInfo, AnimationInfo animationInfo)
    {
        FrameInfo drawAt(Vector2 position, Color color, bool wash, float sort)
        {
            FrameInfo frameInfo = DrawSprite(
            batch,
            position,
            drawInfo.Clip,
            animationInfo.Name,
            asset,
            animationInfo.Start,
            animationInfo.Duration,
            animationInfo.Loop,
            drawInfo.Origin,
            drawInfo.ImageFlip,
            drawInfo.Rotation,
            drawInfo.Scale,
            color,
            wash ? RenderServices.BLEND_WASH : drawInfo.GetBlendMode(),
            drawInfo.BlendState,
            sort,
            animationInfo.CurrentTime());

#if DEBUG
            if (frameInfo.Failed)
            {
                DrawRectangle(batch, new Rectangle(position, new Vector2(32, 32)), Color.White * Calculator.Wave(10), sort);
                DrawText(batch, MurderFonts.PixelFont, $"<c=#ffffff>Sprite: </c>{asset.Name}\n\n<c=#ffffff>Animation:  </c>{animationInfo.Name}", position, new DrawInfo(0)
                {
                    Color = Color.Orange,
                    Outline = Color.Black
                }); ;
            }
#endif

            return frameInfo;
        }

        if (animationInfo.Name == "_")
        {
            return new FrameInfo(0, 0, true, Animation.Empty);
        }

        if (drawInfo.Outline.HasValue && drawInfo.OutlineStyle != OutlineStyle.None)
        {
            if (drawInfo.OutlineStyle.HasFlag(OutlineStyle.Bottom))
            {
                drawAt(position + new Vector2(0, 1), drawInfo.Outline.Value, true, drawInfo.Sort + 0.0001f);
            }

            if (drawInfo.OutlineStyle.HasFlag(OutlineStyle.Top))
            {
                drawAt(position + new Vector2(0, -1), drawInfo.Outline.Value, true, drawInfo.Sort + 0.0001f);
            }

            if (drawInfo.OutlineStyle.HasFlag(OutlineStyle.Left))
            {
                drawAt(position + new Vector2(-1, 0), drawInfo.Outline.Value, true, drawInfo.Sort + 0.0001f);
            }

            if (drawInfo.OutlineStyle.HasFlag(OutlineStyle.Right))
            {
                drawAt(position + new Vector2(1, 0), drawInfo.Outline.Value, true, drawInfo.Sort + 0.0001f);
            }
        }

        if (drawInfo.Shadow.HasValue)
        {
            int shadowOffset = 1;

            // Make sure the shadow shows up even if there is an outline.
            if (drawInfo.Outline.HasValue && drawInfo.OutlineStyle.HasFlag(OutlineStyle.Bottom))
            {
                shadowOffset = 2;
            }

            drawAt(position + new Vector2(0, shadowOffset), drawInfo.Shadow.Value, true, drawInfo.Sort + 0.0002f);
        }

        return drawAt(position, drawInfo.Color, false, drawInfo.Sort);
    }

    /// <summary>
    /// Draws a list of connecting points
    /// </summary>
    /// <param name="spriteBatch">The destination drawing surface</param>
    /// <param name="position">Where to position the points</param>
    /// <param name="points">The points to connect with lines</param>
    /// <param name="color">The color to use</param>
    /// <param name="thickness">The thickness of the lines</param>
    public static void DrawPoints(this Batch2D spriteBatch, Vector2 position, Vector2[] points, Color color, float thickness)
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
    /// <param name="position">Where to position the points</param>
    /// <param name="points">The points to connect with lines</param>
    /// <param name="color">The color to use</param>
    /// <param name="thickness">The thickness of the lines</param>
    /// <param name="sort">Sorting offset</param>
    public static void DrawPoints(this Batch2D spriteBatch, Vector2 position, ImmutableArray<Vector2> points, Color color, float thickness, float sort)
    {
        if (points.Length < 2)
            return;

        for (int i = 1; i < points.Length; i++)
        {
            DrawLine(spriteBatch, (points[i - 1] + position).Round(), (points[i] + position).Round(), color, thickness, sort);
        }

        DrawLine(spriteBatch, (points[points.Length - 1] + position).Round(), (points[0] + position).Round(), color, thickness, sort);
    }

    /// <summary>
    /// Draws a list of connecting points
    /// </summary>
    /// <param name="spriteBatch">The destination drawing surface</param>
    /// /// <param name="position">Where to position the points</param>
    /// <param name="points">The points to connect with lines</param>
    /// <param name="color">The color to use</param>
    /// <param name="thickness">The thickness of the lines</param>
    public static void DrawPoints(this Batch2D spriteBatch, Vector2 position, ReadOnlySpan<Vector2> points, Color color, float thickness)
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
        DrawRectangle(spriteBatch, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - 1, rectangle.Width + lineWidth - 1, lineWidth), color, sorting);
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

    public static void DrawRectangle(this Batch2D batch, Rectangle rectangle, DrawInfo drawInfo)
    {
        batch.Draw(
            texture: SharedResources.GetOrCreatePixel(),
            position: rectangle.TopLeft.ToXnaVector2(),
            targetSize: Point.One,
            sourceRectangle: default,
            sort: drawInfo.Sort,
            rotation: 0,
            scale: rectangle.Size.ToXnaVector2(),
            flip: ImageFlip.None,
            color: drawInfo.Color,
            offset: Microsoft.Xna.Framework.Vector2.Zero,
            drawInfo.GetBlendMode(),
            drawInfo.BlendState
            );
    }

    public static void DrawRectangle(this Batch2D batch, Rectangle rectangle, Color color, float sorting = 0)
    {
        batch.Draw(
            texture: SharedResources.GetOrCreatePixel(),
            position: rectangle.TopLeft.ToXnaVector2(),
            targetSize: Point.One,
            sourceRectangle: default,
            sort: sorting,
            rotation: 0,
            scale: rectangle.Size.ToXnaVector2(),
            flip: ImageFlip.None,
            color: color,
            offset: Microsoft.Xna.Framework.Vector2.Zero,
            BLEND_COLOR_ONLY,
            MurderBlendState.AlphaBlend // Default blend state
            );
    }


    /// <summary>
    /// Fetch the current time for this animation.
    /// </summary>
    public static float CurrentTime(this AnimationInfo @this) => @this.OverrideCurrentTime != -1 ? @this.OverrideCurrentTime :
        @this.UseScaledTime ? Game.Now : Game.NowUnscaled;

    public static Vector3 ToVector3(this BlendStyle blendStyle)
    {
        switch (blendStyle)
        {
            case BlendStyle.Normal: return new(1, 0, 0);
            case BlendStyle.Wash: return new(0, 1, 0);
            case BlendStyle.Color: return new(0, 0, 1);
            default:
                throw new Exception("Blend mode not supported!");
        }
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
    public static void DrawLineShort(this Batch2D spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness, float sort = 1f)
    {
        // calculate the distance between the two vectors
        float distance = Vector2.Distance(point1, point2);

        // calculate the angle between the two vectors
        float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

        DrawLineShort(spriteBatch, point1, distance, angle, color, thickness, sort);
    }

    public static void DrawLine(this Batch2D spriteBatch, Vector2 point, float length, float angle, Color color, float sort = 1f) =>
        DrawLine(spriteBatch, point, length, angle, color, 1f, sort);


    public static void DrawLine(this Batch2D spriteBatch, Vector2 point, float length, float angle, Color color, float thickness, float sort = 1f)
    {
        var halfPixel = new Vector2(thickness / 2f, 0);
        // stretch the pixel between the two vectors
        spriteBatch.Draw(SharedResources.GetOrCreatePixel(),
                         (point - halfPixel.Rotate(angle)).ToXnaVector2(),
                         Microsoft.Xna.Framework.Vector2.One,
                         default,
                         sort,
                         angle,
                         new Microsoft.Xna.Framework.Vector2(length + 0.75f, thickness),
                         ImageFlip.None,
                         color,
                         new Microsoft.Xna.Framework.Vector2(0, 0.5f),
                         BLEND_NORMAL,
                         MurderBlendState.AlphaBlend
                         );
    }

    public static void DrawLineShort(this Batch2D spriteBatch, Vector2 point, float length, float angle, Color color, float thickness, float sort = 1f)
    {
        var halfPixel = new Vector2(thickness / 2f, 0);
        // stretch the pixel between the two vectors
        spriteBatch.Draw(SharedResources.GetOrCreatePixel(),
                         (point - halfPixel.Rotate(angle)).ToXnaVector2(),
                         Microsoft.Xna.Framework.Vector2.One,
                         default,
                         sort,
                         angle,
                         new Microsoft.Xna.Framework.Vector2(length, thickness),
                         ImageFlip.None,
                         color,
                         new Microsoft.Xna.Framework.Vector2(0, 0.5f),
                         BLEND_NORMAL,
                         MurderBlendState.AlphaBlend
                         );
    }

    public static void DrawArrow(this Batch2D spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness, float headSize, float sort = 1f)
    {
        DrawLineShort(spriteBatch, point1, point2, color, thickness, sort);

        Vector2 direction = Vector2.Normalize(point2 - point1);
        Vector2 perpendicular = new Vector2(-direction.Y, direction.X);

        Vector2 arrowPoint1 = point2 - direction * headSize + perpendicular * headSize;
        Vector2 arrowPoint2 = point2 - direction * headSize - perpendicular * headSize;

        DrawLine(spriteBatch, point2, arrowPoint1, color, thickness, sort);
        DrawLine(spriteBatch, point2, arrowPoint2, color, thickness, sort);
    }
    #endregion

    #region Circle and Arcs
    public static void DrawCircleOutline(this Batch2D spriteBatch, Point center, float radius, int sides, Color color, float sort = 1f) =>
        DrawCircleOutline(spriteBatch, center.ToVector2(), radius, sides, color, sort);


    /// <summary>
    /// Draw a circle
    /// </summary>
    /// <param name="spriteBatch">The destination drawing surface</param>
    /// <param name="center">The center of the circle</param>
    /// <param name="radius">The radius of the circle</param>
    /// <param name="sides">The number of sides to generate</param>
    /// <param name="color">The color of the circle</param>
    /// <param name="sort">The sorting value</param>
    public static void DrawCircleOutline(this Batch2D spriteBatch, Vector2 center, float radius, int sides, Color color, float sort = 1f)
    {
        DrawPoints(spriteBatch, center, GeometryServices.CreateCircle(radius, sides), color, sort);
    }

    public static void DrawCircleOutline(this Batch2D spriteBatch, Rectangle rectangle, int sides, Color color)
    {
        DrawPoints(spriteBatch, rectangle.TopLeft, GeometryServices.CreateOrGetCircle(rectangle.Size, sides), color, 1.0f);
    }




    public static void DrawPoint(this Batch2D spriteBatch, Point pos, Color color, float sorting = 0)
    {
        DrawRectangle(spriteBatch, new Rectangle(pos, Point.One), color, sorting);
    }
    #endregion

    #region Drawing

    public static Vector3 BLEND_NORMAL = new(1, 0, 0);
    public static Vector3 BLEND_WASH = new(0, 1, 0);
    public static Vector3 BLEND_COLOR_ONLY = new(0, 0, 1);

    public static void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, Effect? effect, BlendState blend, bool smoothing)
    {
        (VertexInfo[] verts, short[] indices) = MakeTexturedQuad(destination, source, new Vector2(texture.Width, texture.Height), color, BLEND_NORMAL);

        DrawIndexedVertices(matrix, Game.GraphicsDevice, verts, verts.Length, indices, indices.Length / 3, effect, blend, texture, smoothing);
    }

    public static void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, BlendState blend)
    {
        (VertexInfo[] verts, short[] indices) = MakeTexturedQuad(destination, source, new Vector2(texture.Width, texture.Height), color, BLEND_NORMAL);

        if (blend == BlendState.Additive)
            Game.Data.ShaderSprite?.SetTechnique("Add");
        else
            Game.Data.ShaderSprite?.SetTechnique("Alpha");

        DrawIndexedVertices(
            matrix,
            Game.GraphicsDevice, verts, verts.Length, indices, indices.Length / 3, Game.Data.ShaderSprite,
            blend,
            texture,
            false);
    }

    public static void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, BlendState blend, Effect? shaderEffect)
        => DrawTextureQuad(texture, source, destination, matrix, color, blend, shaderEffect, BLEND_NORMAL);
    public static void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, BlendState blend, Effect? shaderEffect, Vector3 colorBlend)
    {
        (VertexInfo[] verts, short[] indices) = MakeTexturedQuad(destination, source, new Vector2(texture.Width, texture.Height), color, colorBlend);

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

        Game.Data.ShaderSprite?.SetTechnique("Alpha");

        DrawIndexedVertices(
            Microsoft.Xna.Framework.Matrix.CreateTranslation(Microsoft.Xna.Framework.Vector3.Zero),
            Game.GraphicsDevice, verts, verts.Length, indices, indices.Length / 3, Game.Data.ShaderSprite,
            BlendState.AlphaBlend,
            null);
    }

    private static Vector2[] _cachedPieChartVertices = Array.Empty<Vector2>();
    static readonly VertexInfo[] _cachedRectVertices = new VertexInfo[4];
    static readonly short[] _cachedRectIndices = new short[6];

    static RenderServices()
    {
        for (int i = 0; i < 4; i++)
        {
            _cachedRectVertices[i] = new VertexInfo();
        }
    }

    private static (VertexInfo[] vertices, short[] indices) MakeQuad(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color, Vector3 BlendType)
    {
        // 0---1
        // |\  |
        // | \ |
        // |  \|
        // 3---2

        _cachedRectVertices[0].Position = p1.ToVector3();
        _cachedRectVertices[0].Color = color;
        _cachedRectVertices[0].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 0);
        _cachedRectVertices[0].BlendType = BlendType;

        _cachedRectVertices[1].Position = p2.ToVector3();
        _cachedRectVertices[1].Color = color;
        _cachedRectVertices[1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 0);
        _cachedRectVertices[1].BlendType = BlendType;

        _cachedRectVertices[2].Position = p3.ToVector3();
        _cachedRectVertices[2].Color = color;
        _cachedRectVertices[2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 1);
        _cachedRectVertices[2].BlendType = BlendType;

        _cachedRectVertices[3].Position = p4.ToVector3();
        _cachedRectVertices[3].Color = color;
        _cachedRectVertices[3].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 1);
        _cachedRectVertices[3].BlendType = BlendType;

        _cachedRectIndices[0] = 0;
        _cachedRectIndices[1] = 1;
        _cachedRectIndices[2] = 2;
        _cachedRectIndices[3] = 0;
        _cachedRectIndices[4] = 2;
        _cachedRectIndices[5] = 3;


        return (_cachedRectVertices, _cachedRectIndices);
    }

    private static (VertexInfo[] vertices, short[] indices) MakeRegularQuad(Rectangle rect, Color color, Vector3 BlendType)
    {
        // 0---1
        // |\  |
        // | \ |
        // |  \|
        // 3---2

        _cachedRectVertices[0].Position = rect.TopLeft.ToVector3();
        _cachedRectVertices[0].Color = color;
        _cachedRectVertices[0].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 0);
        _cachedRectVertices[0].BlendType = BlendType;

        _cachedRectVertices[1].Position = rect.TopRight.ToVector3();
        _cachedRectVertices[1].Color = color;
        _cachedRectVertices[1].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 0);
        _cachedRectVertices[1].BlendType = BlendType;

        _cachedRectVertices[2].Position = rect.BottomRight.ToVector3();
        _cachedRectVertices[2].Color = color;
        _cachedRectVertices[2].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(1, 1);
        _cachedRectVertices[2].BlendType = BlendType;

        _cachedRectVertices[3].Position = rect.BottomLeft.ToVector3();
        _cachedRectVertices[3].Color = color;
        _cachedRectVertices[3].TextureCoordinate = new Microsoft.Xna.Framework.Vector2(0, 1);
        _cachedRectVertices[3].BlendType = BlendType;

        _cachedRectIndices[0] = 0;
        _cachedRectIndices[1] = 1;
        _cachedRectIndices[2] = 2;
        _cachedRectIndices[3] = 0;
        _cachedRectIndices[4] = 2;
        _cachedRectIndices[5] = 3;


        return (_cachedRectVertices, _cachedRectIndices);
    }

    private static (VertexInfo[] vertices, short[] indices) MakeTexturedQuad(Rectangle destination, Rectangle source, Vector2 sourceSize, Color color, Vector3 BlendType)
    {
        // 0---1
        // |\  |
        // | \ |
        // |  \|
        // 3---2

        Microsoft.Xna.Framework.Vector2 uvTopLeft = new(source.X / sourceSize.X, source.Y / sourceSize.Y);
        Microsoft.Xna.Framework.Vector2 uvTopRight = new((source.X + source.Width) / sourceSize.X, source.Y / sourceSize.Y);
        Microsoft.Xna.Framework.Vector2 uvBottomRight = new((source.X + source.Width) / sourceSize.X, (source.Y + source.Height) / sourceSize.Y);
        Microsoft.Xna.Framework.Vector2 uvBottomLeft = new(source.X / sourceSize.X, (source.Y + source.Height) / sourceSize.Y);

        _cachedRectVertices[0].Position = destination.TopLeft.ToVector3();
        _cachedRectVertices[0].Color = color;
        _cachedRectVertices[0].TextureCoordinate = uvTopLeft;
        _cachedRectVertices[0].BlendType = BlendType;

        _cachedRectVertices[1].Position = destination.TopRight.ToVector3();
        _cachedRectVertices[1].Color = color;
        _cachedRectVertices[1].TextureCoordinate = uvTopRight;
        _cachedRectVertices[1].BlendType = BlendType;

        _cachedRectVertices[2].Position = destination.BottomRight.ToVector3();
        _cachedRectVertices[2].Color = color;
        _cachedRectVertices[2].TextureCoordinate = uvBottomRight;
        _cachedRectVertices[2].BlendType = BlendType;

        _cachedRectVertices[3].Position = destination.BottomLeft.ToVector3();
        _cachedRectVertices[3].Color = color;
        _cachedRectVertices[3].TextureCoordinate = uvBottomLeft;
        _cachedRectVertices[3].BlendType = BlendType;

        _cachedRectIndices[0] = 0;
        _cachedRectIndices[1] = 1;
        _cachedRectIndices[2] = 2;
        _cachedRectIndices[3] = 0;
        _cachedRectIndices[4] = 2;
        _cachedRectIndices[5] = 3;


        return (_cachedRectVertices, _cachedRectIndices);
    }

    public static void DrawIndexedVertices<T>(Matrix matrix, GraphicsDevice graphicsDevice, T[] vertices, int vertexCount, short[] indices, int primitiveCount, Effect? effect, BlendState? blendState = null, Texture2D? texture = null, bool smoothing = false) where T : struct, IVertexType
    {
        var b = blendState ?? BlendState.AlphaBlend;

        var size = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

        matrix *= Matrix.CreateScale((1f / size.X) * 2, -(1f / size.Y) * 2, 1f); // scale to relative points
        matrix *= Matrix.CreateTranslation(-1f, 1f, 0); // translate to relative points

        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.BlendState = b;
        graphicsDevice.SamplerStates[0] = smoothing ? SamplerState.LinearClamp : SamplerState.PointClamp;


        if (effect is not null)
        {
            effect.Parameters["MatrixTransform"].SetValue(matrix);

            if (texture is not null)
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    try
                    {
                        pass.Apply();
                    }
                    catch (Exception e)
                    {
                        GameLogger.Error($"Error applying effect pass: {e.Message}");
                    }
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
    }

    public static void DrawPolygon(this Batch2D batch, ImmutableArray<Vector2> vertices, DrawInfo? drawInfo = default)
    {
        batch.DrawPolygon(SharedResources.GetOrCreatePixel(), vertices, drawInfo ?? DrawInfo.Default);
    }

    public static void DrawFilledCircle(this Batch2D batch, Vector2 center, float radius, int steps, DrawInfo? drawInfo = default)
    {
        Vector2[] circleVertices = GeometryServices.CreateOrGetFlattenedCircle(1f, 1f, steps);

        // Scale and translate the vertices
        var scaledTranslatedVertices = circleVertices.Select(p => new Vector2(p.X * radius + center.X, p.Y * radius + center.Y)).ToArray();

        batch.DrawPolygon(SharedResources.GetOrCreatePixel(), scaledTranslatedVertices, drawInfo ?? DrawInfo.Default);
    }

    public static void DrawFilledCircle(this Batch2D batch, Rectangle circleRect, int steps, DrawInfo drawInfo)
    {
        Vector2[] circleVertices = GeometryServices.CreateOrGetFlattenedCircle(1f, 1f, steps);

        // Scale and translate the vertices
        batch.DrawPolygon(SharedResources.GetOrCreatePixel(), circleVertices, drawInfo.WithScale(circleRect.Size).WithOffset(circleRect.Center));
    }
    public static void DrawPieChart(this Batch2D batch, Vector2 center, float radius, float startAngle, float endAngle, int steps, DrawInfo? drawInfo = default)
    {
        // Ensure the cached vertices array is large enough
        if (_cachedPieChartVertices.Length < steps + 3) // +2 to accommodate the center point and ensure enough space
        {
            _cachedPieChartVertices = new Vector2[steps + 3];
        }

        Vector2[] circleVertices = GeometryServices.CreateOrGetFlattenedCircle(1f, 1f, steps);

        // Scale and translate the vertices
        for (int i = 0; i <= steps; i++)
        {
            _cachedPieChartVertices[i + 1] = new Vector2(circleVertices[i].X * radius + center.X, circleVertices[i].Y * radius + center.Y);
        }

        // Calculate the angle between each step
        float angleStep = (MathF.PI * 2) / steps;

        // Calculate the start and end index
        int startIndex = (int)(startAngle / angleStep);
        float startFraction = startAngle % angleStep;

        int endIndex = (int)((endAngle / angleStep) + 2);
        float endFraction = endAngle % angleStep;

        // Insert the center point at the beginning
        _cachedPieChartVertices[endIndex] = Point.Lerp(_cachedPieChartVertices[endIndex - 1], _cachedPieChartVertices[endIndex], endFraction);
        _cachedPieChartVertices[startIndex] = center;

        // Draw the pie chart
        batch.DrawPolygon(SharedResources.GetOrCreatePixel(), _cachedPieChartVertices[startIndex..(endIndex + 1)], drawInfo ?? DrawInfo.Default);
    }

    public static Point DrawText(this Batch2D uiBatch, int font, string text, Vector2 position, DrawInfo? drawInfo = default)
        => DrawText(uiBatch, font, text, position, -1, -1, drawInfo ?? DrawInfo.Default);
    public static Point DrawText(this Batch2D uiBatch, int font, string text, Vector2 position, int maxWidth, DrawInfo? drawInfo = default)
        => DrawText(uiBatch, font, text, position, maxWidth, -1, drawInfo ?? DrawInfo.Default);
    public static Point DrawText(this Batch2D uiBatch, MurderFonts font, string text, Vector2 position, DrawInfo? drawInfo = default)
        => DrawText(uiBatch, (int)font, text, position, -1, -1, drawInfo ?? DrawInfo.Default);
    public static Point DrawText(this Batch2D uiBatch, MurderFonts font, string text, Vector2 position, int maxWidth, DrawInfo? drawInfo = default)
        => DrawText(uiBatch, (int)font, text, position, maxWidth, -1, drawInfo ?? DrawInfo.Default);
    public static Point DrawText(this Batch2D uiBatch, MurderFonts font, string text, Vector2 position, int maxWidth, int visibleCharacters, DrawInfo? drawInfo = default)
        => DrawText(uiBatch, (int)font, text, position, maxWidth, visibleCharacters, drawInfo ?? DrawInfo.Default);

    public static Point DrawText(this Batch2D uiBatch, int pixelFont, string text, Vector2 position, int maxWidth, int visibleCharacters, DrawInfo drawInfo)
    {
        var font = Game.Data.GetFont(pixelFont, cultureInvariant: drawInfo.CultureInvariant);
        return font.Draw(uiBatch, text, position + drawInfo.Offset, drawInfo.Origin, drawInfo.Scale, drawInfo.Sort, drawInfo.Color, drawInfo.Outline, drawInfo.Shadow, maxWidth, visibleCharacters, drawInfo.Debug);
    }

    /// <summary>
    /// Draw a simple text. Without line wrapping, color formatting, line splitting or anything fancy.
    /// </summary>
    public static Point DrawSimpleText(this Batch2D uiBatch, int pixelFont, string text, Vector2 position, DrawInfo drawInfo)
    {
        var font = Game.Data.GetFont(pixelFont);
        return font.DrawSimple(uiBatch, text, position + drawInfo.Origin, drawInfo.Origin, drawInfo.Scale, drawInfo.Sort, drawInfo.Color, drawInfo.Outline, drawInfo.Shadow, drawInfo.Debug);
    }

    /// <summary>
    /// Don't forget to dispose this!
    /// </summary>
    /// <returns></returns>
    public static Texture2D? CreateGameplayScreenshot()
    {
        if (Game.Instance.ActiveScene?.RenderContext is not RenderContext render)
        {
            return null;
        }

        if (render.MainTarget is not RenderTarget2D mainTarget)
        {
            return null;
        }

        return CreateScreenshotFromTarget(mainTarget);
    }

    /// <summary>
    /// Don't forget to dispose this!
    /// </summary>
    /// <returns></returns>
    public static Texture2D? CreateScreenshot()
    {
        if (Game.Instance.ActiveScene?.RenderContext is not RenderContext render)
        {
            return null;
        }

        if (render.LastRenderTarget is not RenderTarget2D mainTarget)
        {
            return null;
        }

        return CreateScreenshotFromTarget(mainTarget);
    }

    /// <summary>
    /// Don't forget to dispose this!
    /// </summary>
    /// <returns></returns>
    public static Texture2D? CreateScreenshotFromTarget(RenderTarget2D mainTarget)
    {
        var gd = Game.GraphicsDevice;

        RenderTarget2D rt = new(gd, mainTarget.Width, mainTarget.Height, false, mainTarget.Format, mainTarget.DepthStencilFormat, mainTarget.MultiSampleCount, RenderTargetUsage.DiscardContents);
        gd.SetRenderTarget(rt);
        gd.Clear(Color.Transparent);

        DrawTextureQuad(mainTarget, mainTarget.Bounds, rt.Bounds, Matrix.Identity, Color.White, BlendState.Opaque);
        return rt;
    }

    public static bool TriggerEventsIfNeeded(Entity e, Guid currentAnimationGuid, AnimationInfo animationInfo, Core.FrameInfo frameInfo)
    {
        // Check for animation events
        // First, assume that we are starting a new animation
        RenderedSpriteCacheComponent? previousCache = e.TryGetRenderedSpriteCache();
        int previousFrame = previousCache?.LastFrameIndex ?? -1;

        // Make sure we didn't change animations
        if (previousCache is not RenderedSpriteCacheComponent cache ||
            cache.RenderedSprite != currentAnimationGuid ||
            !string.Equals(cache.AnimInfo.Name, animationInfo.Name, StringComparison.InvariantCulture))
        {
            // We changed animations, so we need to reset to -1 so we can trigger the first frame event
            previousFrame = frameInfo.InternalFrame - 1;
        }

        // Quickly check if we even changed frames, if not, don't bother with events
        if (frameInfo.InternalFrame != previousFrame)
        {
            Animation currentAnimation = frameInfo.Animation;

            if (currentAnimation.Events == null || currentAnimation.Events.Count == 0)
            {
                return false;
            }

            int lastFrame = previousFrame;
            while (lastFrame != frameInfo.InternalFrame)
            {
                // Trigger events on the next frame (most likelly the frame just rendered, uunless there was a major slowdown)

                if (animationInfo.Loop)
                {
                    int next = Calculator.WrapAround(lastFrame + 1, 0, currentAnimation.FrameCount - 1);

                    if (currentAnimation.Events.TryGetValue(next, out string? eventName))
                    {
                        e.SendAnimationEventMessage(eventName);
                    }

                    lastFrame = next;

                    if (previousFrame == lastFrame)
                    {
                        // We've looped through all the frames and didn't find the previous frame, so we're stuck in a loop or a major slowdown happened
                        return true;
                    }
                }
                else
                {
                    int next = Math.Min(lastFrame + 1, currentAnimation.FrameCount - 1);

                    if (currentAnimation.Events.TryGetValue(next, out string? eventName))
                    {
                        e.SendAnimationEventMessage(eventName);
                    }
                    lastFrame = next;

                    // This animation doesn't loop and we've reached the end, so we're done
                    if (next == currentAnimation.FrameCount - 1)
                    {
                        return false;
                    }
                }

            }
        }
        return false;
    }

    public static string? CheckForEvents(
        RenderedSpriteCacheComponent? previous,
        Guid currentAnimationGuid,
        AnimationInfo animationInfo,
        FrameInfo frameInfo)
    {
        // Check for animation events
        // First, assume that we are starting a new animation
        int previousFrame = previous?.LastFrameIndex ?? -1;

        // Make sure we didn't change animations
        if (previous is null ||
            previous.Value.RenderedSprite != currentAnimationGuid ||
            !string.Equals(previous.Value.AnimInfo.Name, animationInfo.Name, StringComparison.InvariantCulture))
        {
            // We changed animations, so we need to reset to -1 so we can trigger the first frame event
            previousFrame = frameInfo.InternalFrame - 1;
        }

        // Quickly check if we even changed frames, if not, don't bother with events
        if (frameInfo.InternalFrame != previousFrame)
        {
            Animation currentAnimation = frameInfo.Animation;

            if (currentAnimation.Events is null || currentAnimation.Events.Count == 0)
            {
                return null;
            }

            int lastFrame = previousFrame;
            while (lastFrame != frameInfo.InternalFrame)
            {
                // Trigger events on the next frame (most likelly the frame just rendered, uunless there was a major slowdown)

                if (animationInfo.Loop)
                {
                    int next = Calculator.WrapAround(lastFrame + 1, 0, currentAnimation.FrameCount - 1);

                    if (currentAnimation.Events.TryGetValue(next, out string? eventName))
                    {
                        return eventName;
                    }

                    lastFrame = next;
                    if (previousFrame == lastFrame)
                    {
                        // We've looped through all the frames and didn't find the previous frame, so we're stuck in a loop or a major slowdown happened
                        return null;
                    }
                }
                else
                {
                    int next = Math.Min(lastFrame + 1, currentAnimation.FrameCount - 1);

                    if (currentAnimation.Events.TryGetValue(next, out string? eventName))
                    {
                        return eventName;
                    }

                    lastFrame = next;
                    // This animation doesn't loop and we've reached the end, so we're done
                    if (next == currentAnimation.FrameCount - 1)
                    {
                        return null;
                    }

                }
            }
        }

        return null;
    }

    #endregion
}