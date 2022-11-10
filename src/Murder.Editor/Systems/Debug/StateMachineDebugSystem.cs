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
        public ValueTask Draw(RenderContext render, Context context)
        {
            if (context.World.TryGetUnique<EditorComponent>()?.EditorHook is EditorHook hook && hook.ShowStates)
            {
                foreach (var e in context.Entities)
                {
                    Game.Data.PixelFont.Draw(7, render.DebugSpriteBatch, e.GetStateMachine().State, e.GetGlobalTransform().Vector2, Color.Black);
                }
            }

            return default;
        }
    }
}
