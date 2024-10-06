using Bang.Contexts;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Editor.Systems;

[OnlyShowOnDebugView]
[Filter(typeof(IStateMachineComponent))]
internal class StateMachineDebugSystem : IMurderRenderSystem
{
    public void Draw(RenderContext render, Context context)
    {
        if (context.World.TryGetUnique<EditorComponent>()?.EditorHook is EditorHook hook && hook.ShowStates)
        {
            foreach (var e in context.Entities)
            {
                if (e.IsDestroyed || !e.HasTransform())
                {
                    continue;
                }

                RenderServices.DrawText(render.DebugBatch, MurderFonts.PixelFont, e.GetStateMachine().State, e.GetGlobalTransform().Vector2,
                    new DrawInfo(0f) { Color = Color.White, Outline = Color.Black });
            }
        }
    }
}