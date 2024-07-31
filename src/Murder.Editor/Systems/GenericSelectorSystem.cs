using Bang;
using Bang.Entities;
using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Graphics;
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
using System.Collections.Immutable;
using System.Numerics;
using Microsoft.Xna.Framework.Input;
using Murder.Editor.Messages;

namespace Murder.Editor.Systems;

public class GenericSelectorSystem
{
    private readonly Vector2 _selectionBox = new Point(12, 12);
    private Vector2 _offset = Vector2.Zero;

    /// <summary>
    /// Entity that is being dragged, if any.
    /// </summary>
    private Entity? _dragging = null;
    private Entity? _startedDragging = null;
    private Vector2? _dragStart = null;
    private float _selectionStartTime = 0;

    private Point _previousKeyboardDelta = Point.Zero;

    private bool _isShowingImgui = false;
    private string _filter = string.Empty;

    private Point? _startedGroupInWorld;
    private Rectangle? _currentAreaRectangle;

    private bool _showHierarchy = false;
    private ImmutableArray<int> _previousHovering = ImmutableArray<int>.Empty;

    int _previousSelection = -1;

    private int[]? _copiedEntities = null;

    public void StartImpl(World world)
    {
        if (world.TryGetUnique<EditorComponent>()?.EditorHook is EditorHook hook)
        {
            hook.OnEntitySelected += OnEntityToggled;
        }
    }

