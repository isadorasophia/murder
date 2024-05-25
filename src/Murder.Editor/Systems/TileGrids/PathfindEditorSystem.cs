using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Core.Physics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.Systems;

[PathfindEditor]
[Filter(typeof(PathfindGridComponent))]
public class PathfindEditorSystem : IStartupSystem, IUpdateSystem, IGuiSystem, IMurderRenderSystem
{
    private float _tweenStart;

    /// <summary>
    /// Tracks all the coordinates available in <see cref="CellProperties.Point"/>.
    /// </summary>
    private Dictionary<Point, CellProperties>? _coordinates = null;

    private IntRectangle _cachedRectangle = new();

    public void Start(Context context)
    {
        _coordinates = [];

        if (context.World.TryGetUnique<PathfindMapComponent>() is null)
        {
            Map map = context.World.GetUniqueMap().Map;

            PathfindMapComponent pathfindMap = new(map.Width, map.Height);
            pathfindMap.Map.ZeroAll();

            context.World.AddEntity(pathfindMap);
        }

        if (!context.HasAnyEntity)
        {
            // If there is no entity, make sure we have one before operating over this system.
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            hook.AddEntityWithStage?.Invoke(
                [new PathfindGridComponent()], 
                /* group: */ null, 
                /* name: */ "Pathfind Grid Entity");

            return;
        }

        // Manually track all the cell properties currently defined.
        ImmutableArray<CellProperties> cells = context.Entity.GetPathfindGrid().Cells;
        foreach (CellProperties cell in cells)
        {
            _coordinates[cell.Point] = cell;
        }
    }

    public void Update(Context context)
    {
        System.Diagnostics.Debug.Assert(_coordinates is not null);

        EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
        if (!hook.IsMouseOnStage)
        {
            return;
        }

        if (hook.CursorWorldPosition is not Point cursorWorldPosition)
        {
            return;
        }

        if (Game.Input.Pressed(MurderInputButtons.LeftClick))
        {
            // cute
            _tweenStart = Game.Now;
        }

        Point cursorGridPosition = cursorWorldPosition.FromWorldToLowerBoundGridPosition();
        _cachedRectangle = new Rectangle(cursorGridPosition.X, cursorGridPosition.Y, 1, 1);

        bool modified = false;

        if (Game.Input.Down(MurderInputButtons.LeftClick))
        {
            Map map = context.World.GetUnique<PathfindMapComponent>().Map;

            if (!_coordinates.TryGetValue(cursorGridPosition, out CellProperties cell))
            {
                cell = new(cursorGridPosition);
            }

            cell = cell with { Weight = hook.CurrentPathfindWeight, CollisionMask = hook.CurrentPathfindCollisionMask };
            map.SetAt(cell.Point, cell.CollisionMask, cell.Weight);

            _coordinates[cursorGridPosition] = cell;
            modified = true;
        }
        else if (Game.Input.Down(MurderInputButtons.RightClick))
        {
            Map map = context.World.GetUnique<PathfindMapComponent>().Map;

            if (!_coordinates.TryGetValue(cursorGridPosition, out CellProperties cell))
            {
                cell = new(cursorGridPosition);
            }

            cell = cell with { Weight = 0, CollisionMask = CollisionLayersBase.NONE };
            map.SetAt(cell.Point, cell.CollisionMask, cell.Weight);

            _coordinates[cursorGridPosition] = cell;
            modified = true;
        }

        if (modified)
        {
            context.Entity.SetPathfindGrid([.. _coordinates.Values]);
        }
    }

    public void DrawGui(RenderContext render, Context context)
    {
    }

    public void Draw(RenderContext render, Context context)
    {
        Color color = Game.Profile.Theme.White;
        color *= .5f;

        RenderServices.DrawRectangleOutline(render.DebugBatch, (_cachedRectangle * Grid.CellSize).Expand(
            4 - 3 * Ease.ZeroToOne(Ease.CubeInOut, 0.250f, _tweenStart)), color);
    }
}