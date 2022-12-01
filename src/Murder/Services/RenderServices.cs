using Microsoft.Xna.Framework.Graphics;
using Bang.Entities;

using Murder.Core.Graphics;
using Murder.Core.Geometry;
using Murder.Assets.Graphics;
using Murder.Data;
using Murder.Utilities;
using Murder.Components;
using Murder.Messages;

using Matrix = Microsoft.Xna.Framework.Matrix;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Murder.Diagnostics;
using System.Runtime.CompilerServices;

namespace Murder.Services
{
    /// Some of the code based on https://github.com/z2oh/C3.MonoGame.Primitives2D/blob/master/Primitives2D.cs
    public static class RenderServices
    {
        private static readonly Dictionary<String, List<Vector2>> _circleCache = new();
        private static readonly Dictionary<String, List<Vector2>> _flatCircleCache = new();

        public enum Orientation
        {
            Horizontal,
            Vertical
        }

        public static void Render3Slice(
            Batch2D batch,
            AtlasTexture texture,
            Rectangle core,
            Vector2 position,
            Vector2 size,
            Vector2 origin,
            float sort)
        {

            var left = position + new Vector2(-size.X * origin.X, -size.Y * origin.Y).Round(); // TODO: Why there's a magic +1 here?
            // Left
            texture.Draw(
                batch,
                left,
                clip: new IntRectangle(0, core.Y , core.X, core.Height),
                Color.White,
                sort,
                RenderServices.BLEND_NORMAL
                );

            var midPosition = left + new Vector2(core.X, 0).Round();
            var rightSliceSize = new Vector2(texture.Size.X - core.X - core.Width, core.Y);
            var midSize = new Vector2(size.X - core.X - rightSliceSize.X, core.Height);
            // Mid
            texture.Draw(
                batch,
                clip: new IntRectangle(core.X, core.Y, core.Width, core.Height),
                target: new Rectangle(midPosition.X, midPosition.Y, midSize.Width, midSize.Height),
                Color.White,
                sort,
                RenderServices.BLEND_NORMAL
                );

            // Right
            texture.Draw(
                batch,
                position: new Vector2(midPosition.X + midSize.Width, position.Y - size.Y * origin.Y).Round(),
                clip: new IntRectangle(core.X + core.Width, core.Y, core.X, core.Height),
                Color.White,
                sort,
                RenderServices.BLEND_NORMAL
                );

            //batch.DrawRectangleOutline(new IntRectangle(position.X - size.X * origin.X, position.Y - size.Y * origin.Y, size.X, size.Y), Color.Red);
        }