    /// <summary>
    /// This is only used for rendering the entity components during the game (on debug mode).
    /// </summary>
    public void DrawGuiImpl(World world, ImmutableArray<Entity> entities)
    {
        _isShowingImgui = true;

        EditorHook hook = world.GetUnique<EditorComponent>().EditorHook;

        ImGui.BeginMainMenuBar();

        if (ImGui.BeginMenu("Show"))
        {
            ImGui.MenuItem("Hierarchy", "", ref _showHierarchy);
            ImGui.EndMenu();
        }

        ImGui.EndMainMenuBar();

        if (_showHierarchy)
        {
            // Entity List
            ImGui.SetNextWindowBgAlpha(0.9f);
            ImGui.SetNextWindowSizeConstraints(
                new Vector2(300, 300),
                new Vector2(EditorSystem.WINDOW_MAX_WIDTH, EditorSystem.WINDOW_MAX_HEIGHT)
            );

            if (ImGui.Begin("Hierarchy", ref _showHierarchy))
            {
                ImGui.SetWindowPos(new(0, 250), ImGuiCond.Appearing);
                ImGui.SetNextItemWidth(-1);
                ImGui.InputTextWithHint("##search", "Filter...", ref _filter, 300);

                ImGui.BeginChild("hierarchy_entities");
                
                int? filterId = null;
                bool filter = false;
                if (int.TryParse(_filter, out int filterAttempt))
                {
                    filterId = filterAttempt;
                    filter = false;
                }
                else
                {
                    filter = !string.IsNullOrWhiteSpace(_filter);
                }

                foreach (var entity in entities)
                {
                    if (filterId != null)
                    {
                        if (entity.EntityId != filterId)
                            continue;
                    }

                    string? name = null;
                    bool hasName = false;
                    Entity? parent = null;
                    if (EntityServices.TryGetEntityName(entity) is string nameFound)
                    {
                        hasName = true;
                        name = nameFound;
                    }
                    else
                    {
                        // Children don't have asset names, so we need to fetch the parent.

                        if (entity.TryFetchParent() is Entity parentFound)
                        {
                            parent = parentFound;
                            name = parent.FetchChildrenWithNames[entity.EntityId] ?? "Child";
                        }
                        else
                        {
                            name = "Entity";
                        }
                    }

                    if (filter)
                    {
                        if (!hasName)
                        {
                            if (parent is not null)
                            { 
                                if (EntityServices.TryGetEntityName(parent) is string parentName)
                                {
                                    if (!parentName.Contains(_filter, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            // if the entity has a name, we can filter by it.
                            if (!name!.Contains(_filter, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (parent is not null)
                                {
                                    if (EntityServices.TryGetEntityName(parent) is string parentName)
                                    {
                                        if (!parentName.Contains(_filter, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            continue;
                                        }
                                    }
                                }
                                continue;
                            }
                        }
                    }

                    if (entity.Parent is not null)
                    {
                        ImGui.Indent(30);
                        ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Faded);
                    }
                    ImGui.TextColored(hook.IsEntitySelected(entity.EntityId) ? Game.Profile.Theme.Accent : Game.Profile.Theme.Faded, $"{entity.EntityId}");
                    ImGui.SameLine();

                    bool isSelected = hook.IsEntitySelected(entity.EntityId);
                    if (isSelected)
                    {
                        if (_previousSelection != entity.EntityId)
                        {
                            _previousSelection = entity.EntityId;
                            ImGui.SetScrollHereY();
                        }
                    }

                    if (ImGui.Selectable($"{name}##{name}_{entity.EntityId}", isSelected))
                    {
                        hook.SelectEntity(entity, clear: true);
                    }
                    if (ImGui.IsItemHovered())
                    {
                        hook.HoverEntity(entity, true);
                    }


                    if (entity.Parent is not null)
                    {
                        ImGui.Indent(-30);
                        ImGui.PopStyleColor();
                    }
                }
                ImGui.EndChild();
            }
            ImGui.End();
        }
        foreach ((_, Entity e) in hook.AllSelectedEntities)
        {
            ImGui.SetNextWindowBgAlpha(0.9f);
            ImGui.SetNextWindowDockID(42, ImGuiCond.Appearing);

            if (hook.DrawEntityInspector is not null && !hook.DrawEntityInspector(world, e))
            {
                hook.UnselectEntity(e);
            }
        }
    }

    /// <summary>
    /// Update and select entities in the world.
    /// </summary>
    /// <param name="clearOnlyWhenSelectedNewEntity">
    /// Whether it should clear the selected entities. If this is true, it will clear entities whenever
    /// the user selects an empty state and supports selecting multiple entities.
    /// Otherwise, it will only allow selecting one entity at a time.
    /// </param>
    public void Update(World world, ImmutableArray<Entity> entities, bool clearOnlyWhenSelectedNewEntity = false)
    {
        EditorHook hook = world.GetUnique<EditorComponent>().EditorHook;

        if (hook.EditorMode == EditorHook.EditorModes.EditMode)
        {
            return;
        }
        
        if (hook.EntityToBePlaced is not null)
        {
            // An entity will be placed, skip this.
            return;
        }

        // If user has selected to destroy entities.
        if (!Game.Input.KeyboardConsumed && Game.Input.Pressed(MurderInputButtons.Delete))
        {
            foreach ((_, Entity e) in hook.AllSelectedEntities)
            {
                hook.RemoveEntityWithStage?.Invoke(e.EntityId);
            }

            hook.UnselectAll();
        }

        bool isCursorBusy = hook.CursorIsBusy.Any(); // && ignoreCursorOnCollidersSelected;

        if (isCursorBusy || hook.UsingGui)
        // Someone else is using our cursor, let's wait out turn.
        {
            _startedGroupInWorld = null;
            _currentAreaRectangle = null;
            _dragging = null;
            _dragStart = null;
            return;
        }

        if (_dragging?.IsDestroyed ?? false)
        {
            _dragging = null;
            _dragStart = null;
        }

        if (hook.CursorWorldPosition is not Point cursorPosition || !hook.IsMouseOnStage)
        {
            return;
        }

        // ======
        // Support copying and pasting entities.
        // ======
        bool copied = Game.Input.Shortcut(EditorInputHelpers.CTRL_C);
        if (copied && hook.AllSelectedEntities.Count != 0)
        {
            _copiedEntities = [.. hook.AllSelectedEntities.Keys];
        }

        bool pasted = Game.Input.Shortcut(EditorInputHelpers.CTRL_V);
        if (pasted && _copiedEntities is not null)
        {
            Vector2 origin = GetTopLeftOrigin(world, _copiedEntities);
            hook.DuplicateEntitiesAt?.Invoke(_copiedEntities, cursorPosition - origin);
        }
        // ======

        bool clicked = Game.Input.Pressed(MurderInputButtons.LeftClick) && !hook.IsPopupOpen;
        bool down = Game.Input.Down(MurderInputButtons.LeftClick);
        bool released = Game.Input.Released(MurderInputButtons.LeftClick);

        // Hovering room
        if (hook.AllSelectedEntities.Count == 0)
        {
            hook.HoveringGroup = EditorTileServices.FindTargetGroup(world, hook, cursorPosition);
        }

        ImmutableDictionary<int, Entity> selectedEntities = hook.AllSelectedEntities;
        bool isMultiSelecting = Game.Input.Down(MurderInputButtons.Shift);
        bool clickedOnEntity = false;

        MonoWorld monoWorld = (MonoWorld)world;
        Rectangle cameraRect = monoWorld.Camera.SafeBounds;

        hook.ClearHoveringEntities();
        foreach (Entity e in entities)
        {
            if (!e.HasTransform()) continue;

            Vector2 position = e.GetGlobalTransform().Vector2;
            Rectangle rect = GetSeletionBoundingBox(e, world, position, out _);

            if (e.Parent is not null && !hook.EnableSelectChildren)
            {
                // We block dragging entities on world editors otherwise it would be too confusing (signed: Pedro).
                continue;
            }
            bool isInsideCamera = cameraRect.IsInside(rect);
            bool isMuchBiggerThanCamera = rect.Width > cameraRect.Width && rect.Height > cameraRect.Height;

            if (isInsideCamera && isMuchBiggerThanCamera)
            {
                // If the entity is much bigger than the camera, you can still select it by clicking on the very center of it.
                rect = new Rectangle(rect.Center - new Point(5), new Point(10));
            }

            if (rect.Contains(cursorPosition) && !EditorCameraControllerSystem.IsDragging())
            {
                hook.Cursor = CursorStyle.Point;

                if (!hook.IsEntityHovered(e.EntityId))
                {
                    hook.HoverEntity(e, clear:false);
                    // hook.HoverEntity(e, clear: !clearOnlyWhenSelectedNewEntity);
                }

                if (released)
                {
                    _tweenStart = Game.NowUnscaled;
                    _selectPosition = position;
                }

                continue;
            }
            else if (hook.IsEntityHovered(e.EntityId))
            {
                // This entity is no longer being hovered.
                hook.UnhoverEntity(e);
            }
            else if (_currentAreaRectangle.HasValue && _currentAreaRectangle.Value.Contains(position))
            {
                // The entity is within a rectangle area.
                hook.SelectEntity(e, clear: clearOnlyWhenSelectedNewEntity);
            }
        }

        bool cycle = true;
        if (_previousHovering.Length != hook.Hovering.Length)
        {
            cycle = false;
        }
        else
        {
            for (int i = 0; i < _previousHovering.Length; i++)
            {
                if (_previousHovering[i] != hook.Hovering[i])
                {
                    cycle = false;
                    break;
                }
            }
        }

        if (!isCursorBusy)
        {
            if (SelectSmallestEntity(world, cursorPosition, hook.Hovering, hook.AllSelectedEntities.Keys.ToImmutableArray(), released) is Entity entity)
            {
                if (clicked || (cycle && released && _dragStart == cursorPosition))
                {
                    if (!hook.IsEntitySelected(entity.EntityId))
                    {
                        _selectionStartTime = Game.NowUnscaled;
                    }

                    if (!isMultiSelecting)
                    {
                        isMultiSelecting |= hook.IsEntitySelected(entity.EntityId);
                    }
                    hook.SelectEntity(entity, clear: clearOnlyWhenSelectedNewEntity || !isMultiSelecting);
                    clickedOnEntity = true;

                    if (_dragStart == null)
                    {
                        _dragStart = cursorPosition;
                        _startedDragging = entity;
                    }
                }
            }

            if (_dragging == null && _dragStart != null && _startedDragging!=null && down)
            {
                float distance = (cursorPosition - _dragStart.Value).Length();
                if (distance > 4)
                {
                    _dragging = _startedDragging;
                    _offset = _startedDragging.GetGlobalTransform().Vector2 - cursorPosition;

                    foreach ((int _, Entity e) in selectedEntities)
                    {
                        e.AddOrReplaceComponent(new EditorTween(Game.NowUnscaled, 0.4f, EditorTweenType.Lift));
                    }
                }
            }
        }

        if (_dragging != null && !isCursorBusy)
        {
            Vector2 delta = cursorPosition - _dragging.GetGlobalTransform().Vector2 + _offset;

            // On "ctrl", snap entities to the grid.
            bool snapToGrid = Game.Input.Down(Keys.LeftControl);

            // On "shift", constrain entities to the axis corresponding to the direction are being dragged in.
            bool snapToAxis = Game.Input.Down(Keys.LeftShift);
            
            // Drag all the entities which are currently selected.
            foreach ((int _, Entity e) in selectedEntities)
            {
                if (e.IsDestroyed)
                {
                    hook.UnselectEntity(e);
                    continue;
                }

                if (!e.HasTransform())
                {
                    continue;
                }

                IMurderTransformComponent newTransform = e.GetGlobalTransform().Add(delta);

                if (snapToGrid)
                {
                    newTransform = newTransform.SnapToGridDelta();
                }

                if (snapToAxis && _dragStart != null)
                {
                    Vector2 entityOffset = cursorPosition.ToVector2() - newTransform.ToVector2();
                    Vector2 start = _dragStart.Value - entityOffset;
                    Vector2 dragDistance = newTransform.Vector2 - start;

                    if (dragDistance != Vector2.Zero)
                    {
                        Vector2 unitDirection = Vector2.One - RoundToAbsoluteAxis(dragDistance);
                        Vector2 newPosition = start + (dragDistance * unitDirection);
                        Vector2 newDelta = newPosition - start;

                        newTransform = newTransform.Subtract(newDelta);
                    }
                }

                e.SetGlobalTransform(newTransform);
            }
        }
        else
        {
            Point delta = GetKeyboardDelta();

            if (delta != _previousKeyboardDelta)
            {
                // On "ctrl", snap entities to the grid.
                bool snapToGrid = Game.Input.Down(Keys.LeftControl);

                foreach ((int _, Entity e) in selectedEntities)
                {
                    if (!e.HasTransform())
                    {
                        continue;
                    }

                    IMurderTransformComponent newTransform = e.GetGlobalTransform();

                    if (snapToGrid)
                    {
                        newTransform = newTransform.Add(delta * Grid.CellSize)/*.SnapToGridDelta()*/;
                    }
                    else
                    {
                        newTransform = newTransform.Add(delta);
                    }

                    e.SetGlobalTransform(newTransform);
                    e.SendMessage(new AssetUpdatedMessage(typeof(PositionComponent)));
                    e.AddOrReplaceComponent(new EditorTween(Game.NowUnscaled, 1f, EditorTweenType.Move));
                }
            }
        }

        if (released)
        {
            _previousHovering = hook.Hovering;
            _dragStart = null;
        }

        _previousKeyboardDelta = GetKeyboardDelta();

        if (!Game.Input.Down(MurderInputButtons.LeftClick))
        {
            if (_dragging != null)
            {
                foreach ((int _, Entity e) in selectedEntities)
                {
                    e.SendMessage(new AssetUpdatedMessage(typeof(PositionComponent)));
                    e.AddOrReplaceComponent(new EditorTween(Game.NowUnscaled, 0.7f, EditorTweenType.Place));
                }


                // The user stopped clicking, so no longer drag anything.
                _dragging = null;
                _dragStart = null;
            }
        }

        if (hook.IsMouseOnStage && clicked && !clickedOnEntity)
        {
            if (!_isShowingImgui && !clearOnlyWhenSelectedNewEntity)
            {
                // User clicked in an empty space (which is not targeting any entities).
                hook.UnselectAll();
            }

            // We might have just started a group!
            _startedGroupInWorld = cursorPosition;
            _currentAreaRectangle = GridHelper.FromTopLeftToBottomRight(_startedGroupInWorld.Value, cursorPosition);
        }

        if (_dragging != null)
        {
            hook.Cursor = CursorStyle.Hand;
        }

        if (_startedGroupInWorld != null && _currentAreaRectangle != null)
        {
            if (!released)
            {
                Rectangle target = GridHelper.FromTopLeftToBottomRight(_startedGroupInWorld.Value, cursorPosition);
                _currentAreaRectangle = Rectangle.Lerp(_currentAreaRectangle.Value, target, 0.45f);
            }
            else
            {
                // Reset our group and wrap it up.
                _startedGroupInWorld = null;
                _currentAreaRectangle = null;
            }
        }

        if (_currentAreaRectangle.HasValue)
        {
            hook.SelectionBox = _currentAreaRectangle.Value;
        }

        // If you click on empty space, we should clear the selection.
        if (hook.Hovering.Length == 0 && clicked)
        {
            hook.UnselectAll();
        }
    }

    private Vector2 RoundToAbsoluteAxis(Vector2 vector)
    {
        if (MathF.Abs(vector.X) > MathF.Abs(vector.Y))
        {
            return new Vector2(1, 0);
        } 
        else
        {
            return new Vector2(0, 1);
        }
    }

    private Point GetKeyboardDelta()
    {
        Point delta = Point.Zero;

        if (Game.Input.Pressed(Keys.Up))
        {
            delta += new Point(0, -1);
        }

        if (Game.Input.Pressed(Keys.Down))
        {
            delta += new Point(0, 1);
        }

        if (Game.Input.Pressed(Keys.Left))
        {
            delta += new Point(-1, 0);
        }

        if (Game.Input.Pressed(Keys.Right))
        {
            delta += new Point(1, 0);
        }

        return delta;
    }

    private Rectangle GetSeletionBoundingBox(Entity e, World world, Vector2 position, out bool HasBox)
    {
        if (e.TryGetCollider() is ColliderComponent colliderComponent)
        {
            var rect = colliderComponent.GetBoundingBox(position);
            if (!rect.IsEmpty)
            {
                HasBox = true;
                return rect;
            }
        }

        if (e.TryGetSprite() is SpriteComponent sprite && Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid) is SpriteAsset spriteAsset)
        {
            HasBox = true;
            if (e.TryGetParallax() is ParallaxComponent parallax)
            {
                var parallaxOffset = ((MonoWorld)world).Camera.Position * (1 - parallax.Factor);
                return new Rectangle(position + parallaxOffset - spriteAsset.Origin - sprite.Offset * spriteAsset.Size, spriteAsset.Size);
            }
            else
            {
                return new Rectangle(position - spriteAsset.Origin - sprite.Offset * spriteAsset.Size, spriteAsset.Size);
            }
        }

        HasBox = false;
        return new(position - _selectionBox / 2f, _selectionBox);
    }

    private static Vector2 GetTopLeftOrigin(World world, int[] selectedEntities)
    {
        if (selectedEntities.Length == 0)
        {
            return Vector2.Zero;
        }

        Vector2 result = new(float.MaxValue, float.MaxValue);
        foreach (int entityId in selectedEntities)
        {
            Entity e = world.GetEntity(entityId);
            if (e.Parent is not null || e.TryGetTransform() is not IMurderTransformComponent transform)
            {
                // ignore parents or entities without a position.
                continue;
            }

            if (transform.Vector2.X < result.X)
            {
                result = result with { X = transform.Vector2.X };
            }

            if (transform.Vector2.Y < result.Y)
            {
                result = result with { Y = transform.Vector2.Y };
            }
        }

        return result;
    }

    private Entity? SelectSmallestEntity(World world, Vector2 cursor, ImmutableArray<int> hovering, ImmutableArray<int> selectedEntities, bool cycle)
    {
        float smallestBoxArea = float.MaxValue;
        float smallestDistanceSq = float.MaxValue;
        Entity? smallest = null;

        // If one entity is previously selected, we select the next hovered entity.
        if (selectedEntities.Length== 1)
        {
            int selected = selectedEntities[0];
            if (cycle)
            {
                int hoveredIndex = hovering.IndexOf(selected);
                if (hoveredIndex != -1)
                {
                    if (world.TryGetEntity(hovering[Calculator.WrapAround(hoveredIndex + 1, 0, hovering.Length - 1)]) is Entity entity)
                    {
                        return entity;
                    }
                }
            }
            else
            {
                int hoveredIndex = hovering.IndexOf(selected);

                if (hoveredIndex>=0 && world.TryGetEntity(selected) is Entity entity)
                {
                    return entity;
                }
            }
        }

        // No entity, or multiple entities are selected, so we can select the smallest entity.
        foreach (var eId in hovering)
        {
            if (world.TryGetEntity(eId) is not Entity entity)
            {
                continue;
            }

            if (!entity.HasTransform())
            {
                continue;
            }

            var position = entity.GetGlobalTransform().Vector2;
            var box = GetSeletionBoundingBox(entity, world, position, out _);
            var boxArea = box.Width * box.Height;
            var distance = (box.Center - cursor).LengthSquared();

            if (boxArea < smallestBoxArea)
            {
                smallestBoxArea = boxArea;
                smallest = entity;
                smallestDistanceSq = distance;
            }
            else if (boxArea == smallestBoxArea)
            {
                if (distance < smallestDistanceSq)
                {
                    smallestBoxArea = boxArea;
                    smallest = entity;
                    smallestDistanceSq = distance;
                }
            }
        }

        return smallest;
    }

    /// <summary>
    /// Tracks tween for <see cref="_selectPosition"/>.
    /// </summary>
    private float _tweenStart;

    /// <summary>
    /// This is the position currently selected by the cursor.
    /// </summary>
    private Vector2? _selectPosition;

    private readonly Color _hoverColor = (Game.Profile.Theme.Accent * .7f);

    private readonly ImmutableArray<Vector2> _xPointsCache = [

                    new Vector2(-4, 4),  // Top left
                    new Vector2(4, -4),  // Bottom right
                    new Vector2(4, 4),   // Top right
                    new Vector2(-4, -4)  // Bottom left
    ];

    protected void DrawImpl(RenderContext render, World world, ImmutableArray<Entity> entities)
    {
        EditorHook hook = world.GetUnique<EditorComponent>().EditorHook;
        if (hook.EditorMode == EditorHook.EditorModes.EditMode)
        {
            return;
        }

        if (Game.Input.Down(Keys.Space))
        {
            return;
        }

        foreach (Entity e in entities)
        {
            if (!e.HasTransform()) continue;
            if (e.Parent != null) continue;

            Vector2 position = e.GetGlobalTransform().Vector2;
            Rectangle bounds = GetSeletionBoundingBox(e, world, position, out var hasBox);
            if (!render.Camera.SafeBounds.TouchesInside(bounds))
                continue;

            if (hook.IsEntitySelected(e.EntityId))
            {
                float rotation = Ease.BackOut(Calculator.ClampTime(_selectionStartTime, Game.NowUnscaled, .6f)) * MathF.PI * 0.5f;

                // Define the rotation matrix
                Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation(rotation, Vector2.Zero);

                // Draw the rotated lines
                RenderServices.DrawLine(render.DebugBatch, Vector2.Transform(_xPointsCache[0], rotationMatrix) + position, Vector2.Transform(_xPointsCache[1], rotationMatrix) + position, Game.Profile.Theme.Yellow, 0.5f);
                RenderServices.DrawLine(render.DebugBatch, Vector2.Transform(_xPointsCache[2], rotationMatrix) + position, Vector2.Transform(_xPointsCache[3], rotationMatrix) + position, Game.Profile.Theme.Yellow, 0.5f);

                if (_dragging != null)
                {
                    if (_dragging.TryGetComponent<EditorTween>() is EditorTween draggingTween)
                    {
                        float delta = Ease.BackOut(Calculator.ClampTime(draggingTween.StartTime, Game.NowUnscaled, draggingTween.Duration));
                        float cornerSize = 6 * delta;
                        float corners = 1 - delta;

                        Vector2 topLeft = bounds.TopLeft + new Vector2(corners, corners) * 4;
                        RenderServices.DrawLine(render.DebugFxBatch, topLeft, topLeft + new Vector2(cornerSize, 0), Game.Profile.Theme.HighAccent, 0.5f);
                        RenderServices.DrawLine(render.DebugFxBatch, topLeft, topLeft + new Vector2(0, cornerSize), Game.Profile.Theme.HighAccent, 0.5f);

                        Vector2 topRight = bounds.TopRight + new Vector2(-corners, corners) * 4;
                        RenderServices.DrawLine(render.DebugFxBatch, topRight, topRight + new Vector2(-cornerSize, 0), Game.Profile.Theme.HighAccent, 0.5f);
                        RenderServices.DrawLine(render.DebugFxBatch, topRight, topRight + new Vector2(0, cornerSize), Game.Profile.Theme.HighAccent, 0.5f);

                        Vector2 bottomLeft = bounds.BottomLeft + new Vector2(corners, -corners) * 4;
                        RenderServices.DrawLine(render.DebugFxBatch, bottomLeft, bottomLeft + new Vector2(cornerSize, 0), Game.Profile.Theme.HighAccent, 0.5f);
                        RenderServices.DrawLine(render.DebugFxBatch, bottomLeft, bottomLeft + new Vector2(0,-cornerSize), Game.Profile.Theme.HighAccent, 0.5f);

                        Vector2 bottomRight = bounds.BottomRight + new Vector2(-corners, -corners) * 4;
                        RenderServices.DrawLine(render.DebugFxBatch, bottomRight, bottomRight + new Vector2(-cornerSize, 0), Game.Profile.Theme.HighAccent, 0.5f);
                        RenderServices.DrawLine(render.DebugFxBatch, bottomRight, bottomRight + new Vector2(0, -cornerSize), Game.Profile.Theme.HighAccent, 0.5f);
                    }
                }
                else
                {
                    float alpha = 1f;
                    if (e.TryGetComponent<EditorTween>() is EditorTween editorTween &&
                    editorTween.StartTime + editorTween.Duration > Game.NowUnscaled)
                    {
                        float delta = Calculator.ClampTime(editorTween.StartTime, Game.NowUnscaled, editorTween.Duration);
                        alpha = Ease.CubeOut(delta);

                        if (editorTween.Type == EditorTweenType.Move && delta < 1)
                        {
                            alpha = 0;
                        }
                    }

                    if (alpha > 0)
                    {
                        RenderServices.DrawRectangle(render.DebugFxBatch, bounds, _hoverColor * (hook.IsEntityHovered(e.EntityId) ? 0.65f : 0.45f) * alpha);
                        RenderServices.DrawRectangleOutline(render.DebugFxBatch, bounds, alpha * Game.Profile.Theme.Accent * 0.45f);
                    }
                }
            }
            else if (hook.IsEntityHovered(e.EntityId))
            {
                RenderServices.DrawRectangleOutline(render.DebugFxBatch, bounds, _hoverColor * 0.45f);
                DrawSelectionTween(render, position);
            }
            else if (!hasBox)
            {
                RenderServices.DrawCircleOutline(render.DebugBatch, position, 2, 6, Game.Profile.Theme.Yellow);
            }
            
        }

        DrawSelectionRectangle(render);
    }

    private void DrawSelectionTween(RenderContext render, Vector2 position)
    {
        float tween = Ease.ZeroToOne(Ease.BackOut, 2f, _tweenStart);
        if (tween == 1)
        {
            _selectPosition = null;
        }
        else
        {
            float expand = (1 - tween) * 3;

            float startAlpha = .9f;
            Color color = Game.Profile.Theme.Accent * (startAlpha - startAlpha * tween);

            Vector2 size = _selectionBox.Add(expand * 2);
            Rectangle rectangle = new(position - size / 2f, size);

            RenderServices.DrawRectangleOutline(render.DebugBatch, rectangle, color);
        }
    }

    private void DrawSelectionRectangle(RenderContext render)
    {
        if (_currentAreaRectangle is not null && _currentAreaRectangle.Value.Size.X > 1)
        {
            RenderServices.DrawRectangle(render.DebugFxBatch, _currentAreaRectangle.Value, Color.White * .25f);
            RenderServices.DrawRectangleOutline(render.DebugFxBatch, _currentAreaRectangle.Value, Color.White * .75f);
        }
    }

    private void OnEntityToggled(Entity e, bool selected)
    {
        if (selected)
        {
            e.AddOrReplaceComponent(new IsSelectedComponent());
        }
        else
        {
            e.RemoveComponent<IsSelectedComponent>();
        }
    }


    protected void ShowAllPossibleSelections(EditorHook hook, ref float lastCursorMoved, ref Vector2 previousCursorPosition)
    {
        if (hook.Hovering.Length > 0)
        {
            if (hook.CursorWorldPosition is not null)
            {
                if (previousCursorPosition == hook.CursorWorldPosition)
                {
                    if (lastCursorMoved < Game.NowUnscaled - 0.2f && !hook.CursorIsBusy.Any() && !hook.UsingGui)
                    {
                        if (ImGui.BeginTooltip())
                        {
                            foreach (var entity in hook.Hovering)
                            {
                                if (hook.AllSelectedEntities.Where(e => e.Key == entity).Any())
                                {
                                    ImGui.TextColored(Game.Profile.Theme.Accent, $"({entity}) {hook.GetNameForEntityId?.Invoke(entity)}");
                                }
                                else
                                {
                                    ImGui.Text($"({entity}) {hook.GetNameForEntityId?.Invoke(entity)}");
                                }
                            }

                            ImGui.EndTooltip();
                        }
                    }
                }
                else
                {
                    previousCursorPosition = hook.CursorWorldPosition.Value;
                    lastCursorMoved = Game.NowUnscaled;
                }
            }

        }
    }
}
