using Bang.Contexts;
using Bang.StateMachines;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Bang.Entities;
using Murder.Utilities;
using Murder.Services;
using Murder.Editor.Attributes;

namespace Murder.Editor.Systems;

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
                    continue;
                RenderServices.DrawText(render.DebugBatch, MurderFonts.PixelFont, e.GetStateMachine().State, e.GetGlobalTransform().Vector2,
                    new DrawInfo(0f) { Color = Color.Black });
            }
        }
    }
}
