using Bang;
using Bang.Contexts;
using Bang.Systems;
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

namespace Murder.Editor.Systems
{
    [OnlyShowOnDebugView]
    [WorldEditor(startActive: true)]
    [PathfindEditor]
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
                    Rectangle tileRectangle = XnaExtensions.ToRectangle(
                        x * Grid.CellSize - Grid.HalfCellSize, y * Grid.CellSize - Grid.HalfCellSize, Grid.CellSize, Grid.CellSize);

                    Rectangle cellRectangle = new Rectangle(x * Grid.CellSize, y * Grid.CellSize, Grid.CellSize, Grid.CellSize);

                    int topLeft = map.GetCollision(x - 1, y - 1);
                    int topRight = map.GetCollision(x, y - 1);
                    int botLeft = map.GetCollision(x - 1, y);
                    int botRight = map.GetCollision(x, y);

                    int collisionMask = map.GetGridMap(x, y).CollisionType;
                    bool hasTileCollision = IsSolid(collisionMask);

                    Color solidGridColor = new Color(.1f, .9f, .9f) * .4f;
                    DrawTileCollisions(topLeft, topRight, botLeft, botRight, render, tileRectangle, solidGridColor);

                    Color color = ColorForTileMask(collisionMask);
                    if (!hasTileCollision)
                    {
                        DrawCarveCollision(botRight, render, cellRectangle, color);
                    }

                    int weight = map.WeightAt(x, y);
                    if (pathfindMap is not null)
                    {
                        int collisionMaskForPathfind = pathfindMap.GetCollision(x, y);
                        Color solidPathfindColor = ColorForPathfindTileMask(collisionMaskForPathfind);

                        int pathfind = pathfindMap.GetCollision(x, y);
                        DrawCarveCollision(pathfind, render, cellRectangle, solidPathfindColor);

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

        private Color ColorForTileMask(int mask)
        {
            if (IsSolid(mask))
            {
                return new Color(.1f, .9f, .9f) * .4f;
            }

            if (IsBlockingLineOfSight(mask))
            {
                return new Color(.9f, .2f, .8f) * .4f;
            }

            return new Color(.2f, .2f, .2f) * .1f;
        }

        private Color ColorForPathfindTileMask(int mask)
        {
            if (IsSolid(mask))
            {
                return new Color(.2f, 1, .7f) * .7f;
            }

            if (IsBlockingLineOfSight(mask))
            {
                return new Color(.2f, 1, .7f) * .3f;
            }

            return new Color(.2f, 1, .7f) * .1f;
        }

        private bool IsSolid(int mask) =>
            (mask & CollisionLayersBase.SOLID) != 0;

        private bool IsBlockingLineOfSight(int mask) =>
            (mask & CalculatePathfindSystem.LineOfSightCollisionMask) != 0;

        private void DrawTileCollisions(int topLeft, int topRight, int botLeft, int botRight,
            RenderContext render, Rectangle rectangle, Color color)
        {
            float sorting = 1;
            int mask = CollisionLayersBase.SOLID;

            if ((topLeft & mask) != 0)
            {
                render.DebugBatch.DrawRectangle(new Rectangle(rectangle.X, rectangle.Y, Grid.HalfCellSize, Grid.HalfCellSize), color, sorting);
            }

            if ((topRight & mask) != 0)
            {
                render.DebugBatch.DrawRectangle(new Rectangle(rectangle.X + Grid.HalfCellSize, rectangle.Y, Grid.HalfCellSize, Grid.HalfCellSize), color, sorting);
            }

            if ((botLeft & mask) != 0)
            {
                render.DebugBatch.DrawRectangle(new Rectangle(rectangle.X, rectangle.Y + Grid.HalfCellSize, Grid.HalfCellSize, Grid.HalfCellSize), color, sorting);
            }

            if ((botRight & mask) != 0)
            {
                render.DebugBatch.DrawRectangle(new Rectangle(rectangle.X + Grid.HalfCellSize, rectangle.Y + Grid.HalfCellSize, Grid.HalfCellSize, Grid.HalfCellSize), color, sorting);
            }
        }

        private void DrawCarveCollision(int cell, RenderContext render, Rectangle rectangle, Color color)
        {
            float sorting = 1;
            int mask = CollisionLayersBase.CARVE;

            if ((cell & mask) != 0)
            {
                render.DebugBatch.DrawRectangle(rectangle, color, sorting);
            }
        }
    }
}