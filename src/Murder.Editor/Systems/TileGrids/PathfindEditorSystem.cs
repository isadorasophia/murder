using Bang.Contexts;
using Bang.Systems;
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

namespace Murder.Editor.Systems;

[PathfindEditor]
[Filter(typeof(PathfindGridComponent))]
public class PathfindEditorSystem : IStartupSystem, IUpdateSystem, IGuiSystem, IMurderRenderSystem
{
    private float _tweenStart;

    public void Start(Context context)
    {
        if (!context.HasAnyEntity)
        {
            // If there is no entity, make sure we have one before operating over this system.
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            hook.AddEntityWithStage?.Invoke(
                [new PathfindGridComponent()], 
                /* group: */ string.Empty, 
                /* name: */ "Pathfind Grid Entity");
        }
    }

    public void Update(Context context)
    {
    }

    public void DrawGui(RenderContext render, Context context)
    {
    }

    public void Draw(RenderContext render, Context context)
    {
        EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

        if (hook.CursorWorldPosition is not Point cursorWorldPosition)
        {
            return;
        }

        Point cursorGridPosition = cursorWorldPosition.FromWorldToLowerBoundGridPosition();
        IntRectangle rectangle = new Rectangle(cursorGridPosition.X, cursorGridPosition.Y, 1, 1);

        Color color = Game.Profile.Theme.White;
        color *= .5f;

        RenderServices.DrawRectangleOutline(render.DebugBatch, (rectangle * Grid.CellSize).Expand(
            4 - 3 * Ease.ZeroToOne(Ease.CubeInOut, 0.250f, _tweenStart)), color);

        if (Game.Input.Pressed(MurderInputButtons.LeftClick))
        {
            // cute
            _tweenStart = Game.Now;
        }

        if (Game.Input.Down(MurderInputButtons.LeftClick))
        {
            // actually apply
        }
        else if (Game.Input.Down(MurderInputButtons.RightClick))
        {
            // deapply!!
        }
    }
}