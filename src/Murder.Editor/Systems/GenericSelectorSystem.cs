using Bang;
using Bang.Entities;
using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Components;
using Murder.Editor.EditorCore;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Editor.Systems
{
    public class GenericSelectorSystem
    {
        private readonly Vector2 _selectionBox = new Point(12, 12);
        private Vector2 _offset = Vector2.Zero;

        /// <summary>
        /// Entity that is being dragged, if any.
        /// </summary>
        private Entity? _dragging = null;

        private bool _isShowingImgui = false;
        private string _filter = string.Empty;

        private Point? _startedGroupInWorld;
        private Rectangle? _currentAreaRectangle;

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

            // Entity List
            ImGui.SetNextWindowBgAlpha(0.9f);
            ImGui.SetNextWindowSizeConstraints(
                new System.Numerics.Vector2(300, 300),
                new System.Numerics.Vector2(600, 768)
            );

            ImGui.Begin("Hierarchy");

            ImGui.SetWindowPos(new(0, 250), ImGuiCond.Appearing);
            ImGui.InputTextWithHint("##search", "Filter...", ref _filter, 300);

            ImGui.BeginChild("hierarchy_entities");

            bool filter = !string.IsNullOrWhiteSpace(_filter);
            foreach (var entity in entities)
            {
                var name = $"Instance";
                if (entity.TryGetComponent<PrefabRefComponent>(out var assetComponent))
                {
                    if (Game.Data.TryGetAsset<PrefabAsset>(assetComponent.AssetGuid) is PrefabAsset asset)
                    {
                        name = asset.Name;
                    }
                }
                
                if (filter)
                {
                    if (!name.Contains(_filter))
                        continue;
                }

                if (ImGui.Selectable($"{name}({entity.EntityId})##{name}_{entity.EntityId}", hook.IsEntitySelected(entity.EntityId)))
                {
                    hook.SelectEntity(entity, clear: true);
                }
                if (ImGui.IsItemHovered())
                {
                    hook.HoverEntity(entity);
                }
            }

            ImGui.EndChild();
            ImGui.End();

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
            if (hook.UsingCursor)
            // Someone else is using our cursor, let's wait out turn.
            {
                _startedGroupInWorld = null;
                _currentAreaRectangle = null;
                _dragging = null;
                return;
            }

            if (hook.EntityToBePlaced is not null)
            {
                // An entity will be placed, skip this.
                return;
            }

            // If user has selected to destroy entities.
            if (Game.Input.Pressed(MurderInputButtons.Delete))
            {
                foreach ((_, Entity e) in hook.AllSelectedEntities)
                {
                    hook.RemoveEntityWithStage?.Invoke(e.EntityId);
                }

                hook.UnselectAll();
            }

            if (_dragging?.IsDestroyed ?? false)
            {
                _dragging = null;
            }

            bool clicked = Game.Input.Pressed(MurderInputButtons.LeftClick) && !hook.IsPopupOpen;
            bool released = Game.Input.Released(MurderInputButtons.LeftClick);

            MonoWorld monoWorld = (MonoWorld)world;
            Point cursorPosition = monoWorld.Camera.GetCursorWorldPosition(hook.Offset, new(hook.StageSize.X, hook.StageSize.Y));

            hook.CursorWorldPosition = cursorPosition;
            hook.CursorScreenPosition = Game.Input.CursorPosition - hook.Offset;

            Rectangle bounds = new(hook.Offset, hook.StageSize);
            bool hasFocus = bounds.Contains(Game.Input.CursorPosition);

            ImmutableDictionary<int, Entity> selectedEntities = hook.AllSelectedEntities;
            bool isMultiSelecting = Game.Input.Down(MurderInputButtons.Shift) || selectedEntities.Count > 1;

            bool clickedOnEntity = false;
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

                if (hasFocus && rect.Contains(cursorPosition) && !EditorCameraControllerSystem.IsDragging())
                {
                    hook.Cursor = CursorStyle.Point;

                    if (!hook.IsEntityHovered(e.EntityId))
                    {
                        hook.HoverEntity(e);
                    }
                    
                    if (released)
                    {
                        _tweenStart = Game.Now;
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

            if (clicked && SelectSmallestEntity(world, hook.CursorWorldPosition, hook.Hovering) is Entity entity)
            {
                hook.SelectEntity(entity, clear: clearOnlyWhenSelectedNewEntity || !isMultiSelecting);
                clickedOnEntity = true;

                _offset = entity.GetGlobalTransform().Vector2 - cursorPosition;
                _dragging = entity;
            }

            if (_dragging != null)
            {
                Vector2 delta = cursorPosition - _dragging.GetGlobalTransform().Vector2 + _offset;

                // On "ctrl", snap entities to the grid.
                bool snapToGrid = Game.Input.Down(MurderInputButtons.Ctrl);

                // Drag all the entities which are currently selected.
                foreach ((int _, Entity e) in selectedEntities)
                {
                    if (e.IsDestroyed)
                    {
                        hook.UnselectEntity(e);
                        continue;
                    }

                    IMurderTransformComponent newTransform = e.GetGlobalTransform().Add(delta);
                    if (snapToGrid)
                    {
                        newTransform = newTransform.SnapToGridDelta();
                    }

                    e.SetGlobalTransform(newTransform);
                }
            }

            if (!Game.Input.Down(MurderInputButtons.LeftClick))
            {
                // The user stopped clicking, so no longer drag anything.
                _dragging = null;
            }

            if (hasFocus && clicked && !clickedOnEntity)
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

        private Entity? SelectSmallestEntity(World world, Vector2 cursor, ImmutableArray<int> hovering)
        {
            float smallestBoxArea = float.MaxValue;
            float smallestDistanceSq = float.MaxValue;
            Entity? smallest = null;

            foreach (var eId in hovering)
            {
                if (world.TryGetEntity(eId) is not Entity entity)
                    continue;
                var position = entity.GetGlobalTransform().Vector2;
                var box = GetSeletionBoundingBox(entity,world, position, out _);
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

        protected void DrawImpl(RenderContext render, World world, ImmutableArray<Entity> entities)
        {
            EditorHook hook = world.GetUnique<EditorComponent>().EditorHook;
            foreach (Entity e in entities)
            {
                if (!e.HasTransform()) continue;
                if (e.Parent != null) continue;

                Vector2 position = e.GetGlobalTransform().Vector2;
                if (!render.Camera.SafeBounds.Contains(position))
                    continue;
                Rectangle bounds = GetSeletionBoundingBox(e, world, position, out var hasBox);

                if (hook.IsEntitySelected(e.EntityId))
                {
                    RenderServices.DrawRectangle(render.DebugFxSpriteBatch, bounds, _hoverColor * (hook.IsEntityHovered(e.EntityId) ? 0.65f : 0.45f));
                    RenderServices.DrawRectangleOutline(render.DebugFxSpriteBatch, bounds, Game.Profile.Theme.Accent * 0.45f);
                }
                else if (hook.IsEntityHovered(e.EntityId))
                {
                    RenderServices.DrawRectangleOutline(render.DebugFxSpriteBatch, bounds, _hoverColor * 0.45f);
                }
                else if (!hasBox)
                {
                    RenderServices.DrawCircle(render.DebugSpriteBatch, position, 2, 6, Game.Profile.Theme.Yellow);
                }
            }
        

            DrawSelectionTween(render);
            DrawSelectionRectangle(render);
        }

        private void DrawSelectionTween(RenderContext render)
        {
            if (_selectPosition is Vector2 position)
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

                    RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, rectangle, color);
                }
            }
        }

        private void DrawSelectionRectangle(RenderContext render)
        {
            if (_currentAreaRectangle is not null && _currentAreaRectangle.Value.Size.X > 1)
            {
                RenderServices.DrawRectangle(render.DebugFxSpriteBatch, _currentAreaRectangle.Value, Color.White * .25f);
                RenderServices.DrawRectangleOutline(render.DebugFxSpriteBatch, _currentAreaRectangle.Value, Color.White * .75f);
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
    }
}
