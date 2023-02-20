using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Bang.StateMachines;
using Bang;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Filter(kind: ContextAccessorKind.Read, typeof(IStateMachineComponent))]
    [Watch(typeof(IStateMachineComponent))]
    public class StateMachineSystem : IUpdateSystem, IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities) { }

        public void OnModified(World world, ImmutableArray<Entity> entities) { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                if (e.IsDestroyed)
                {
                    IStateMachineComponent? routine = e.TryGetStateMachine();
                    routine?.OnDestroyed();
                }
            }
        }

        public void Update(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                IStateMachineComponent routine = e.GetStateMachine();
                routine.Tick(Game.DeltaTime);
            }
        }
    }
}
