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
using Murder.Editor.EditorCore;
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
    public class TileEditorSystem : IMurderRenderSystem
    {
        public enum EditorMode
        {
            Draw,
            Cut
        }
        private EditorMode _editorMode = EditorMode.Draw;

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

            _inputAvailable = true;
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            if (hook.UsingCursor || hook.UsingGui)
            // Someone else is using our cursor, let's wait out turn.
            {
                _inputAvailable = false;
            }

            // Draw the toolbox

            bool isCursorWithin = false;
            isCursorWithin = DrawToolbox(render, editor);

            // Whether the cursor if within or interacting with any of the rooms.

            if (!isCursorWithin)
            {
                foreach (Entity e in context.Entities)
                {
                    isCursorWithin |= DrawResizeBox(render, context.World, editor, e);

                    if (_editorMode == EditorMode.Draw)
                    {
                        isCursorWithin |= DrawTilePainter(render, editor, e);
                    }
                    else if (_editorMode == EditorMode.Cut)
                    {
                        isCursorWithin |= DrawTileSelector(render, editor, e);
                    }
                }
            }

            if (!isCursorWithin)
            {
                DrawNewRoom(editor);
            }

            return;
        }

        private bool _pressedToolboxCut = false;
        private bool DrawToolbox(RenderContext render, EditorComponent editor)
        {
            var atlas = Game.Data.FetchAtlas(Murder.Data.AtlasId.Editor);
            var icon = atlas.Get(_editorMode == EditorMode.Cut ? "cursor_cut" : "cursor_pencil");

            Rectangle buttonRect = Rectangle.CenterRectangle(new Vector2(render.Camera.HalfWidth, 30), 39, 39);
            bool hovered = buttonRect.Contains(editor.EditorHook.CursorScreenPosition);
            bool down = false;


            if (hovered)
            {
                down = Game.Input.Down(MurderInputButtons.LeftClick);

                if (down)
                {
                    if (!_pressedToolboxCut)
                    {
                        down = true;
                        _pressedToolboxCut = true;
                    }
                }
                else
                {
                    if (_pressedToolboxCut)
                    {
                        //Clicked!
                        if (_editorMode == EditorMode.Cut)
                        {
                            _editorMode = EditorMode.Draw;
                        }
                        else
                        {
                            _editorMode = EditorMode.Cut;
                        }

                        _pressedToolboxCut = false;
                    }
                }
            }
            
            if ((down || _pressedToolboxCut) && !Game.Input.Down(MurderInputButtons.LeftClick))
            {
                down = false;
                _pressedToolboxCut = false;
            }

            render.UiBatch.DrawRectangle(buttonRect, down? Color.White : hovered ? Color.Gray * 0.8f: Color.Gray * 0.5f, 0.2f);
            render.UiBatch.DrawRectangleOutline(buttonRect, Color.Black, 1, 0.12f);
            icon.Draw(render.UiBatch, buttonRect.Center,
                new (Vector2.Zero, icon.Size), Color.White, Vector2.One * 2, 0, icon.Size/2f - new Vector2(0,0), ImageFlip.None, RenderServices.BLEND_NORMAL, 0);
            
            return hovered || _pressedToolboxCut;
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
                e.SetTileGrid(grid);
            }

            return _resize != null;
        }

        private int _targetEntity = -1;
        private Rectangle? _resize;

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
                    if (EditorServices.DragArea(
                        $"drag_{id}", editor.EditorHook.CursorWorldPosition.Value, worldRectangle, out Vector2 newDragWorldTopLeft))
                    {
                        Point newGridTopLeft = newDragWorldTopLeft.ToGridPoint();

                        // Clamp at zero.
                        newGridTopLeft.X = Math.Max(newGridTopLeft.X, 0);
                        newGridTopLeft.Y = Math.Max(newGridTopLeft.Y, 0);

                        _resize = new(position: newGridTopLeft, gridRectangle.Size);
                        _targetEntity = id;
                    }
                }

                Point offset = new(lineWidth, lineWidth);

                // Draw a cute area within the rectangle.
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
                return null;

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
        private Point? _startedSelectDragging;
        private Rectangle? _selectedArea;
        private Point? _selectedAreaDragOffset;
        private Color? _dragColor;

        private float _tweenStart;
        private Rectangle _currentRectDraw;
        private bool _inputAvailable;

        
        private bool GatherMapInfo(RenderContext render, EditorComponent editor, Entity e,
            out TileGridComponent gridComponent,
            [NotNullWhen(true)]  out TileGrid? grid,
            out Point cursorGridPosition,
            out IntRectangle bounds
            )
        {
            gridComponent = default;
            grid = null;
            bounds = default;
            cursorGridPosition = default;
            
            if (_resize is not null || render.Camera.Zoom < EditorFloorRenderSystem.ZoomThreshold)
            {
                // We are currently resizing, we have no business building tiles for now.
                return false;
            }

            if (_startedShiftDragging != null && _targetEntity != e.EntityId)
            {
                // we have no business just yet.
                return false;
            }

            if (!_inputAvailable)
                return false;

            gridComponent = e.GetTileGrid();
            grid = gridComponent.Grid;

            if (editor.EditorHook.CursorWorldPosition is not Point cursorWorldPosition)
                return false;

            cursorGridPosition = cursorWorldPosition.FromWorldToLowerBoundGridPosition();
            bounds = gridComponent.Rectangle;
            return true;
        }

        private bool DrawTileSelector(RenderContext render, EditorComponent editor, Entity e)
        {
            if (!GatherMapInfo(render, editor , e,
                out TileGridComponent gridComponent,
                out TileGrid? grid,
                out Point cursorGridPosition,
                out IntRectangle bounds))
            {
                return false;
            }

            Color color = Game.Profile.Theme.White.ToXnaColor();
            color = color * .5f;

            // Otherwise, we are at classical individual tile selection.

            bool hovered = _selectedArea?.Contains(cursorGridPosition) ?? false;

            if (_selectedArea != null)
            {
                if (_selectedAreaDragOffset != null)
                {
                    RenderServices.DrawRectangle(render.DebugFxBatch,
                        (_selectedArea.Value * Grid.CellSize)
                        .Expand(4 - 3 * Ease.ZeroToOne(Ease.BackInOut, 0.250f, _tweenStart)), color * .05f);
                }
                else
                {
                    RenderServices.DrawRectangleOutline(render.DebugFxBatch,
                        (_selectedArea.Value * Grid.CellSize)
                        .Expand(4 - 3 * Ease.ZeroToOne(Ease.BackInOut, 0.250f, _tweenStart)), color * (hovered ? 1 : .05f));
                }
            }
            else if (_startedSelectDragging != null)
            {
                RenderServices.DrawRectangleOutline(render.DebugFxBatch,
                    (GridHelper.FromTopLeftToBottomRight(_startedSelectDragging.Value, cursorGridPosition) * Grid.CellSize)
                    .Expand(4 - 3 * Ease.ZeroToOne(Ease.BackInOut, 0.250f, _tweenStart)), color * .15f);
            }
            else
            {
                IntRectangle rectangle = new Rectangle(cursorGridPosition.X, cursorGridPosition.Y, 1, 1);
                RenderServices.DrawRectangleOutline(render.DebugFxBatch, (rectangle * Grid.CellSize).Expand(4 - 3 * Ease.ZeroToOne(Ease.BackInOut, 0.250f, _tweenStart)), color * .15f);
            }

            if (Game.Input.Pressed(MurderInputButtons.LeftClick))
            {
                if (hovered)
                {
                    if (_selectedArea != null)
                    {
                        _selectedAreaDragOffset = cursorGridPosition - _selectedArea.Value.TopLeft.Point();
                    }
                }
                else if (_startedSelectDragging == null)
                {
                    _targetEntity = e.EntityId;

                    // Start tracking the origin.
                    _selectedArea = null;
                    _startedSelectDragging = cursorGridPosition;
                    _currentRectDraw = (GridHelper.FromTopLeftToBottomRight(_startedSelectDragging.Value, cursorGridPosition) * Grid.CellSize);
                    _dragColor = Game.Input.Down(MurderInputButtons.LeftClick) ? Color.Green * .1f : Color.Red * .05f;
                    _tweenStart = Game.Now;
                }
            }

            if (!Game.Input.Down(MurderInputButtons.LeftClick))
            {
                // Movement is over!
                if (_selectedAreaDragOffset != null && _selectedArea != null)
                {
                    MoveFromTo(_startedSelectDragging, cursorGridPosition, _selectedArea.Value.Size);
                }

                if (_startedSelectDragging != null)
                {
                    _selectedArea = GridHelper.FromTopLeftToBottomRight(_startedSelectDragging.Value, cursorGridPosition);
                    if (_selectedArea.Value.Size.X <= 1 || _selectedArea.Value.Size.Y <= 1)
                    {
                        _selectedArea = null;
                    }
                }
                _startedSelectDragging = null;
                
                _selectedAreaDragOffset = null;
            }
            else
            {
                // Dragging
                if (_selectedAreaDragOffset != null && _selectedArea !=null)
                {
                    _selectedArea = new Rectangle(cursorGridPosition - _selectedAreaDragOffset.Value, _selectedArea.Value.Size  );
                }
            }

            if (Game.Input.Pressed(MurderInputButtons.RightClick))
            {
                _selectedArea = null;
                _startedSelectDragging = null;
                _selectedAreaDragOffset = null;
            }

            return true;
        }

        private void MoveFromTo(Point? from, Point to, Vector2 size)
        {
            
        }

        /// <summary>
        /// This is the logic for capturing input for new tiles.
        /// </summary>
        /// <returns>
        /// Whether the mouse has interacted with this entity.
        /// </returns>
        private bool DrawTilePainter(RenderContext render, EditorComponent editor, Entity e)
        {
            if (!GatherMapInfo(render, editor, e,
                out TileGridComponent gridComponent,
                out TileGrid? grid,
                out Point cursorGridPosition,
                out IntRectangle bounds))
            {
                return false;
            }

            if (editor.EditorHook.CursorWorldPosition is not Point cursorWorldPosition)
                return false;

            if (!bounds.Contains(cursorGridPosition))
            {
                if (_startedShiftDragging == null)
                {
                    return false;
                }

                // If we are dragging, clamp the bounds of the current room.
                if (cursorGridPosition.X < gridComponent.Origin.X)
                {
                    cursorGridPosition.X = gridComponent.Origin.X;
                }
                else if (cursorGridPosition.X >= bounds.Right)
                {
                    cursorGridPosition.X = bounds.Right - 1;
                }

                if (cursorGridPosition.Y < gridComponent.Origin.Y)
                {
                    cursorGridPosition.Y = gridComponent.Origin.Y;
                }
                else if (cursorGridPosition.Y >= bounds.Bottom)
                {
                    cursorGridPosition.Y = bounds.Bottom - 1;
                }
            }

            // We are actually applying operation over an area.
            if (_startedShiftDragging == null &&
                Game.Input.Down(MurderInputButtons.Shift) &&
                (Game.Input.Down(MurderInputButtons.LeftClick) || Game.Input.Down(MurderInputButtons.RightClick)))
            {
                _targetEntity = e.EntityId;

                // Start tracking the origin.
                _startedShiftDragging = cursorGridPosition;
                _currentRectDraw = (GridHelper.FromTopLeftToBottomRight(_startedShiftDragging.Value, cursorGridPosition) * Grid.CellSize);
                _dragColor = Game.Input.Down(MurderInputButtons.LeftClick) ? Color.Green * .1f : Color.Red * .05f;
                _tweenStart = Game.Now;
            }

            int selectedTileMask = editor.EditorHook.CurrentSelectedTile.ToMask();
            if (_startedShiftDragging != null && _dragColor != null)
            {
                // Draw the rectangle applied over the area.
                IntRectangle draggedRectangle = GridHelper.FromTopLeftToBottomRight(_startedShiftDragging.Value, cursorGridPosition);
                Rectangle targetSize = draggedRectangle * Grid.CellSize;
                _currentRectDraw = Rectangle.Lerp(_currentRectDraw, targetSize, 0.45f);

                RenderServices.DrawRectangle(render.DebugBatch, _currentRectDraw, _dragColor.Value);
                RenderServices.DrawRectangleOutline(render.DebugBatch, _currentRectDraw, Color.White * .5f);

                if (Game.Input.Released(MurderInputButtons.LeftClick))
                {
                    _startedShiftDragging = null;
                    grid.SetGridPosition(draggedRectangle, selectedTileMask);
                }
                else if (Game.Input.Released(MurderInputButtons.RightClick))
                {
                    _startedShiftDragging = null;
                    grid.UnsetGridPosition(draggedRectangle, selectedTileMask);
                }

                return true;
            }

            // We are applying an operation over an individual tile.
            if (!editor.EditorHook.IsMouseOnStage)
            {
                return false;
            }

            Color color = Game.Profile.Theme.White.ToXnaColor();
            color = color * .5f;

            // Otherwise, we are at classical individual tile selection.
            IntRectangle rectangle = new Rectangle(cursorGridPosition.X, cursorGridPosition.Y, 1, 1);
            RenderServices.DrawRectangleOutline(render.DebugBatch, (rectangle * Grid.CellSize).Expand(4 - 3 * Ease.ZeroToOne(Ease.BackInOut, 0.250f, _tweenStart)), color);

            if (Game.Input.Down(MurderInputButtons.LeftClick))
            {
                if (!grid.AtGridPosition(cursorGridPosition).HasFlag(selectedTileMask))
                {
                    _tweenStart = Game.Now;

                    grid.SetGridPosition(cursorGridPosition, selectedTileMask);
                }
            }
            else if (Game.Input.Down(MurderInputButtons.RightClick))
            {
                if (grid.AtGridPosition(cursorGridPosition).HasFlag(selectedTileMask))
                {
                    _tweenStart = Game.Now;

                    grid.UnsetGridPosition(cursorGridPosition, selectedTileMask);
                }
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
                        new IComponent[]
                        {
                            new RoomComponent(defaultFloor),
                            new TileGridComponent(cursorGridPosition, 6, 6)
                        },
                        /* group */ null,
                        /* name */ null);
                }

                ImGui.EndPopup();
            }

            ImGui.PopID();

            return true;
        }
    }
}