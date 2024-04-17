using Bang.Contexts;
using Murder.Core.Graphics;
using Bang.Systems;
using Murder.Components;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets.Graphics;
using Bang.Entities;
using Murder.Core;
using Murder.Utilities;
using Murder.Services;
using System.Collections.Immutable;
using System.Numerics;
using Murder.Core.Geometry;
using System.Diagnostics;
using Murder.Data;

namespace Murder.Systems;

[Filter(filter: ContextAccessorFilter.AnyOf, kind: ContextAccessorKind.Read, typeof(TileGridComponent))]
public class FloorRenderSystem : IMurderRenderSystem, IExitSystem
{
    private struct FloorChunk(int id, Vector2 position)
    {
        public int Id = id;
        public Vector2 Position = position;
    }

    private static int TileChunkSize => 16;
    
    // Cache
    TilesetAsset[]? _tilesetAssetsCache = null;

    private static readonly RuntimeAtlas _atlas = new("Floor System Cache", new Point(4096), new Point(TileChunkSize * Grid.CellSize));
    private readonly Dictionary<int, FloorChunk> _chunks = new();
    private readonly HashSet<int> _chunksToDraw = new();

    public FloorRenderSystem()
    {
    }

    public void Draw(RenderContext render, Context context)
    {
        if (context.World.TryGetUnique<TilesetComponent>() is not TilesetComponent tilesetComponent)
        {
            // Skip drawing on empty.
            return;
        }

        _chunksToDraw.Clear();

        if (_tilesetAssetsCache == null)
        {
            _tilesetAssetsCache = tilesetComponent.Tilesets.ToAssetArray<TilesetAsset>();
        }

        // Loop through all chunks that intersect with the camera bounds
        int minX, maxX, minY, maxY;
        if (context.HasAnyEntity)
        {
            IntRectangle cameraGrid = new IntRectangle(
                Calculator.FloorToInt(render.Camera.Bounds.X / Grid.CellSize),
                Calculator.FloorToInt(render.Camera.Bounds.Y / Grid.CellSize),
                Calculator.CeilToInt(render.Camera.Bounds.Width / Grid.CellSize),
                Calculator.CeilToInt(render.Camera.Bounds.Height / Grid.CellSize)
                ).Expand(+2);

            // Debug rectangle of camera bounds
            RenderServices.DrawRectangleOutline(render.DebugBatch, cameraGrid * Grid.CellSize, Color.Orange);

            (minX, minY, maxX, maxY) = (cameraGrid.X, cameraGrid.Y, cameraGrid.X + cameraGrid.Width, cameraGrid.Y + cameraGrid.Height);
        }
        else
        {
            // Skip drawing on empty.
            return;
        }
        int minChunkX = Calculator.FloorToInt(minX / (float)TileChunkSize);
        int minChunkY = Calculator.FloorToInt(minY / (float)TileChunkSize);
        int maxChunkX = Calculator.FloorToInt(maxX / (float)TileChunkSize);
        int maxChunkY = Calculator.FloorToInt(maxY / (float)TileChunkSize);

        {// Choose which chunks to draw.
            for (int y = minChunkY; y <= maxChunkY; y++)
            {
                for (int x = minChunkX; x <= maxChunkX; x++)
                {
                    int index = x + y * TileChunkSize;
                    if (!_chunks.ContainsKey(index) || !_atlas.HasCache(_chunks[index].Id))
                    {
                        _chunks[index] = new(CreateChunk(x, y, context.Entities, tilesetComponent), new Vector2(x,y));
                    }
                    _chunksToDraw.Add(index);
                }
            }
        }

        // Draw the requested chunks.
        foreach (int index in _chunksToDraw)
        {
            var chunk = _chunks[index];
            _atlas.Draw(chunk.Id, render.FloorBatch, chunk.Position * TileChunkSize * Grid.CellSize, new DrawInfo(RenderServices.YSort((chunk.Position.Y - 2) * Grid.CellSize)));
        }
    }

