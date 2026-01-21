using Bang.Contexts;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using Murder.Components;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [OnPause]
    [Filter(typeof(IStateMachineComponent), typeof(DoNotPauseComponent))]
    public class StateMachineOnPauseSystem : IUpdateSystem, IExitSystem
    {
        public void Exit(Context context)
        {
            Clean(context.Entities, force: true);
        }

        public void Update(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                if (e.TryGetStateMachine() is not IStateMachineComponent routine)
                {
                    continue;
                }

                routine.Tick(Game.UnscaledDeltaTime);
            }
        }

        private void Clean(ImmutableArray<Entity> entities, bool force)
        {
            foreach (Entity e in entities)
            {
                if (e.TryGetStateMachine() is not IDisposable disposableRoutine)
                {
                    continue;
                }

                if (e.IsDestroyed || force)
                {
                    disposableRoutine.Dispose();
                }
            }
        }
    }
}