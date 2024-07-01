using Bang;
using Bang.Contexts;
using Bang.Systems;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Services;
using Murder.Systems;
using Murder.Utilities;
using System.Diagnostics;

namespace Murder.Editor.Systems
{
    [WorldEditor(startActive: true)]
    [PathfindEditor]
    [TileEditor]
    [OnlyShowOnDebugView]
    [Filter(kind: ContextAccessorKind.Read, typeof(MapComponent))]
    internal class DebugMapRenderSystem : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            var editorHook = context.World.GetUnique<EditorComponent>().EditorHook;

            if (!editorHook.DrawGrid)
            {
                return;
            }

            Map? map = context.World.TryGetUniqueMap()?.Map;
            if (map is null)
            {
                return;
            }

            Map? pathfindMap = context.World.TryGetUnique<PathfindMapComponent>()?.Map;

            (int minX, int maxX, int minY, int maxY) = render.Camera.GetSafeGridBounds(map);
            Color gridColor = Color.CreateFrom256(r: 170, g: 227, b: 12) * .1f;

            for (int y = minY; y < maxY; y++)
            {
                RenderServices.DrawLine(
                    render.DebugBatch,
                    new Point(minX, y) * Grid.CellSize,
                    new Point(maxX, y) * Grid.CellSize,
                    gridColor);

                for (int x = minX; x < maxX; x++)
                {
                    Rectangle cellRectangle = new Rectangle(x * Grid.CellSize, y * Grid.CellSize, Grid.CellSize, Grid.CellSize);

                    int mask = map.GetCollision(x, y);

                    int collisionMask = map.GetGridMap(x, y).CollisionType;
                    bool hasTileCollision = IsSolid(collisionMask);

                    int weight = map.WeightAt(x, y);
                    if (pathfindMap is not null)
                    {
                        int collisionMaskForPathfind = pathfindMap.GetCollision(x, y);
                        mask |= pathfindMap.GetCollision(x, y);
                        weight += pathfindMap.WeightAt(x, y);

                        hasTileCollision |= IsSolid(collisionMaskForPathfind);
                    }

                    if (editorHook.DrawPathfind && !hasTileCollision)
                    {
                        Color numberColor = ColorForTileWeight(weight);

                        RenderServices.DrawText(render.DebugBatch, MurderFonts.PixelFont, $"{weight}",
                            cellRectangle.TopLeft + Grid.HalfCellDimensions,
                            new DrawInfo(0)
                            {
                                Origin = new(0.5f, 0.5f),
                                Color = numberColor,
                            });
                    }

                    DrawTileCollisions(mask, render, cellRectangle);
                }
            }

            for (int x = minX; x < maxX; x++)
            {
                RenderServices.DrawLine(
                    render.DebugBatch,
                    new Point(x, minY) * Grid.CellSize,
                    new Point(x, maxY) * Grid.CellSize,
                    gridColor);
            }
        }

        private Color ColorForTileWeight(int weight)
        {
            if (weight == 1)
            {
                return Color.CreateFrom256(123, 133, 208);
            }

            if (weight > 101)
            {
                return Color.CreateFrom256(248, 91, 131);
            }

            return Color.CreateFrom256(255, 159, 255);
        }

        private bool IsSolid(int mask) =>
            (mask & CollisionLayersBase.SOLID) != 0;
        private bool IsHole(int mask) =>
                    (mask & CollisionLayersBase.HOLE) != 0;

        private bool IsBlockingLineOfSight(int mask) =>
            (mask & CollisionLayersBase.BLOCK_VISION) != 0;

        private void DrawTileCollisions(int mask, RenderContext render, Rectangle rectangle)
        {
            float sorting = 1;
            Color solidGridColor = new Color(.1f, .9f, .9f) * .4f;
            Color carveGridColor = new Color(.1f, .9f, .3f) * .2f;
            Color otherGridColor = new Color(.1f, .1f, .9f) * .2f;
            Color holeGridColor = new Color(.9f, .5f, .1f) * .4f;

            if (IsSolid(mask))
            {
                render.DebugBatch.DrawRectangle(rectangle, solidGridColor, sorting);
            }
            else if ((mask & CollisionLayersBase.CARVE) != 0)
            {
                render.DebugBatch.DrawRectangle(rectangle, carveGridColor, sorting);
            }
            else if (IsBlockingLineOfSight(mask))
            {
                float padding = Grid.CellSize * 0.1f;
                render.DebugBatch.DrawRectangleOutline(rectangle.Expand(-padding), carveGridColor, 1, sorting);
            }
            else if (mask >= (1 << 9))
            {
                // Any masks that are not defined in murder.
                render.DebugBatch.DrawRectangle(rectangle, otherGridColor, sorting);
            }

            if (IsHole(mask))
            {
                render.DebugBatch.DrawRectangle(rectangle, holeGridColor, sorting);
            }

        }
    }
}