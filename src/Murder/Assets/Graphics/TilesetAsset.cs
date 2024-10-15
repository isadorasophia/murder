using Murder.Attributes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets.Graphics
{
    public class TilesetAsset : GameAsset
    {
        public override char Icon => '\uf84c';
        public override string EditorFolder => "#Tilesets";

        [GameAssetId(typeof(SpriteAsset))]
        public readonly Guid Image = Guid.Empty;

        public readonly Point Offset = new();
        public readonly Point Size = new();

        public readonly int YSortOffset = 0;

        public readonly bool ConsiderOutsideOccupied = false;

        [Tooltip("Whether this tile has a collision or not.")]
        [CollisionLayer]
        public readonly int CollisionLayer;

        [Tooltip("Properties of this tileset.")]
        [Default("Add properties")]
        public readonly ITileProperties? Properties = null;

        /// <summary>
        /// This is the order (or layer) which this tileset will be drawn into the screen.
        /// </summary>
        public readonly int Order = new();

        [SpriteBatchReference]
        public int TargetBatch = Batches2D.GameplayBatchId;

        [GameAssetId<TilesetAsset>]
        public readonly ImmutableArray<Guid> AdditionalTiles = ImmutableArray<Guid>.Empty;

        [Slider(0, 1)]
        public float Sort = 0;

        public T? GetProperties<T>() where T : notnull, ITileProperties
        {
            if (Properties is not T tValue)
            {
                return default;
            }

            return tValue;
        }
        
        public void CalculateAndDrawAutoTile(RenderContext render, int x, int y, bool topLeft, bool topRight, bool botLeft, bool botRight, float alpha, Color color, Microsoft.Xna.Framework.Vector3 blend)
        {
            var batch = render.GetBatch((int)TargetBatch);
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
                DrawTile(batch, x, y, 1, 3, alpha, color, blend, 0);

            // Top Right Inside Corner
            if (topLeft && topRight && !botLeft && botRight)
                DrawTile(batch, x, y, 2, 3, alpha, color, blend, 0);

            // Bottom Left Inside Corner
            if (topLeft && !topRight && botLeft && botRight)
                DrawTile(batch, x, y, 1, 4, alpha, color, blend, 1);

            // Bottom Right Inside Corner
            if (!topLeft && topRight && botLeft && botRight)
                DrawTile(batch, x, y, 2, 4, alpha, color, blend, 1);

            // Diagonal Down Up
            if (topLeft && !topRight && !botLeft && botRight)
                DrawTile(batch, x, y, 0, 3, alpha, color, blend);

            // Diagonal Up Down
            if (!topLeft && topRight && botLeft && !botRight)
                DrawTile(batch, x, y, 0, 4, alpha, color, blend);
        }

        public void DrawTile(Batch2D batch, int x, int y, int tileX, int tileY, float alpha, Color color, Microsoft.Xna.Framework.Vector3 blend, float sortAdjust = 0)
        {
            var ase = Game.Data.GetAsset<SpriteAsset>(Image);

            var noise = NoiseHelper.GustavsonNoise(x, y, false, true);
            var texture = ase.Frames[Calculator.RoundToInt(noise * (ase.Frames.Length - 1))];
            float sort = RenderServices.YSort(y + Grid.HalfCellSize + Sort * 0.1f + sortAdjust * 8 + YSortOffset);

            texture.Draw(batch, new Vector2(x - Offset.X, y - Offset.Y),
                new Rectangle(tileX * Size.X, tileY * Size.Y, Size.X, Size.Y),
                color * alpha, Vector2.One, 0, Vector2.Zero, ImageFlip.None, blend, sort);
        }
    }
}