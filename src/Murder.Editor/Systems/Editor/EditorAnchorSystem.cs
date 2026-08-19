using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Cutscenes;
using Murder.Components.Serialization;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Messages;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.Systems.Editor;

[WorldEditor(startActive: true)]
[OnlyShowOnDebugView]
[Filter(kind: ContextAccessorKind.Read, typeof(CutsceneAnchorsEditorComponent), typeof(PositionComponent))]
public class EditorAnchorSystem : IUpdateSystem, IMurderRenderSystem
{
    private Vector2 _starDrag;
    private int _draggingIndex;

    private bool _isCursorPressed = false;

    public void Update(Context context)
    {
        if (Game.Input.Pressed(MurderInputButtons.LeftClick))
        {
            _isCursorPressed = true;
        }
        else
        {
            _isCursorPressed = false;
        }
    }

    public void Draw(RenderContext render, Context context)
    {
        if (context.World.TryGetUnique<EditorComponent>() is not EditorComponent editor)
        {
            return;
        }

        EditorHook hook = editor.EditorHook;
        if (hook.CursorWorldPosition is not Point cursor)
        {
            hook.CursorIsBusy.Remove(typeof(CutsceneEditorSystem));
            return;
        }

        bool anyHovered = false;
        foreach (var e in context.Entities)
        {
            bool showHandles =
                (hook.EditorMode == EditorHook.EditorModes.EditMode && (!hook.CanSwitchModes || hook.IsEntitySelectedOrParent(e))) &&
                (hook.CursorIsBusy.Count == 1 && hook.CursorIsBusy.Contains(typeof(CutsceneEditorSystem)) || !hook.CursorIsBusy.Any());

            if (!showHandles)
            {
                continue;
            }

            CutsceneAnchorsEditorComponent cutsceneAnchors = e.GetComponent<CutsceneAnchorsEditorComponent>();
            Vector2 position = e.GetGlobalPosition();

            if (!Game.Input.Down(MurderInputButtons.LeftClick))
            {
                _draggingIndex = -1;
            }

            for (int i = 0; i < cutsceneAnchors.Anchors.Length; i++)
            {
                AnchorId anchor = cutsceneAnchors.Anchors[i];
                Vector2 anchorPosition = anchor.Anchor.Position + position;

                bool hovered = _draggingIndex == i || Calculator.DistanceCheck(cursor, anchorPosition, 8);

                RenderServices.DrawSprite(render.DebugBatch, Game.Profile.EditorAssets.PointAnchorImage, anchorPosition, new DrawInfo(RenderServices.YSort(position.Y))
                {
                    Outline = hovered ? Color.White : null,
                    Color = hovered ? Color.White : Color.White * 0.5f
                });

                if (hovered)
                {
                    anyHovered = true;

                    if (_isCursorPressed)
                    {
                        _starDrag = anchorPosition - cursor;
                        _draggingIndex = i;
                    }

                    RenderServices.DrawText(render.DebugBatch, MurderFonts.PixelFont, anchor.Id, anchorPosition + new Vector2(0, -16), new DrawInfo(0)
                    {
                        Outline = Color.Black,
                        Color = Color.White,
                        Origin = new Vector2(0.5f, 1)
                    });
                }

                if (_draggingIndex == i)
                {
                    Vector2 newPosition = cursor + _starDrag - position;
                    e.ReplaceComponent(cutsceneAnchors.WithAnchorAt(anchor.Id, newPosition));
                    e.SendMessage(new AssetUpdatedMessage(typeof(CutsceneAnchorsEditorComponent)));
                }
            }
        }

        if (anyHovered)
        {
            hook.CursorIsBusy.Add(typeof(CutsceneEditorSystem));
        }
        else
        {
            hook.CursorIsBusy.Remove(typeof(CutsceneEditorSystem));
        }
    }
}

[WorldEditor(startActive: true)]
[OnlyShowOnDebugView]
[Filter(typeof(OverrideCenterOffsetComponent), typeof(PositionComponent))]
public class EditorCenterOffsetSystem : IUpdateSystem, IMurderRenderSystem
{
    private Vector2 _starDrag;
    private bool _dragging = false;

    private bool _isCursorPressed = false;

    public void Update(Context context)
    {
        if (Game.Input.Pressed(MurderInputButtons.LeftClick))
        {
            _isCursorPressed = true;
        }
        else
        {
            _isCursorPressed = false;
        }
    }

    public void Draw(RenderContext render, Context context)
    {
        if (context.World.TryGetUnique<EditorComponent>() is not EditorComponent editor)
        {
            return;
        }

        EditorHook hook = editor.EditorHook;
        if (hook.CursorWorldPosition is not Point cursor)
        {
            hook.CursorIsBusy.Remove(typeof(EditorCenterOffsetSystem));
            return;
        }

        bool anyHovered = false;
        foreach (Entity e in context.Entities)
        {
            bool showHandles =
                (hook.EditorMode == EditorHook.EditorModes.EditMode && (!hook.CanSwitchModes || hook.IsEntitySelectedOrParent(e))) &&
                (hook.CursorIsBusy.Count == 1 && hook.CursorIsBusy.Contains(typeof(EditorCenterOffsetSystem)) || !hook.CursorIsBusy.Any());

            if (!showHandles)
            {
                continue;
            }

            OverrideCenterOffsetComponent overrideOffset = e.GetOverrideCenterOffset();
            Vector2 position = e.GetGlobalPosition();

            if (!Game.Input.Down(MurderInputButtons.LeftClick))
            {
                _dragging = false;
            }

            Vector2 anchorPosition = overrideOffset.Offset + position;

            bool hovered = _dragging || Calculator.DistanceCheck(cursor, anchorPosition, 8);

            RenderServices.DrawSprite(render.DebugBatch, Game.Profile.EditorAssets.PointAnchorImage, anchorPosition, new DrawInfo(RenderServices.YSort(position.Y))
            {
                Outline = hovered ? Color.White : null,
                Color = hovered ? Color.White : Color.White * 0.5f
            });

            if (hovered)
            {
                anyHovered = true;

                if (_isCursorPressed)
                {
                    _starDrag = anchorPosition - cursor;
                    _dragging = true;
                }
            }

            if (_dragging)
            {
                Vector2 newPosition = cursor + _starDrag - position;
                e.ReplaceComponent(new OverrideCenterOffsetComponent(newPosition));
                e.SendMessage(new AssetUpdatedMessage(typeof(OverrideCenterOffsetComponent)));
            }
        }

        if (anyHovered)
        {
            hook.CursorIsBusy.Add(typeof(EditorCenterOffsetSystem));
        }
        else
        {
            hook.CursorIsBusy.Remove(typeof(EditorCenterOffsetSystem));
        }
    }
}
