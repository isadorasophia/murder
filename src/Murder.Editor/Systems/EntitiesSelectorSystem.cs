using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Murder.Assets;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Util;
using Murder.Utilities;

namespace Murder.Editor.Systems
{
    /// <summary>
    /// This system will draw selected entities and drag them through the map.
    /// </summary>
    [DoNotPause]
    [OnlyShowOnDebugView]
    [WorldEditor]
    [Filter(ContextAccessorFilter.AllOf, ContextAccessorKind.Read, typeof(ITransformComponent))]
    public class EntitiesSelectorSystem : IStartupSystem, IUpdateSystem, IGuiSystem, IMonoRenderSystem
    {
        private const float DRAG_MIN_DURATION = 0.05f;

        private readonly Vector2 _selectionBox = new Point(12, 12);
        
        /// <summary>
        /// Entity that is being dragged, if any.
        /// </summary>
        private Entity? _dragging = null;

        private float _dragTimer = 0;
        
        public ValueTask Start(Context context)
        {
            if (context.World.TryGetUnique<EditorComponent>()?.EditorHook is EditorHook hook)
            {
                hook.OnEntitySelected += OnEntityToggled;
            }

            return default;
        }
        
        public ValueTask DrawGui(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            // Entity List
            ImGui.SetNextWindowBgAlpha(0.9f);
            ImGui.SetNextWindowSizeConstraints(
                new System.Numerics.Vector2(300, 300),
                new System.Numerics.Vector2(600, 768)
            );

            ImGui.Begin("Hierarchy");

            ImGui.SetWindowPos(new(0, 250), ImGuiCond.Appearing);

            ImGui.BeginChild("hierarchy_entities");
            foreach (var entity in context.Entities)
            {
                var name = $"Instance";
                if (entity.TryGetComponent<PrefabRefComponent>(out var assetComponent))
                {
                    if (Game.Data.TryGetAsset<PrefabAsset>(assetComponent.AssetGuid) is PrefabAsset asset)
                    {
                        name = asset.Name;
                    }
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

            for (int i = hook.AllOpenedEntities.Length - 1; i >= 0; i--)
            {
                int opened = hook.AllOpenedEntities[i];

                if (context.World.TryGetEntity(opened) is Entity openedEntity)
                {
                    ImGui.SetNextWindowBgAlpha(0.9f);
                    ImGui.SetNextWindowDockID(42, ImGuiCond.Appearing);

                    if (hook.IsEntitySelected(openedEntity.EntityId))
                    {
                        ImGui.SetNextWindowFocus();
                    }

                    if (hook.DrawEntityInspector is not null && !hook.DrawEntityInspector(openedEntity))
                    {
                        hook.UnselectEntity(openedEntity);
                    }
                }
            }

            return default;
        }

        public ValueTask Update(Context context)
        {
            var hook = context.World.GetUnique<EditorComponent>().EditorHook;
            var world = (MonoWorld)context.World;
            
            bool clicked = Game.Input.Pressed(MurderInputButtons.LeftClick);
            Point cursorPosition = world.Camera.GetCursorWorldPosition(hook.Offset, new(hook.StageSize.X, hook.StageSize.Y));

            bool clickedOnEntity = false;
            
            hook.CursorWorldPosition = cursorPosition;
            hook.CursorScreenPosition = Game.Input.CursorPosition - hook.Offset;

            Rectangle bounds = new(hook.Offset, hook.StageSize);
            
            bool hasFocus = bounds.Contains(Game.Input.CursorPosition);
            if (hasFocus)
            {
                foreach (var e in context.Entities)
                {
                    Vector2 position = e.GetGlobalTransform().Vector2;
                    var rect = new Rectangle(position - _selectionBox / 2f, _selectionBox);

                    if (e.Parent is not null)
                    {
                        // Skip entities that are children.
                        continue;
                    }

                    if (rect.Contains(cursorPosition))
                    {
                        hook.Cursor = EditorHook.CursorStyle.Point;

                        if (!hook.IsEntityHovered(e.EntityId))
                        {
                            hook.HoverEntity(e);
                        }

                        if (clicked)
                        {
                            hook.SelectEntity(e, clear: true);
                            clickedOnEntity = true;

                            _dragging = e;
                        }

                        if (_dragging == e && Game.Input.Down(MurderInputButtons.LeftClick))
                        {
                            _dragTimer += Game.FixedDeltaTime;
                        }

                        if (Game.Input.Released(MurderInputButtons.LeftClick))
                        {
                            _tweenStart = Game.Now;
                            _selectPosition = position;
                        }

                        break;
                    }
                    else if (hook.IsEntityHovered(e.EntityId))
                    {
                        hook.UnhoverEntity(e);
                    }
                }

                if (_dragTimer > DRAG_MIN_DURATION && _dragging != null)
                {
                    _dragging.SetGlobalTransform(cursorPosition.ToPosition());
                }
                if (!Game.Input.Down(MurderInputButtons.LeftClick))
                {
                    _dragTimer = 0;
                }
            }

            if (clicked && !clickedOnEntity)
            {
                hook.UnselectAll();
            }

            if (_dragTimer > DRAG_MIN_DURATION)
            {
                hook.Cursor = EditorHook.CursorStyle.Hand;
            }

            return default;
        }

        /// <summary>
        /// Tracks tween for <see cref="_selectPosition"/>.
        /// </summary>
        private float _tweenStart;

        /// <summary>
        /// This is the position currently selected by the cursor.
        /// </summary>
        private Vector2? _selectPosition;
        
        private readonly Color _hoverColor = Game.Profile.Theme.Accent.WithAlpha(.7f);

        public ValueTask Draw(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            foreach (Entity e in context.Entities)
            {
                Vector2 position = e.GetGlobalTransform().Vector2;
                if (hook.IsEntityHovered(e.EntityId))
                {
                    Rectangle hoverRectangle = new(position - _selectionBox / 2f, _selectionBox);
                    RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, hoverRectangle, _hoverColor);
                }
            }

            DrawSelectionTween(render);

            return default;
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
                    Color color = Game.Profile.Theme.Accent.WithAlpha(startAlpha - startAlpha * tween);
                    
                    Vector2 size = _selectionBox + expand * 2;
                    Rectangle rectangle = new(position - size / 2f, size);
                    
                    RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, rectangle, color);
                }
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