        public static void RenderRepeating(
            Batch2D batch,
            AtlasTexture texture,
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
        /// <param name="camera">Camera of the world. Used to verify whether the sprite will be shown in screen and call Image.Draw()</param>
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
        public static bool RenderSprite(
            Batch2D spriteBatch,
            Camera2D camera,
            Vector2 pos,
            string animationId,
            AsepriteAsset ase,
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

                var (imgPath, complete) = animation.Evaluate(animationStartedTime, useScaledTime? Game.Now : Game.NowUnescaled, animationDuration);
                if (Game.Data.FetchAtlas(AtlasId.Gameplay)?.Get(imgPath) is not AtlasTexture image)
                {
                    return false;
                }
                
                Vector2 imageOffset = ase.Origin.ToVector2() + new Vector2(image.Size.X * offset.X, image.Size.Y * offset.Y);
                Vector2 position = Vector2.Round(pos);

                if (spriteBatch.ClipWhenOutOfBounds && !camera.SafeBounds.Touches(new(position - imageOffset, image.Size)))
                {
                    // [Perf] Skip drawing sprites that do not show up in the camera rectangle.
                    return false;
                }
                
                image.Draw(
                    spriteBatch,
                    position,
                    scale,
                    imageOffset,
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
        /// <param name="camera">Camera of the world. Used to verify whether the sprite will be shown in screen and call Image.Draw()</param>
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
        public static bool RenderSpriteWithOutline(
            Batch2D spriteBatch,
            Camera2D camera,
            Vector2 pos,
            string animationId,
            AsepriteAsset ase,
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

                var (imgPath, complete) = animation.Evaluate(animationStartedTime, useScaledTime ? Game.Now : Game.NowUnescaled, animationDuration);
                if (Game.Data.FetchAtlas(AtlasId.Gameplay)?.Get(imgPath) is not AtlasTexture image)
                {
                    return false;
                }

                Vector2 imageOffset = ase.Origin.ToVector2() + new Vector2(image.Size.X * offset.X, image.Size.Y * offset.Y);
                Vector2 position = Vector2.Round(pos);

                if (spriteBatch.ClipWhenOutOfBounds && !camera.SafeBounds.Touches(new(position, image.Size)))
                {
                    // [Perf] Skip drawing sprites that do not show up in the camera rectangle.
                    return false;
                }
                
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

        public static void MessageCompleteAnimations(Entity e, AsepriteComponent s, bool complete)
        {
            if (complete)
            {
                if (s.NextAnimations.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(s.NextAnimations[0]))
                        e.ReplaceComponent(s.Play(e.HasPauseAnimation(), s.NextAnimations));
                }
                e.SendMessage(new AnimationCompleteMessage());
            }
        }
        public static bool RenderSprite(
            Batch2D spriteBatch,
            Vector2 pos,
            float rotation,
            string animationId,
            AsepriteAsset ase,
            float animationStartedTime,
            Color color,
            float sort = 1,
            bool useScaledTime = true) 
            => RenderSprite(spriteBatch, AtlasId.Gameplay, pos, rotation, Vector2.One, animationId, ase, animationStartedTime, color, sort, useScaledTime);

        public static bool RenderSprite(
            Batch2D spriteBatch,
            AtlasId atlasId,
            Vector2 pos,
            float rotation,
            Vector2 scale,
            string animationId,
            AsepriteAsset ase,
            float animationStartedTime,
            Color color,
            float sort = 1,
            bool useScaledTime = true)
        {
            ImageFlip spriteEffects = ImageFlip.None;

            if (animationId != null)
            {
                if (!ase.Animations.TryGetValue(animationId, out var animation))
                {
                    return false;
                }

                var (imgPath, complete) = animation.Evaluate(animationStartedTime, useScaledTime ? Game.Now : Game.NowUnescaled, animation.AnimationDuration);
                if (Game.Data.FetchAtlas(atlasId)?.Get(imgPath) is not AtlasTexture image)
                {
                    return false;
                }

                var imageSize = image.SourceRectangle.Size;

                Vector2 position = Vector2.Round(pos - ase.Origin);

                var blend = RenderServices.BLEND_NORMAL;

                image.Draw(spriteBatch, position, scale, Vector2.Zero, rotation, spriteEffects, color, blend, sort);

                return complete;
            }

            return false;
        }

        /// <summary>
		/// Draws a list of connecting points
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// /// <param name="position">Where to position the points</param>
		/// <param name="points">The points to connect with lines</param>
		/// <param name="color">The color to use</param>
		/// <param name="thickness">The thickness of the lines</param>
		private static void DrawPoints(Batch2D spriteBatch, Vector2 position, List<Vector2> points, Color color, float thickness)
        {
            if (points.Count < 2)
                return;

            for (int i = 1; i < points.Count; i++)
            {
                DrawLine(spriteBatch, points[i - 1] + position, points[i] + position, color, thickness);
            }
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
            return (5000 - y) / 10000;
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
        public static void DrawCircle(this Batch2D spriteBatch, Point center, float radius, int sides, Color color) =>
            DrawCircle(spriteBatch, center.ToVector2(), radius, sides, color);


        /// <summary>
		/// Draw a circle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="center">The center of the circle</param>
		/// <param name="radius">The radius of the circle</param>
		/// <param name="sides">The number of sides to generate</param>
		/// <param name="color">The color of the circle</param>
		public static void DrawCircle(this Batch2D spriteBatch, Vector2 center, float radius, int sides, Color color)
        {
            DrawPoints(spriteBatch, center, CreateCircle(radius, sides), color, 1.0f);
        }

        public static void DrawFlatenedCircle(this Batch2D spriteBatch, Vector2 center, float radius, float scaleY, int sides, Color color)
        {
            DrawPoints(spriteBatch, center, CreateFlatenedCircle(radius, scaleY, sides), color, 1.0f);
        }

        /// <summary>
		/// Creates a list of vectors that represents a circle
		/// </summary>
		/// <param name="radius">The radius of the circle</param>
		/// <param name="sides">The number of sides to generate</param>
		/// <returns>A list of vectors that, if connected, will create a circle</returns>
		private static List<Vector2> CreateCircle(double radius, int sides)
        {
            // Look for a cached version of this circle
            String circleKey = $"{radius}x{sides}";
            if (_circleCache.ContainsKey(circleKey))
            {
                return _circleCache[circleKey];
            }

            List<Vector2> vectors = new List<Vector2>();

            const double max = 2.0 * Math.PI;
            double step = max / sides;

            for (double theta = 0.0; theta < max; theta += step)
            {
                vectors.Add(new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta))));
            }

            // then add the first vector again so it's a complete loop
            vectors.Add(new Vector2((float)(radius * Math.Cos(0)), (float)(radius * Math.Sin(0))));

            // Cache this circle so that it can be quickly drawn next time
            _circleCache.Add(circleKey, vectors);

            return vectors;
        }

        private static List<Vector2> CreateFlatenedCircle(float radius, float scaleY, int sides)
        {
            // Look for a cached version of this circle
            String circleKey = $"{radius}x{scaleY}x{sides}";
            if (_flatCircleCache.ContainsKey(circleKey))
            {
                return _flatCircleCache[circleKey];
            }

            List<Vector2> vectors = new List<Vector2>();

            const double max = 2.0 * Math.PI;
            double step = max / sides;

            for (double theta = 0.0; theta < max; theta += step)
            {
                vectors.Add(new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)) * scaleY));
            }

