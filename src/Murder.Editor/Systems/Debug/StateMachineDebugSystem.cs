using Bang.Contexts;
using Bang.StateMachines;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Bang.Entities;
using Murder.Utilities;

namespace Murder.Editor.Systems
{
    [Filter(typeof(IStateMachineComponent))]
    internal class StateMachineDebugSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            if (context.World.TryGetUnique<EditorComponent>()?.EditorHook is EditorHook hook && hook.ShowStates)
            {
                foreach (var e in context.Entities)
                {
                    if (e.IsDestroyed || !e.HasTransform())
                        continue;
                    Game.Data.PixelFont.Draw(render.DebugSpriteBatch, e.GetStateMachine().State, 1, e.GetGlobalTransform().Vector2, 0f, Color.Black);
                }
            }
        }
    }
}
