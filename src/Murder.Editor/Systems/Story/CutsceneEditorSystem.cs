using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Serialization;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Core;
using Murder.Editor.Messages;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Editor.Systems
{
    [StoryEditor]
    [Filter(typeof(CutsceneAnchorsEditorComponent))]
    internal class CutsceneEditorSystem : IStartupSystem, IUpdateSystem, IMurderRenderSystem
    {
        private readonly static int _hash = typeof(CutsceneEditorSystem).GetHashCode();
        private readonly struct DraggedAnchor
        {
            public Entity Owner { init; get; }

            /// <summary>
            /// If null, this means that the cutscene owner is being dragged.
            /// </summary>
            public string? Id { init; get; }
        }

        /// <summary>
        /// Entity (or anchor) that is currently being dragged.
        /// </summary>
        private DraggedAnchor? _dragged = null;
        private Point _draggedOffset = Point.Zero;

        /// <summary>
        /// Anchor that is being hovered, if any.
        /// </summary>
        private DraggedAnchor? _hovered = null;

        private SpriteAsset _cameraTexture = null!;
        private SpriteAsset _anchorTexture = null!;

        private readonly Vector2 _hitBox = new Point(20, 20);

        public void Start(Context context)
        {
            _cameraTexture = Game.Data.TryGetAsset<SpriteAsset>(Game.Profile.EditorAssets.CutsceneImage)!;
            _anchorTexture = Game.Data.TryGetAsset<SpriteAsset>(Game.Profile.EditorAssets.AnchorImage)!;
        }

        public void Update(Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            // If user has selected to destroy entities.
            if (Game.Input.Pressed(MurderInputButtons.Delete) && _hovered != null)
            {
                Entity target = _hovered.Value.Owner;

                if (_hovered.Value.Id is string name)
                {
                    target.SetCutsceneAnchorsEditor(
                        target.GetCutsceneAnchorsEditor().WithoutAnchorAt(name));
                }
                else
                {
                    // Actually deleting an entire cutscene
                    hook.RemoveEntityWithStage?.Invoke(target.EntityId);
                }

                // Make sure we don't delete anything else.
                Game.Input.Consume(MurderInputButtons.Delete);
                _hovered = null;
            }

            MonoWorld world = (MonoWorld)context.World;
            
            // First, start by checking which entities were selected.
            // This is useful if we decide to move a group of anchors or entities.
            foreach (Entity e in context.Entities)
            {
                Vector2 position = e.GetGlobalTransform().Vector2;
                Rectangle cutsceneRect = new(position - _hitBox / 2f, _hitBox);
                if (hook.CursorWorldPosition!=null && cutsceneRect.Contains(hook.CursorWorldPosition.Value))
                {
                    hook.Cursor = CursorStyle.Point;
                    _hovered = new() { Owner = e, Id = null };

                    OnAnchorOrCameraHovered(hook, hook.CursorWorldPosition.Value, position.Point(), e, null);
                }
                else if (_hovered?.Owner?.EntityId == e.EntityId && _hovered?.Id == null)
                {
                    _hovered = null;
                }

                if (!hook.IsEntitySelected(e.EntityId))
                    continue;

                foreach (AnchorId anchor in e.GetCutsceneAnchorsEditor().Anchors)
                {
                    Vector2 anchorPosition = position + anchor.Anchor.Position;
                    Rectangle rect = new(anchorPosition - _hitBox / 2f, _hitBox);

                    if (hook.CursorWorldPosition != null && rect.Contains(hook.CursorWorldPosition.Value))
                    {
                        OnAnchorOrCameraHovered(hook, hook.CursorWorldPosition.Value, anchorPosition.Point(), e, anchor.Id);
                    }
                    else if (_hovered?.Owner?.EntityId == e.EntityId && _hovered?.Id == anchor.Id)
                    {
                        _hovered = null;
                    }
                }
            }

            // Now, actually drag whatever entities were selected.
            if (_dragged is DraggedAnchor draggedAnchor)
            {
                hook.Cursor = CursorStyle.Hand;
                hook.CursorIsBusy.Add(typeof(CutsceneEditorSystem));

                Entity dragged = draggedAnchor.Owner;
                string? name = draggedAnchor.Id;

                hook.SelectEntity(dragged, true);

                IMurderTransformComponent cutsceneTransform = dragged.GetGlobalTransform();

                // On "ctrl", snap entities to the grid.
                bool snapToGrid = Game.Input.Down(MurderInputButtons.Ctrl);
                if (hook.CursorWorldPosition is Point cursorPosition)
                {
                    if (name is null)
                    {
                        // This is actually dragging the root of the cutscene.
                        Vector2 delta = cursorPosition - cutsceneTransform.Vector2 - _draggedOffset;

                        IMurderTransformComponent newTransform = cutsceneTransform.Add(delta);
                        if (snapToGrid)
                        {
                            newTransform = newTransform.SnapToGridDelta();
                        }

                        dragged.SetGlobalTransform(newTransform);
                        dragged.SendMessage(new AssetUpdatedMessage(typeof(PositionComponent)));
                    }
                    else
                    {
                        Vector2 anchorPosition = dragged.GetCutsceneAnchorsEditor().FindAnchor(name).Position;
                        Vector2 delta = cursorPosition - cutsceneTransform.Vector2 - anchorPosition;

                        Vector2 finalNewPosition = anchorPosition + delta - _draggedOffset;

                        if (snapToGrid)
                        {
                            finalNewPosition = finalNewPosition.SnapToGridDelta();
                        }

                        dragged.SetCutsceneAnchorsEditor(
                            dragged.GetCutsceneAnchorsEditor().WithAnchorAt(name, finalNewPosition));
                    }
                }
            }
            if (!Game.Input.Down(MurderInputButtons.LeftClick))
            {
                // The user stopped clicking, so no longer drag anything.
                _dragged = null;
                hook.CursorIsBusy.Remove(typeof(CutsceneEditorSystem));
            }
        }

        private void OnAnchorOrCameraHovered(EditorHook hook, Point position, Point hoveredPosition, Entity e, string? name)
        {
            bool clicked = Game.Input.Pressed(MurderInputButtons.LeftClick);

            hook.Cursor = CursorStyle.Point;
            _hovered = new() { Owner = e, Id = name };

            if (clicked)
            {
                _draggedOffset = position - hoveredPosition;
                _dragged = _hovered;
            }

            if (Game.Input.Released(MurderInputButtons.LeftClick))
            {
                _tweenStart = Game.Now;
                _selectPosition = position;
            }
        }

        public void Draw(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            DrawSelectionTween(render);
            DrawAnchors(render, context, hook);

            if (_hovered == null)
            {
                DrawAddCutsceneOrAnchor(hook, context);
            }
        }

        private void DrawAnchors(RenderContext render, Context context, EditorHook hook)
        {
            int lineWidth = Math.Max(Calculator.RoundToInt(2f / render.Camera.Zoom), 1);

            foreach (Entity e in context.Entities)
            {
                Vector2 position = e.GetGlobalTransform().Vector2;

                ImmutableArray<AnchorId> anchors = e.GetCutsceneAnchorsEditor().Anchors;

                bool isCutsceneSelected = _hovered?.Owner?.EntityId == e.EntityId && _hovered?.Id == null;
                RenderSprite(render, _cameraTexture, position, isCutsceneSelected);

                if (hook.GetNameForEntityId?.Invoke(e.EntityId) is string cutsceneName)
                {
                    DrawText(render, cutsceneName, position);
                }

                if (hook.IsEntitySelected(e.EntityId))
                {
                    RenderServices.DrawRectangleOutline(render.DebugFxBatch, FindContainingArea(position, anchors), Game.Profile.Theme.White * .2f, 1);

                    foreach (AnchorId anchor in anchors)
                    {
                        bool isAnchorSelected = _hovered?.Owner?.EntityId == e.EntityId && _hovered?.Id == anchor.Id;

                        Vector2 anchorPosition = position + anchor.Anchor.Position;
                        RenderSprite(render, _anchorTexture, anchorPosition, isCutsceneSelected || isAnchorSelected);

                        // Also draw the preview of what the camera would see from this anchor.
                        if (isAnchorSelected)
                        {
                            Vector2 size = new Vector2(Game.Profile.GameWidth, Game.Profile.GameHeight);
                            Rectangle rect = new(anchorPosition - size / 2f, size);
                            RenderServices.DrawRectangleOutline(render.GameUiBatch, rect, Game.Profile.Theme.Yellow, lineWidth);
                        }

                        DrawText(render, anchor.Id, anchorPosition);
                    }
                }

                var distance = (position - hook.CursorWorldPosition)?.Length() / 128f * render.Camera.Zoom;
                if (distance != null && distance.Value < 1) 
                {
                    RenderServices.DrawCircleOutline(render.DebugBatch, position, 2, 6, Game.Profile.Theme.Yellow * (1 - distance.Value));
                }
            }
        }

        private Rectangle FindContainingArea(Vector2 initialPosition, ImmutableArray<AnchorId> anchors)
        {
            Vector2 minPoint = initialPosition;
            Vector2 maxPoint = initialPosition;

            foreach (AnchorId anchor in anchors)
            {
                Vector2 anchorPosition = initialPosition + anchor.Anchor.Position;

                if (anchorPosition.X < minPoint.X)
                {
                    minPoint.X = anchorPosition.X;
                }

                if (anchorPosition.Y < minPoint.Y)
                {
                    minPoint.Y = anchorPosition.Y;
                }

                if (anchorPosition.X > maxPoint.X)
                {
                    maxPoint.X = anchorPosition.X;
                }

                if (anchorPosition.Y > maxPoint.Y)
                {
                    maxPoint.Y = anchorPosition.Y;
                }
            }

            return GridHelper.FromTopLeftToBottomRight(minPoint, maxPoint);
        }

        /// <summary>
        /// Draw a text right below <paramref name="position"/>.
        /// </summary>
        private void DrawText(RenderContext render, string text, Vector2 position)
        {
            int lineWidth = Math.Max(Calculator.RoundToInt(2f / render.Camera.Zoom), 1);

            RenderServices.DrawText(render.DebugBatch, MurderFonts.PixelFont, text, position + new Vector2(0, lineWidth), new DrawInfo(0)
            {
                Origin = new Vector2(0.5f, -1),
                Color = Color.White,
                Scale = new Vector2(lineWidth)
            });
        }

        private void RenderSprite(RenderContext render, SpriteAsset asset, Vector2 position, bool isHighlighted)
        {
            RenderServices.DrawSprite(
                render.GameUiBatch,
                asset,
                position,
                new DrawInfo(RenderServices.YSort(isHighlighted ? 0 : position.Y))
                {
                    Outline = isHighlighted ? Color.White * 0.6f : null
                },
                new AnimationInfo()
                {
                    UseScaledTime = false
                });

        }

        /// <summary>
        /// Tracks tween for <see cref="_selectPosition"/>.
        /// </summary>
        private float _tweenStart;

        /// <summary>
        /// Tracks the pretty tween!
        /// </summary>
        private Vector2? _selectPosition;

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
                    Color color = Game.Profile.Theme.White * (startAlpha - startAlpha * tween);

                    Vector2 size = _hitBox.Add(expand * 2);
                    Rectangle rectangle = new(position - size / 2f, size);

                    RenderServices.DrawRectangleOutline(render.DebugBatch, rectangle, color);
                }
            }
        }

        /// <summary>
        /// This adds a new cutscene or a new anchor.
        /// </summary>
        private bool DrawAddCutsceneOrAnchor(EditorHook hook, Context context)
        {
            if (ImGui.BeginPopupContextItem("GameplayContextMenu", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoReopen))
            {
                if (ImGui.Selectable("Add anchor"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;

                    float bestMatch = int.MaxValue;
                    Entity? cutscene = default;

                    if (hook.AllSelectedEntities.Count == 1)
                    {
                        cutscene = context.World.TryGetEntity(hook.AllSelectedEntities.Keys.First());
                    }
                    else
                    {
                        // Let's find the closest cutscene to add this anchor to.
                        foreach (Entity e in context.Entities)
                        {
                            Vector2 distance = cursorWorldPosition - e.GetMurderTransform().Vector2;
                            float length = distance.LengthSquared();

                            if (length < bestMatch)
                            {
                                bestMatch = length;
                                cutscene = e;
                            }
                        }
                    }

                    if (cutscene == null)
                    {
                        CreateNewCutscene(
                            hook,
                            cursorWorldPosition,
                            new AnchorId[] { new("0", new(Vector2.Zero)) }.ToImmutableArray());
                    }
                    else
                    {
                        Vector2 relativePosition = cursorWorldPosition - cutscene.GetMurderTransform().Vector2;
                        CutsceneAnchorsEditorComponent anchorsComponents = cutscene.TryGetCutsceneAnchorsEditor() ?? new CutsceneAnchorsEditorComponent();

                        cutscene.SetCutsceneAnchorsEditor(anchorsComponents.AddAnchorAt(relativePosition));
                    }
                }

                if (ImGui.Selectable("Add cutscene"))
                {
                    CreateNewCutscene(hook, hook.LastCursorWorldPosition, ImmutableArray<AnchorId>.Empty);
                }

                ImGui.EndPopup();
            }

            return true;
        }

        private void CreateNewCutscene(EditorHook hook, Vector2 position, ImmutableArray<AnchorId> anchors)
        {
            hook.AddEntityWithStage?.Invoke(
                new IComponent[]
                {
                    new PositionComponent(position),
                    new CutsceneAnchorsEditorComponent(anchors)
                },
                /* group */ null,
                /* name */ null);
        }
    }
}