            // then add the first vector again so it's a complete loop
            vectors.Add(new Vector2((float)(radius * Math.Cos(0)), (float)(radius * Math.Sin(0)) * scaleY));

            // Cache this circle so that it can be quickly drawn next time
            _flatCircleCache.Add(circleKey, vectors);

            return vectors;
        }

        public static void DrawPoint(Batch2D spriteBatch, Point pos, Color color)
        {
            DrawRectangle(spriteBatch, new Rectangle(pos, Point.One), color);
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
        public static void DrawTextureQuad(AtlasTexture texture, Rectangle destination, Matrix matrix, Color color, BlendState blend)
        {
            (VertexInfo[] verts, short[] indices) = MakeTexturedQuad(destination, texture.SourceRectangle, texture.AtlasSize, color, BLEND_NORMAL);

            if (blend == BlendState.Additive)
                Game.Data.ShaderSprite.SetTechnique("Add");
            else
                Game.Data.ShaderSprite.SetTechnique("Alpha");

            DrawIndexedVertices(
                matrix,
                Game.GraphicsDevice, verts, verts.Length, indices, indices.Length / 3, Game.Data.ShaderSprite,
                blend,
                texture.Atlas);
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

        public static void DrawTextureQuad(AtlasTexture texture, Rectangle destination, Color color)
        {
            (VertexInfo[] verts, short[] indices) = MakeTexturedQuad(destination, texture.SourceRectangle, texture.AtlasSize, color, BLEND_NORMAL);

            Game.Data.ShaderSprite.SetTechnique("Alpha");

            DrawIndexedVertices(
                Microsoft.Xna.Framework.Matrix.CreateTranslation(Microsoft.Xna.Framework.Vector3.Zero),
                Game.GraphicsDevice, verts, verts.Length, indices, indices.Length / 3, Game.Data.ShaderSprite,
                BlendState.AlphaBlend,
                texture.Atlas);
        }

        public static void DrawQuad(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
        {
            (VertexInfo[] verts, short[] indices) = MakeQuad(p1, p2, p3, p4, color, BLEND_COLOR_ONLY);

            Game.Data.ShaderSprite.SetTechnique("Alpha");
            DrawIndexedVertices(
                Microsoft.Xna.Framework.Matrix.CreateTranslation(Microsoft.Xna.Framework.Vector3.Zero),
                Game.GraphicsDevice, verts, verts.Length, indices, indices.Length / 3, Game.Data.ShaderSprite,
                BlendState.AlphaBlend,
                null);
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
        public static void DrawTextureQuad(Texture2D texture, Point position, Matrix matrix)
        {
            DrawTextureQuad(texture, new Rectangle(position, new Point(texture.Width, texture.Height)), BLEND_NORMAL, Color.White, matrix);
        }
        public static void DrawTextureQuad(Texture2D texture, Rectangle rect, Vector3 blend, Color color, Matrix matrix)
        {
            (VertexInfo[] verts, short[] indices) = MakeRegularQuad(rect, color, blend);

            DrawIndexedVertices(
                matrix,
                Game.GraphicsDevice, verts, verts.Length, indices, indices.Length / 3, Game.Data.ShaderSprite,
                BlendState.AlphaBlend,
                texture);
        }

        public static void DrawTextureQuad(Texture2D texture, Rectangle rect, Vector3 blend, Color color, Effect effect, bool smoothing)
        {

            Game.GraphicsDevice.SamplerStates[0] = smoothing? SamplerState.PointClamp : SamplerState.AnisotropicClamp;
            (VertexInfo[] verts, short[] indices) = MakeRegularQuad(rect, color, blend);
            
            DrawIndexedVertices(
                Microsoft.Xna.Framework.Matrix.CreateTranslation(Microsoft.Xna.Framework.Vector3.Zero),
                Game.GraphicsDevice, verts, verts.Length, indices, indices.Length / 3, effect,
                BlendState.AlphaBlend,
                texture);
        }

        public static void DrawTextureQuad(Texture2D texture, Rectangle rect, Vector3 blend, Color color)
        {
            DrawTextureQuad(texture, rect, blend, color, Matrix.Identity);
        }

        static readonly VertexInfo[] _cachedVertices = new VertexInfo[4];
        static readonly short[] _cachedIndices = new short[6];
        static public BlendState MultiplyBlend;

        static RenderServices()
        {
            for (int i = 0; i < 4; i++)
            {
                _cachedVertices[i] = new VertexInfo();
            }

            MultiplyBlend = new BlendState()
            {
                ColorBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero,
            };
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

        public static void DrawVertices<T>(Microsoft.Xna.Framework.Matrix matrix, GraphicsDevice graphicsDevice, T[] vertices, int vertexCount, Effect effect, Texture2D? texture = null) where T: struct, IVertexType
        {
            var b = BlendState.AlphaBlend;

            var size = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            matrix *= Matrix.CreateScale((1f / size.X) * 2, -(1f / size.Y) * 2, 1f); // scale to relative points
            matrix *= Matrix.CreateTranslation(-1f, 1f, 0); // translate to relative points


            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.BlendState = b;

            effect.Parameters["MatrixTransform"].SetValue(matrix);
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                if (texture != null)
                    graphicsDevice.Textures[0] = texture;

                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertexCount / 3);
            }
        }

        public static void DrawIndexedVertices<T>(Matrix matrix, GraphicsDevice graphicsDevice, T[] vertices, int vertexCount, short[] indices, int primitiveCount, Effect effect, BlendState? blendState = null, Texture2D? texture = null) where T : struct, IVertexType
        {
            var b = blendState != null ? blendState : BlendState.AlphaBlend;
            
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
        #endregion
    }

}