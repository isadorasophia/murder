using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
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
using System.Numerics;

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

        bool populatePathfindMap = false;

        if (context.World.TryGetUnique<PathfindMapComponent>() is not PathfindMapComponent pathfindMap)
        {
            Map map = context.World.GetUniqueMap().Map;

            pathfindMap = new(map.Width, map.Height);
            pathfindMap.Map.ZeroAll();

            context.World.AddEntity(pathfindMap);

            populatePathfindMap = true;
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
            if (populatePathfindMap)
            {
                pathfindMap.Map.OverrideValueAt(cell.Point, cell.CollisionMask, cell.Weight);
            }

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
            map.OverrideValueAt(cell.Point, cell.CollisionMask, cell.Weight);

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
            map.OverrideValueAt(cell.Point, cell.CollisionMask, cell.Weight);

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
        Map pathMap = context.World.GetUnique<PathfindMapComponent>().Map;
        Map? map = context.World.TryGetUniqueMap()?.Map;

        Point cursor = _cachedRectangle.TopLeft;
        int mask = pathMap.At(cursor.X, cursor.Y);
        int mapMask = map?.At(cursor.X, cursor.Y) ?? 0;

        if (ImGui.BeginTooltip())
        {
            ImGui.Text($"Point: {cursor}");
            ImGui.Separator();

            Vector4 pathColor = Game.Profile.Theme.Green;
            Vector4 mapColor = Game.Profile.Theme.Accent;

            foreach ((string name, int id) in AssetsFilter.CollisionLayers)
            {
                if ((mask & id) != 0)
                {
                    ImGui.TextColored(mapColor, name);
                }

                if ((mapMask & id) != 0)
                {
                    ImGui.TextColored(mapColor, name);
                }
            }

            ImGui.EndTooltip();
        }
    }

    public void Draw(RenderContext render, Context context)
    {
        Color color = Game.Profile.Theme.White;
        color *= .5f;

        RenderServices.DrawRectangleOutline(render.DebugBatch, (_cachedRectangle * Grid.CellSize).Expand(
            4 - 3 * Ease.ZeroToOne(Ease.CubeInOut, 0.250f, _tweenStart)), color);
    }
}