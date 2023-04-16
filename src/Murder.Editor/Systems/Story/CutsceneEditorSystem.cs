using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Cutscenes;
using Murder.Core;
using Murder.Core.Cutscenes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Data;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.Systems
{
    [StoryEditor]
    [Filter(typeof(CutsceneAnchorsComponent))]
    internal class CutsceneEditorSystem : IStartupSystem, IUpdateSystem, IMonoRenderSystem
    {
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

        /// <summary>
        /// Anchor that is being hovered, if any.
        /// </summary>
        private DraggedAnchor? _hovered = null;
        private float _dragTimer = 0;

        private AsepriteAsset _cameraTexture = null!;
        private AsepriteAsset _anchorTexture = null!;
        
        private readonly Vector2 _hitBox = new Point(20, 20);

        public void Start(Context context)
        {
            _cameraTexture = Game.Data.TryGetAsset<AsepriteAsset>(Game.Profile.EditorAssets.CutsceneImage)!;
            _anchorTexture = Game.Data.TryGetAsset<AsepriteAsset>(Game.Profile.EditorAssets.AnchorImage)!;
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
                    target.SetCutsceneAnchors(
                        target.GetCutsceneAnchors().WithoutAnchorAt(name));
                }
                else
                {
                    // Actually deleting an entire cutscene
                    hook.RemoveEntityWithStage?.Invoke(target.EntityId);
                }

                _hovered = null;
            }

            Rectangle bounds = new(hook.Offset, hook.StageSize);
            bool hasFocus = bounds.Contains(Game.Input.CursorPosition);
            
            MonoWorld world = (MonoWorld)context.World;
            Point cursorPosition = world.Camera.GetCursorWorldPosition(hook.Offset, new(hook.StageSize.X, hook.StageSize.Y));

            // First, start by checking which entities were selected.
            // This is useful if we decide to move a group of anchors or entities.
            foreach (Entity e in context.Entities)
            {
                Vector2 position = e.GetGlobalTransform().Vector2;
                Rectangle cutsceneRect = new(position - _hitBox / 2f, _hitBox);
                if (hasFocus && cutsceneRect.Contains(cursorPosition))
                {
                    hook.Cursor = EditorHook.CursorStyle.Point;
                    _hovered = new() { Owner = e, Id = null };

                    OnAnchorOrCameraHovered(hook, cursorPosition, e, null);
                }
                else if (_hovered?.Owner?.EntityId == e.EntityId && _hovered?.Id == null)
                {
                    _hovered = null;
                }

                foreach ((string name, Anchor anchor) in e.GetCutsceneAnchors().Anchors)
                {
                    Vector2 anchorPosition = position + anchor.Position;
                    Rectangle rect = new(anchorPosition - _hitBox / 2f, _hitBox);

                    if (hasFocus && rect.Contains(cursorPosition))
                    {
                        OnAnchorOrCameraHovered(hook, cursorPosition, e, name);
                    }
                    else if (_hovered?.Owner?.EntityId == e.EntityId && _hovered?.Id == name)
                    {
                        _hovered = null;
                    }
                }
            }

            // Now, actually drag whatever entities were selected.
            if (_dragTimer > EntitiesSelectorSystem.DRAG_MIN_DURATION && _dragged is DraggedAnchor draggedAnchor)
            {
                hook.Cursor = EditorHook.CursorStyle.Hand;
                
                Entity dragged = draggedAnchor.Owner;
                string? name = draggedAnchor.Id;

                IMurderTransformComponent cutsceneTransform = dragged.GetGlobalTransform();
                
                // On "ctrl", snap entities to the grid.
                bool snapToGrid = Game.Input.Down(MurderInputButtons.Ctrl);
                
                if (name is null)
                {
                    // This is actually dragging the root of the cutscene.
                    Vector2 delta = cursorPosition - cutsceneTransform.Vector2;

                    IMurderTransformComponent newTransform = cutsceneTransform.Add(delta);
                    if (snapToGrid)
                    {
                        newTransform = newTransform.SnapToGridDelta();
                    }

                    dragged.SetGlobalTransform(newTransform);
                }
                else
                {
                    Vector2 anchorPosition = dragged.GetCutsceneAnchors().Anchors[name].Position;
                    Vector2 delta = cursorPosition - cutsceneTransform.Vector2 - anchorPosition;

                    Vector2 finalNewPosition = anchorPosition + delta;
                    
                    if (snapToGrid)
                    {
                        finalNewPosition = finalNewPosition.SnapToGridDelta();
                    }

                    dragged.SetCutsceneAnchors(
                        dragged.GetCutsceneAnchors().WithAnchorAt(name, finalNewPosition));
                }
            }
            
            if (!Game.Input.Down(MurderInputButtons.LeftClick))
            {
                // The user stopped clicking, so no longer drag anything.
                _dragTimer = 0;
                _dragged = null;
            }
        }

        private void OnAnchorOrCameraHovered(EditorHook hook, Point position, Entity e, string? name)
        {
            bool clicked = Game.Input.Pressed(MurderInputButtons.LeftClick);

            hook.Cursor = EditorHook.CursorStyle.Point;
            _hovered = new() { Owner = e, Id = name };

            if (clicked)
            {
                _dragged = _hovered;
            }

            if (_dragged?.Owner == e && Game.Input.Down(MurderInputButtons.LeftClick))
            {
                _dragTimer += Game.FixedDeltaTime;
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

                ImmutableDictionary<string, Anchor> anchors = e.GetCutsceneAnchors().Anchors;
                RenderServices.DrawRectangleOutline(render.DebugFxSpriteBatch, FindContainingArea(position, anchors.Values), Game.Profile.Theme.White, 1);

                bool isCutsceneSelected = _hovered?.Owner?.EntityId == e.EntityId && _hovered?.Id == null;
                RenderSprite(render, _cameraTexture, position, isCutsceneSelected);

                if (hook.GetNameForEntityId?.Invoke(e.EntityId) is string cutsceneName)
                {
                    DrawText(render, cutsceneName, position);
                }
                
                foreach ((string name, Anchor anchor) in anchors)
                {
                    bool isAnchorSelected = _hovered?.Owner?.EntityId == e.EntityId && _hovered?.Id == name;

                    Vector2 anchorPosition = position + anchor.Position;
                    RenderSprite(render, _anchorTexture, anchorPosition, isCutsceneSelected || isAnchorSelected);

                    // Also draw the preview of what the camera would see from this anchor.
                    if (isAnchorSelected)
                    {
                        Vector2 size = new Vector2(Game.Profile.GameWidth, Game.Profile.GameHeight);
                        Rectangle rect = new(anchorPosition - size / 2f, size);
                        RenderServices.DrawRectangleOutline(render.GameUiBatch, rect, Game.Profile.Theme.Yellow, lineWidth);
                    }

                    DrawText(render, name, anchorPosition);
                }

                var distance = (position - hook.CursorWorldPosition).Length() / 128f * render.Camera.Zoom;
                if (distance < 1)
                {
                    RenderServices.DrawCircle(render.DebugSpriteBatch, position, 2, 6, Game.Profile.Theme.Yellow * (1 - distance));
                }
            }
        }
        
        private Rectangle FindContainingArea(Vector2 initialPosition, IEnumerable<Anchor> anchors)
        {
            Vector2 minPoint = initialPosition;
            Vector2 maxPoint = initialPosition;

            foreach (Anchor anchor in anchors)
            {
                Vector2 anchorPosition = initialPosition + anchor.Position;

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

            Game.Data.PixelFont.Draw(render.DebugSpriteBatch, text, lineWidth, position + new Vector2(0, lineWidth), alignment: new Vector2(0.5f, -1), 0f, Color.White);
        }

        private void RenderSprite(RenderContext render, AsepriteAsset asset, Vector2 position, bool isHighlighted)
        {
            if (isHighlighted)
            {
                RenderServices.DrawSpriteWithOutline(
                    spriteBatch: render.GameUiBatch,
                    pos: position,
                    animationId: string.Empty,
                    ase: asset,
                    animationStartedTime: 0,
                    animationDuration: 0,
                    offset: Vector2.Zero,
                    flipped: false,
                    rotation: 0,
                    color: Color.White,
                    blend: RenderServices.BLEND_NORMAL,
                    sort: 0);
            }
            else
            {
                RenderServices.DrawSprite(
                    spriteBatch: render.GameUiBatch,
                    pos: position,
                    rotation: 0f,
                    scale: Vector2.One,
                    animationId: string.Empty,
                    ase: asset,
                    animationStartedTime: 0,
                    color: Color.White,
                    blend: RenderServices.BLEND_NORMAL
                    );
            }
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

                    Vector2 size = _hitBox + expand * 2;
                    Rectangle rectangle = new(position - size / 2f, size);

                    RenderServices.DrawRectangleOutline(render.DebugSpriteBatch, rectangle, color);
                }
            }
        }

        /// <summary>
        /// This adds a new cutscene or a new anchor.
        /// </summary>
        private bool DrawAddCutsceneOrAnchor(EditorHook hook, Context context)
        {
            ImGui.PushID("Popup!");
            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.Selectable("Add anchor"))
                {
                    Point cursorWorldPosition = hook.CursorWorldPosition;

                    float bestMatch = int.MaxValue;
                    Entity? cutscene = default;

                    // Let's find the closest cutscene to add this anchor to.
                    foreach (Entity e in context.Entities)
                    {
                        Vector2 distance = cursorWorldPosition - e.GetTransform().Vector2;
                        float length = distance.LengthSquared();

                        if (length < bestMatch)
                        {
                            bestMatch = length;
                            cutscene = e;
                        }
                    }

                    if (cutscene == null)
                    {
                        CreateNewCutscene(
                            hook, 
                            cursorWorldPosition, 
                            new Dictionary<string, Anchor>() { { "0", new(Vector2.Zero) } }.ToImmutableDictionary());
                    }
                    else
                    {
                        Vector2 relativePosition = cursorWorldPosition - cutscene.GetTransform().Vector2;
                        CutsceneAnchorsComponent anchorsComponents = cutscene.GetCutsceneAnchors();

                        cutscene.SetCutsceneAnchors(anchorsComponents.AddAnchorAt(relativePosition));
                    }
                }

                if (ImGui.Selectable("Add cutscene"))
                {
                    Point cursorWorldPosition = hook.CursorWorldPosition;
                    CreateNewCutscene(hook, cursorWorldPosition, ImmutableDictionary<string, Anchor>.Empty);
                }

                ImGui.EndPopup();
            }

            ImGui.PopID();

            return true;
        }

        private void CreateNewCutscene(EditorHook hook, Vector2 position, ImmutableDictionary<string, Anchor> anchors)
        {
            hook.AddEntityWithStage?.Invoke(
                new IComponent[]
                {
                            new PositionComponent(position),
                            new CutsceneAnchorsComponent(anchors)
                },
                /* group */ null);
        }
    }
}
