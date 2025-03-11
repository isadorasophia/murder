using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Murder.Attributes;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Components;
using Murder.Editor.Core;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Murder.Editor.Systems
{
    [TileEditor]
    [Filter(typeof(TileGridComponent))]
    public partial class TileEditorSystem : IUpdateSystem, IMurderRenderSystem
    {
        public void Update(Context context)
        {
            if (context.World.TryGetUnique<EditorComponent>()?.EditorHook is not EditorHook hook)
            {
                return;
            }

            if (!Game.Instance.IsActive)
            {
                return;
            }

            if (hook.CursorWorldPosition is null)
            {
                return;
            }

            _inputAvailable = true;

            if (hook.CursorIsBusy.Count != 0 || hook.UsingGui)
            {
                // Someone else is using our cursor, let's wait out turn.
                _inputAvailable = false;
            }

            UpdateToolbox(((MonoWorld)context.World).Camera, hook);

            bool isCursorBusy = _downOnToolboxArea || _hoveredOnToolboxArea;
            if (isCursorBusy)
            {
                return;
            }

            if (_editorMode == EditorMode.Cut)
            {
                // update here?
            }
            else
            {
                foreach (Entity e in context.Entities)
                {
                    TileGridComponent gridComponent = e.GetTileGrid();

                    TileGrid grid = gridComponent.Grid;
                    Point position = gridComponent.Origin;

                    if (_dragModeAvailable)
                    {
                        UpdateDragRoom(e, hook.CursorWorldPosition.Value, position, grid);
                    }

                    if (_isPaintTileAvailable)
                    {
                        UpdatePaintTile(hook, e, gridComponent);
                    }
                }

                if (Game.Input.Released(MurderInputButtons.LeftClick) || Game.Input.Released(MurderInputButtons.RightClick))
                {
                    _startedShiftDragging = null;
                }
            }
        }

        private void UpdateDragRoom(Entity e, Point cursorWorldPosition, Point position, TileGrid grid)
        {
            IntRectangle gridRectangle = new Rectangle(position.X, position.Y, grid.Width, grid.Height);
            Rectangle worldRectangle = gridRectangle * Grid.CellSize;

            int id = e.EntityId;
            if (EditorServices.DragArea(
                $"drag_{id}", cursorWorldPosition, worldRectangle, out Vector2 newDragWorldTopLeft))
            {
                Point newGridTopLeft = newDragWorldTopLeft.ToGridPoint();

                // Clamp at zero.
                newGridTopLeft.X = Math.Max(newGridTopLeft.X, 0);
                newGridTopLeft.Y = Math.Max(newGridTopLeft.Y, 0);

                _resize = new(position: newGridTopLeft, gridRectangle.Size);
                _targetEntity = id;
            }
        }

        private void UpdatePaintTile(EditorHook hook, Entity e, TileGridComponent grid)
        {
            if (hook.CursorWorldPosition is not Point cursorWorldPosition)
            {
                return;
            }

            Point lowerBoundGridPosition = cursorWorldPosition.FromWorldToLowerBoundGridPosition();
            if (FitCursorPositionToGrid(grid, lowerBoundGridPosition) is not Point cursorGridPosition)
            {
                return;
            }

            // We are actually applying operation over an area.
            if (_startedShiftDragging == null &&
                Game.Input.Down(MurderInputButtons.Shift) &&
                (Game.Input.Down(MurderInputButtons.LeftClick) || Game.Input.Down(MurderInputButtons.RightClick)))
            {
                _targetEntity = e.EntityId;

                // Start tracking the origin.
                _startedShiftDragging = cursorGridPosition;
                _currentRectDraw = GridHelper.FromTopLeftToBottomRight(_startedShiftDragging.Value, cursorGridPosition) * Grid.CellSize;
                _dragColor = Game.Input.Down(MurderInputButtons.LeftClick) ? Color.Green * .1f : Color.Red * .05f;
                _tweenStart = Game.Now;
            }

            int selectedTileMask = hook.CurrentSelectedTile.ToMask();

            if (_startedShiftDragging != null && _dragColor != null)
            {
                // Draw the gridRectangle applied over the area.
                IntRectangle draggedRectangle = GridHelper.FromTopLeftToBottomRight(_startedShiftDragging.Value, cursorGridPosition);

                if (grid.Rectangle.Contains(lowerBoundGridPosition))
                {
                    Rectangle targetSize = draggedRectangle * Grid.CellSize;
                    _currentRectDraw = Rectangle.Lerp(_currentRectDraw, targetSize, 0.45f);
                }

                if (Game.Input.Released(MurderInputButtons.LeftClick))
                {
                    grid.Grid.SetGridPosition(draggedRectangle, selectedTileMask);
                }
                else if (Game.Input.Released(MurderInputButtons.RightClick))
                {
                    grid.Grid.UnsetGridPosition(draggedRectangle, selectedTileMask);
                }

                return;
            }

            // We are applying an operation over an individual tile.
            if (!hook.IsMouseOnStage)
            {
                return;
            }

            if (Game.Input.Down(MurderInputButtons.LeftClick))
            {
                if (!grid.Grid.AtGridPosition(cursorGridPosition).HasFlag(selectedTileMask))
                {
                    _tweenStart = Game.Now;

                    // paint individual tile
                    grid.Grid.SetGridPosition(cursorGridPosition, selectedTileMask);
                }
            }
            else if (Game.Input.Down(MurderInputButtons.RightClick))
            {
                if (grid.Grid.AtGridPosition(cursorGridPosition).HasFlag(selectedTileMask))
                {
                    _tweenStart = Game.Now;

                    // remove individual tile
                    grid.Grid.UnsetGridPosition(cursorGridPosition, selectedTileMask);
                }
            }
        }

        public void Draw(RenderContext render, Context context)
        {
            if (context.World.TryGetUnique<EditorComponent>() is not EditorComponent editor)
            {
                return;
            }

            if (!Game.Instance.IsActive)
            {
                return;
            }

            bool isCursorBusy = _downOnToolboxArea || _hoveredOnToolboxArea;

            _dragModeAvailable = false;
            _isPaintTileAvailable = false;

            // Draw the toolbox
            DrawToolbox(render);

            // Whether the cursor if within or interacting with any of the rooms.

            if (!isCursorBusy)
            {
                if (_editorMode == EditorMode.Cut)
                {
                    isCursorBusy |= DrawTileSelector(render, context, editor);
                }
                else
                {
                    foreach (Entity e in context.Entities)
                    {
                        isCursorBusy |= DrawResizeBox(render, context.World, editor, e);
                        isCursorBusy |= DrawTilePainter(render, editor, e);
                    }
                }
                
            }

            if (!isCursorBusy)
            {
                DrawNewRoom(editor);
            }

            return;
        }

        private bool DrawResizeBox(RenderContext render, World world, EditorComponent editor, Entity e)
        {
            TileGridComponent gridComponent = e.GetTileGrid();

            TileGrid grid = gridComponent.Grid;
            Point position = gridComponent.Origin;

            Color color = Game.Profile.Theme.Accent.ToXnaColor();

            IntRectangle rectangle = new Rectangle(position.X, position.Y, grid.Width, grid.Height);
            int lineWidth = Calculator.RoundToInt(2 / render.Camera.Zoom);

            RenderServices.DrawRectangleOutline(render.DebugBatch, rectangle * Grid.CellSize, color, lineWidth);
            RenderServices.DrawRectangleOutline(render.DebugBatch, (rectangle * Grid.CellSize).Expand(lineWidth), Color.Black * .2f, lineWidth);
    
            if (DrawHandles(render, world, editor, e.EntityId, rectangle, color) is IntRectangle newRectangle)
            {
                grid.Resize(newRectangle);

                e.RemoveTileGrid();
                e.SetTileGrid(grid);
            }

            return _resize != null;
        }

        private int _targetEntity = -1;
        private Rectangle? _resize;

        private bool _dragModeAvailable = false;
        private bool _isPaintTileAvailable = false;

        /// <summary>
        /// Draw a box and listen to user input.
        /// </summary>
        /// <param name="render">Render context.</param>
        /// <param name="editor">Editor component.</param>
        /// <param name="id">Unique identifier for this box.</param>
        /// <param name="gridRectangle">Rectangle position in the grid.</param>
        /// <param name="color">Color which will be displayed.</param>
        private IntRectangle? DrawHandles(RenderContext render, World world, EditorComponent editor, int id, IntRectangle gridRectangle, Color color)
        {
            if (_inputAvailable)
            {
                if (_resize is not null && !Game.Input.Down(MurderInputButtons.LeftClick))
                {
                    if (_targetEntity != id)
                    {
                        // We have no business here, this is not the current entity.
                        return default;
                    }

                    if (!Game.Input.Down(MurderInputButtons.LeftClick))
                    {
                        ChangeCursorTo(world, CursorStyle.Normal);

                        // If there was a preview, it seems that it's time to build it!
                        Rectangle result = _resize.Value;

                        _resize = null;
                        return result;
                    }
                }
            }

            Vector2 worldPosition = gridRectangle.TopLeft * Grid.CellSize;
            Vector2 worldBottomRight = gridRectangle.BottomRight * Grid.CellSize;

            int lineWidth = Calculator.RoundToInt(2 / render.Camera.Zoom);

            // Zoom out mode, so we can just drag the whole room.
            if (render.Camera.Zoom < EditorFloorRenderSystem.ZoomThreshold)
            {
                Rectangle worldRectangle = gridRectangle * Grid.CellSize;

                if (_inputAvailable && editor.EditorHook.CursorWorldPosition != null)
                {
                    _dragModeAvailable = true;
                }

                Point offset = new(lineWidth, lineWidth);

                // Draw a cute area within the gridRectangle.
                RenderServices.DrawRectangle(
                    render.DebugFxBatch,
                    new Rectangle(gridRectangle.TopLeft * Grid.CellSize + offset, gridRectangle.Size * Grid.CellSize - offset),
                    Color.White * .5f);

                Point center = gridRectangle.CenterPoint * Grid.CellSize;
                string name = editor.EditorHook.TryGetGroupNameForEntity(id) ?? "Room";

                RenderServices.DrawText(render.DebugBatch, MurderFonts.LargeFont, name, center, new DrawInfo(0f)
                {
                    Origin = new Vector2(.5f, .5f),
                    Color = Color.White,
                    Outline = Color.Black,
                    Scale = new(lineWidth, lineWidth)
                });
            }


            if (!_inputAvailable)
            {
                return null;
            }

            if (editor.EditorHook.CursorWorldPosition is Point cursorPosition)
            {
                // Now, draw the bottom right handle.
                if (EditorServices.DrawHandle($"offset_{id}_BR", render,
                    cursorPosition, position: worldBottomRight, color, out Vector2 newWorldBottomRight))
                {
                    Point gridDelta = newWorldBottomRight.ToGridPoint() - gridRectangle.BottomRight;

                    _resize = new(gridRectangle.TopLeft, gridRectangle.Size + gridDelta);
                    _targetEntity = id;
                }

                // We are fancy and also draw the top left handle.
                if (EditorServices.DrawHandle($"offset_{id}_TL", render,
                    cursorPosition, position: worldPosition, color, out Vector2 newWorldTopLeft))
                {
                    Point gridDelta = newWorldTopLeft.ToGridPoint() - gridRectangle.TopLeft;

                    Point newGridTopLeftPosition = newWorldTopLeft.ToGridPoint();

                    // Clamp at zero.
                    if (newGridTopLeftPosition.X >= 0 && newGridTopLeftPosition.Y >= 0)
                    {
                        _resize = new(newGridTopLeftPosition, gridRectangle.Size - gridDelta);
                        _targetEntity = id;
                    }
                }
            }

            // Let's add the preview to the user.
            if (_resize is not null)
            {
                RenderServices.DrawRectangleOutline(render.DebugBatch, _resize.Value * Grid.CellSize, Color.Green, lineWidth);
                ChangeCursorTo(world, CursorStyle.Hand);
            }

            return default;
        }

        private static void ChangeCursorTo(World world, CursorStyle style)
        {
            if (world.TryGetUnique<EditorComponent>() is EditorComponent editorComponent)
            {
                editorComponent.EditorHook.Cursor = style;
            }
        }

        private Point? _startedShiftDragging;
        private Color? _dragColor;

        private float _tweenStart;
        private Rectangle _currentRectDraw;
        private bool _inputAvailable;

        /// <summary>
        /// This is the logic for capturing input for new tiles.
        /// </summary>
        /// <returns>
        /// Whether the mouse has interacted with this entity.
        /// </returns>
        private bool DrawTilePainter(RenderContext render, EditorComponent editor, Entity e)
        {
            if (!CanDrawCursorGestures(render, e.EntityId))
            {
                return false;
            }

            if (editor.EditorHook.CursorWorldPosition is not Point cursorWorldPosition)
            {
                return false;
            }

            _isPaintTileAvailable = true;

            if (_startedShiftDragging != null && _dragColor != null)
            {
                RenderServices.DrawRectangle(render.DebugBatch, _currentRectDraw, _dragColor.Value);
                RenderServices.DrawRectangleOutline(render.DebugBatch, _currentRectDraw, Color.White * .5f);

                return true;
            }

            Color color = Game.Profile.Theme.White.ToXnaColor();
            color *= .5f;

            TileGridComponent gridComponent = e.GetTileGrid();
            Point lowerBoundGridPosition = cursorWorldPosition.FromWorldToLowerBoundGridPosition();

            if (FitCursorPositionToGrid(gridComponent, lowerBoundGridPosition) is not Point cursorGridPosition)
            {
                return false;
            }

            // Otherwise, we are at classical individual tile selection.
            IntRectangle rectangle = new Rectangle(cursorGridPosition.X, cursorGridPosition.Y, 1, 1);
            RenderServices.DrawRectangleOutline(render.DebugBatch, (rectangle * Grid.CellSize).Expand(4 - 3 * Ease.ZeroToOne(Ease.BackInOut, 0.250f, _tweenStart)), color);

            return true;
        }

        private bool CanDrawCursorGestures(RenderContext render, int entityId)
        {
            if (_resize is not null || render.Camera.Zoom < EditorFloorRenderSystem.ZoomThreshold)
            {
                // We are currently resizing, we have no business building tiles for now.
                return false;
            }

            if (_startedShiftDragging != null && _targetEntity != entityId)
            {
                // we have no business just yet.
                return false;
            }

            if (!_inputAvailable)
            {
                return false;
            }

            return true;        
        }

        /// <summary>
        /// This draws and create a new room if the user prompts with the context menu.
        /// </summary>
        private bool DrawNewRoom(EditorComponent editor)
        {
            ImGui.PushID("Popup!");
            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.Selectable("Add new room!"))
                {
                    Point cursorWorldPosition = editor.EditorHook.LastCursorWorldPosition;
                    Point cursorGridPosition = cursorWorldPosition.FromWorldToLowerBoundGridPosition();
                    Guid defaultFloor = Architect.EditorSettings.DefaultFloor;

                    editor.EditorHook.AddEntityWithStage?.Invoke(
                        [
                            new RoomComponent(defaultFloor),
                            new TileGridComponent(cursorGridPosition, 6, 6)
                        ],
                        /* group */ null,
                        /* name */ null);
                }

                ImGui.EndPopup();
            }

            ImGui.PopID();

            return true;
        }

        /// <returns>Null if cursor is not within the grid nor is dragging.</returns>
        private Point? FitCursorPositionToGrid(TileGridComponent grid, Point cursorGridPosition)
        {
            IntRectangle bounds = grid.Rectangle;
            if (!bounds.Contains(cursorGridPosition))
            {
                if (_startedShiftDragging is null)
                {
                    return null;
                }

                IntRectangle draggedRectangle = GridHelper.FromTopLeftToBottomRight(_startedShiftDragging.Value, cursorGridPosition);
                if (!bounds.TouchesInside(draggedRectangle))
                {
                    return null;
                }

                // If we are dragging, clamp the bounds of the current room.
                if (cursorGridPosition.X < grid.Origin.X)
                {
                    cursorGridPosition.X = grid.Origin.X;
                }
                else if (cursorGridPosition.X >= bounds.Right)
                {
                    cursorGridPosition.X = bounds.Right - 1;
                }

                if (cursorGridPosition.Y < grid.Origin.Y)
                {
                    cursorGridPosition.Y = grid.Origin.Y;
                }
                else if (cursorGridPosition.Y >= bounds.Bottom)
                {
                    cursorGridPosition.Y = bounds.Bottom - 1;
                }
            }

            return cursorGridPosition;
        }
    }
}