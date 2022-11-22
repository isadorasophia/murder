using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Services;

namespace Murder.Editor.Systems
{
    [OnlyShowOnDebugView]
    [Filter(typeof(TargetInteractionComponent))]
    public class DebugShowInteractionsSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            if (context.World.TryGetUnique<EditorComponent>() is EditorComponent editorComponent && !editorComponent.EditorHook.DrawTargetInteractions)
            {
                return default;
            }

            foreach (Entity e in context.Entities)
            {
                if (context.World.TryGetEntity(e.GetTargetInteraction().Target) is Entity target && target.HasTransform())
                {
                    RenderServices.DrawLine(render.DebugFxSpriteBatch, e.GetTransform().Point, target.GetTransform().Point, Color.White);
                }
            }

            return default;
        }
    }
}
