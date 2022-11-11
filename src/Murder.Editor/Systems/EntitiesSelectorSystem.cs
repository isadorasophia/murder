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
    public class EntitiesSelectorSystem : IUpdateSystem, IGuiSystem, IMonoRenderSystem
    {
        private const float DRAG_MIN_DURATION = 0.15f;

        private readonly Point _selectionBox = new Point(10, 10);

        /// <summary>
        /// Entity currently selected.
        /// </summary>
        private int _select = -1;

        /// <summary>
        /// Entity that is being dragged, if any.
        /// </summary>
        private Entity? _dragging = null;

        private float _dragTimer = 0;

        private Point HalfSelectionBox => new Point(_selectionBox.X / 2, _selectionBox.Y / 2);

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

                if (ImGui.Selectable($"{name}({entity.EntityId})##{name}_{entity.EntityId}", hook.Selected.Contains(entity.EntityId)))
                {
                    SelectEntity(hook, entity);
                }
                if (ImGui.IsItemHovered())
                {
                    hook.Hovering = entity.EntityId;
                }
            }

            ImGui.EndChild();
            ImGui.End();

            for (int i = hook.Selected.Count - 1; i >= 0; i--)
            {
                int selected = hook.Selected[i];

                if (context.World.TryGetEntity(selected) is Entity selectedEntity)
                {
                    ImGui.SetNextWindowBgAlpha(0.9f);
                    ImGui.SetNextWindowDockID(42, ImGuiCond.Appearing);

                    if (_select == selectedEntity.EntityId)
                    {
                        ImGui.SetNextWindowFocus();
                    }

                    if (hook.DrawEntityInspector is not null && !hook.DrawEntityInspector(selectedEntity))
                    {
                        hook.Selected.Remove(selected);
                    }
                }
            }

            return default;
        }

        public ValueTask Update(Context context)
        {
            _select = -1;

            var hook = context.World.GetUnique<EditorComponent>().EditorHook;
            var world = (MonoWorld)context.World;

            var clicked = Game.Input.Pressed(MurderInputButtons.LeftClick);
            var cursorPosition = world.Camera.GetCursorWorldPosition(hook.Offset, new(hook.StageSize.X, hook.StageSize.Y));
            hook.CursorWorldPosition = cursorPosition;

            hook.CursorScreenPosition = Game.Input.CursorPosition - hook.Offset;

            var bounds = new Rectangle(hook.Offset, hook.StageSize);
            var hasFocus = bounds.Contains(Game.Input.CursorPosition);

            if (hasFocus)
            {
                foreach (var e in context.Entities)
                {
                    Vector2 pos = e.GetGlobalTransform().Vector2;
                    var rect = new Rectangle(pos - _selectionBox / 2f, _selectionBox);

                    if (e.Parent is not null)
                    {
                        // Skip entities that are children.
                        continue;
                    }

                    if (rect.Contains(cursorPosition))
                    {
                        if (hook.Hovering != e.EntityId)
                        {
                            hook.Hovering = e.EntityId;
                            hook.OnHoverEntity?.Invoke(e);
                            hook.Cursor = EditorHook.CursorStyle.Point;
                        }

                        if (clicked)
                        {
                            hook.OnClickEntity?.Invoke(e);
                            SelectEntity(hook, e);
                            _dragging = e;
                        }

                        if (_dragging == e && Game.Input.Down(MurderInputButtons.LeftClick))
                        {
                            _dragTimer += Game.FixedDeltaTime;
                        }

                        break;
                    }
                    else if (hook.Hovering == e.EntityId)
                    {
                        hook.Hovering = -1;
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

            if (_dragTimer > DRAG_MIN_DURATION)
            {
                hook.Cursor = EditorHook.CursorStyle.Hand;
            }

            return default;
        }

        public ValueTask Draw(RenderContext render, Context context)
        {
            var hook = context.World.GetUnique<EditorComponent>().EditorHook;

            foreach (var e in context.Entities)
            {
                Vector2 pos = e.GetGlobalTransform().Vector2;

                if (e.Parent is not null)
                {
                    // Children are not selectable.
                    RenderServices.DrawCircle(render.DebugSpriteBatch, pos, 4, 4,
                        Game.Profile.Theme.GenericAsset.ToXnaColor());
                }
                else if (e.EntityId == hook.Hovering)
                {
                    var variation = Calculator.RoundToInt(MathF.Sin(Game.Instance.ElapsedTime * 6) + 1f);
                    Point variationBox = new Point(variation, variation);

                    RenderServices.DrawCircle(render.DebugSpriteBatch, pos, 4, 8,
                        Game.Profile.Theme.GenericAsset.ToXnaColor());
                }
                else if (hook.Selected.Contains(e.EntityId))
                {
                    RenderServices.DrawRectangleOutline(render.DebugSpriteBatch,
                        new Rectangle(pos.Point - HalfSelectionBox / 2f, HalfSelectionBox),
                        Game.Profile.Theme.HighAccent.ToXnaColor(), 1);
                }
                else
                {
                    RenderServices.DrawPoint(render.DebugSpriteBatch, pos.Point, Game.Profile.Theme.Accent.ToXnaColor());
                }
            }

            return default;
        }

        private void SelectEntity(EditorHook hook, Entity entity)
        {
            hook.Selected.AddOnce(entity.EntityId);
            _select = entity.EntityId;
        }
    }
}