    private int CreateChunk(int atX, int atY, ImmutableArray<Entity> entities, TilesetComponent tilesetComponent)
    {
        Debug.Assert(_tilesetAssetsCache != null);

        int id = _atlas.PlaceChunk();
        var batch = _atlas.Begin(id);

        // For each room in the world
        foreach (Entity e in entities)
        {
            if (tilesetComponent.Tilesets.IsEmpty)
            {
                // Nothing to be drawn.
                continue;
            }

            var gridComponent = e.GetTileGrid();
            TileGrid grid = e.GetTileGrid().Grid;

            int minX = atX * TileChunkSize;
            int minY = atY * TileChunkSize;
            int maxX = minX + TileChunkSize;
            int maxY = minY + TileChunkSize;

            Point offset = new Point(minX, minY);

            // Clamp to the grid bounds
            minX = Math.Max(minX, gridComponent.Rectangle.X);
            minY = Math.Max(minY, gridComponent.Rectangle.Y);
            maxX = Math.Min(maxX, gridComponent.Rectangle.Right);
            maxY = Math.Min(maxY, gridComponent.Rectangle.Bottom);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    for (int i = 0; i < _tilesetAssetsCache.Length; ++i)
                    {
                        var tilesetAsset = _tilesetAssetsCache[i];
                        if (tilesetAsset == null)
                            continue;

                        if (tilesetAsset.TargetBatch != Batches2D.FloorBatchId)
                            continue;

                        if (tilesetComponent.Tilesets.IsEmpty ||
                            e.TryGetRoom()?.Floor is not Guid floorGuid ||
                            Game.Data.TryGetAsset<FloorAsset>(floorGuid) is not FloorAsset floorAsset)
                        {
                            // Nothing to be drawn.
                            continue;
                        }
                        SpriteAsset floorSpriteAsset = floorAsset.Image.Asset;
                        Texture2D[] floorSpriteAtlas = Game.Data.FetchAtlas(floorSpriteAsset.Atlas).Textures;

                        var tile = grid.GetTile(entities, i, _tilesetAssetsCache.Length, x - grid.Origin.X, y - grid.Origin.Y);

                        IntRectangle rectangle = XnaExtensions.ToRectangle(
                            (x - offset.X) * Grid.CellSize, (y - offset.Y) * Grid.CellSize, Grid.CellSize, Grid.CellSize);

                        // Draw the individual tiles
                        if (tile.tile >= 0)
                        {
                            tilesetAsset.DrawTile(batch,
                                rectangle.X - Grid.HalfCellSize, rectangle.Y - Grid.HalfCellSize,
                                tile.tile % 3, Calculator.FloorToInt(tile.tile / 3f),
                                1f, Color.White,
                                RenderServices.BLEND_NORMAL, tile.sortAdjust);
                        }

                        // Draw the actual floor
                        if (floorSpriteAsset is not null && x < maxX && y < maxY)
                        {
                            ImmutableArray<int> floorFrames = floorSpriteAsset.Animations[string.Empty].Frames;

                            var noise = Calculator.RoundToInt(NoiseHelper.Simple2D(x, y) * (floorFrames.Length - 1));
                            AtlasCoordinates floor = floorSpriteAsset.GetFrame(floorFrames[noise]);

                            // Draw each individual ground tile.
                            batch.Draw(
                                floorSpriteAtlas[floor.AtlasIndex],
                                (new Point(x, y) - offset) * Grid.CellSize,
                                floor.Size,
                                floor.SourceRectangle,
                                1,
                                0,
                                Vector2.One,
                                ImageFlip.None,
                                Color.White,
                                Vector2.Zero,
                                RenderServices.BLEND_NORMAL
                                );
                        }
                    }
                }
            }
        }
        RenderServices.DrawText(batch, MurderFonts.PixelFont, id.ToString(), new Vector2(0, 0), new DrawInfo(0));
        _atlas.End();

        return id;
    }

    public void Exit(Context context)
    {
        _chunks.Clear();
        _atlas.Cleanup();
    }
}