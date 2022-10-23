using Murder.Attributes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Assets.Graphics
{
    internal class TilesetAsset : GameAsset
    {
        public override char Icon => '\uf84c';
        public override string EditorFolder => "#Tilesets";

        [GameAssetId(typeof(AsepriteAsset))]
        public Guid Image = Guid.Empty;

        public Point offset = new();
        public Point tileSize = new();

        internal void DrawAutoTile(Batch2D batch, int x, int y, bool topLeft, bool topRight, bool botLeft, bool botRight, float alpha, Color color, Vector3 blend)
        {
            // Top Left 
            if (!topLeft && !topRight && !botLeft && botRight)
                DrawTile(batch, x, y, 0, 0, alpha, color, blend);

            // Top
            if (!topLeft && !topRight && botLeft && botRight)
                DrawTile(batch, x, y, 1, 0, alpha, color, blend);

            // Top Right
            if (!topLeft && !topRight && botLeft && !botRight)
                DrawTile(batch, x, y, 2, 0, alpha, color, blend);

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
                DrawTile(batch, x, y, 1, 3, alpha, color, blend);

            // Top Right Inside Corner
            if (topLeft && topRight && !botLeft && botRight)
                DrawTile(batch, x, y, 2, 3, alpha, color, blend);

            // Top Left Inside Corner
            if (topLeft && !topRight && botLeft && botRight)
                DrawTile(batch, x, y, 1, 4, alpha, color, blend);

            // Top Right Inside Corner
            if (!topLeft && topRight && botLeft && botRight)
                DrawTile(batch, x, y, 2, 4, alpha, color, blend);

            // Diagonal Down Up
            if (topLeft && !topRight && !botLeft && botRight)
                DrawTile(batch, x, y, 0, 3, alpha, color, blend);

            // Diagonal Up Down
            if (!topLeft && topRight && botLeft && !botRight)
                DrawTile(batch, x, y, 0, 4, alpha, color, blend);
        }

        internal void DrawTile(Batch2D batch, int x, int y, int tileX, int tileY, float alpha, Color color, Vector3 blend)
        {
            var ase = Game.Data.GetAsset<AsepriteAsset>(Image);

            var noise = NoiseHelper.GustavsonNoise(x, y, false, true);
            var texture = Game.Data.FetchAtlas(AtlasId.Gameplay).Get(ase.Frames[Calculator.RoundToInt(noise * (ase.Frames.Length - 1))]);

            texture.Draw(batch, new Vector2(x - offset.X, y - offset.Y),
                new Rectangle(tileX * tileSize.X, tileY * tileSize.Y, tileSize.X, tileSize.Y),
                color.WithAlpha(color.A * alpha), RenderServices.YSort(y + Grid.HalfCell), blend);
        }
    }
}