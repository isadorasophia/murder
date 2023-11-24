using Bang;
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
    [Watch(typeof(IStateMachineComponent))]
    public class StateMachineOnPauseSystem : IUpdateSystem, IReactiveSystem, IExitSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        { }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            Clean(entities, force: false);
        }

        public void Exit(Context context)
        {
            Clean(context.Entities, force: true);
        }

        public void Update(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                IStateMachineComponent routine = e.GetComponent<IStateMachineComponent>();
                routine.Tick(Game.UnscaledDeltaTime);
            }
        }

        private void Clean(ImmutableArray<Entity> entities, bool force)
        {
            foreach (Entity e in entities)
            {
                if (e.IsDestroyed || force)
                {
                    IStateMachineComponent? routine = e.TryGetStateMachine();
                    routine?.OnDestroyed();
                }
            }
        }
    }
}