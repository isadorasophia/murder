using Microsoft.Xna.Framework.Graphics;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Assets.Graphics
{
    public class TilesetAsset : GameAsset
    {
        public override char Icon => '\uf84c';
        public override string EditorFolder => "#Tilesets";

        [GameAssetId(typeof(AsepriteAsset))]
        public readonly Guid Image = Guid.Empty;

        public readonly Point Offset = new();
        public readonly Point Size = new();

        public readonly int YSortOffset = 0;

        public readonly bool ConsiderOutsideOccupied = false;


        [Tooltip("Whether this tile has a collision or not.")]
        public readonly bool IsSolid = new();
        
        /// <summary>
        /// This is the order (or layer) which this tileset will be drawn into the screen.
        /// </summary>
        public readonly int Order = new();

        public TargetSpriteBatches TargetBatch = TargetSpriteBatches.Gameplay;

        [Slider(0,1)]
        public float Sort = 0;

        internal void DrawAutoTile(RenderContext render, int x, int y, bool topLeft, bool topRight, bool botLeft, bool botRight, float alpha, Color color, Microsoft.Xna.Framework.Vector3 blend)
        {
            var batch = render.GetSpriteBatch(TargetBatch);
            // Top Left 
            if (!topLeft && !topRight && !botLeft && botRight)
                DrawTile(batch, x, y, 0, 0, alpha, color, blend, 1);

            // Top
            if (!topLeft && !topRight && botLeft && botRight)
                DrawTile(batch, x, y, 1, 0, alpha, color, blend, 1);

            // Top Right
            if (!topLeft && !topRight && botLeft && !botRight)
                DrawTile(batch, x, y, 2, 0, alpha, color, blend, 1);

            // Left 
            if (!topLeft && topRight && !botLeft && botRight)
                DrawTile(batch, x, y, 0, 1, alpha, color, blend);

            // Full tile
            if (topLeft && topRight && botLeft && botRight)
                DrawTile(batch, x, y, 1, 1, alpha, color, blend);

            // Right
            if (topLeft && !topRight && botLeft && !botRight)
                DrawTile(batch, x, y, 2, 1, alpha, color, blend);

            // Bottom Left 
            if (!topLeft && topRight && !botLeft && !botRight)
                DrawTile(batch, x, y, 0, 2, alpha, color, blend);

            // Bottom
            if (topLeft && topRight && !botLeft && !botRight)
                DrawTile(batch, x, y, 1, 2, alpha, color, blend);

            // Bottom Right
            if (topLeft && !topRight && !botLeft && !botRight)
                DrawTile(batch, x, y, 2, 2, alpha, color, blend);

            // Top Left Inside Corner
            if (topLeft && topRight && botLeft && !botRight)
                DrawTile(batch, x, y, 1, 3, alpha, color, blend, -1);

            // Top Right Inside Corner
            if (topLeft && topRight && !botLeft && botRight)
                DrawTile(batch, x, y, 2, 3, alpha, color, blend, -1);

            // Top Left Inside Corner
            if (topLeft && !topRight && botLeft && botRight)
                DrawTile(batch, x, y, 1, 4, alpha, color, blend, 1);

            // Top Right Inside Corner
            if (!topLeft && topRight && botLeft && botRight)
                DrawTile(batch, x, y, 2, 4, alpha, color, blend, 1);

            // Diagonal Down Up
            if (topLeft && !topRight && !botLeft && botRight)
                DrawTile(batch, x, y, 0, 3, alpha, color, blend);

            // Diagonal Up Down
            if (!topLeft && topRight && botLeft && !botRight)
                DrawTile(batch, x, y, 0, 4, alpha, color, blend);
        }

        internal void DrawTile(Batch2D batch, int x, int y, int tileX, int tileY, float alpha, Color color, Microsoft.Xna.Framework.Vector3 blend, float sortAdjust = 0)
        {
            var ase = Game.Data.GetAsset<AsepriteAsset>(Image);

            var noise = NoiseHelper.GustavsonNoise(x, y, false, true);
            var texture = Game.Data.FetchAtlas(AtlasId.Gameplay).Get(ase.Frames[Calculator.RoundToInt(noise * (ase.Frames.Length - 1))]);

            texture.Draw(batch, new Vector2(x - Offset.X, y - Offset.Y),
                new Rectangle(tileX * Size.X, tileY * Size.Y, Size.X, Size.Y),
                color * alpha, RenderServices.YSort(y + Grid.HalfCell + Sort*0.1f + sortAdjust * 8 + YSortOffset), blend);
        }

        /// <summary>
        /// Creates a new texture 2D from the graphics device.
        /// </summary>
        public Texture2D CreatePreviewImage()
        {
            RenderTarget2D target = new(Game.GraphicsDevice, Size.X * 2, Size.Y * 2);

            Game.GraphicsDevice.SetRenderTarget(target);
            Game.GraphicsDevice.Clear(Color.Transparent);

            Batch2D batch = new(Game.GraphicsDevice);
            batch.Begin(
                Game.Data.ShaderSprite,
                batchMode: BatchMode.DepthSortDescending,
                blendState: BlendState.AlphaBlend,
                sampler: SamplerState.PointClamp,
                depthStencil: DepthStencilState.DepthRead);

            DrawTile(batch, 0, 0, 0, 0, 1, Color.White, RenderServices.BLEND_NORMAL);
            DrawTile(batch, Size.X, 0, 2, 0, 1, Color.White, RenderServices.BLEND_NORMAL);
            DrawTile(batch, 0, Size.Y, 0, 2, 1, Color.White, RenderServices.BLEND_NORMAL);
            DrawTile(batch, Size.X, Size.Y, 2, 2, 1, Color.White, RenderServices.BLEND_NORMAL);

            batch.End();
            batch.Dispose();

            Game.GraphicsDevice.SetRenderTarget(null);
            return target;
        }
    }
}