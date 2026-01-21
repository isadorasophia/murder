using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Filter(kind: ContextAccessorKind.Read, typeof(IStateMachineComponent))]
    [Watch(typeof(IStateMachineComponent))]
    public class StateMachineSystem : IStartupSystem, IUpdateSystem, IReactiveSystem, IExitSystem
    {
        public void Start(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                if (e.TryGetStateMachine() is not IStateMachineComponent routine)
                {
                    continue;
                }

                routine.Start();
            }
        }

        public virtual void Update(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                Update(e);
            }
        }

        protected void Update(Entity e)
        {
            if (e.TryGetStateMachine() is IStateMachineComponent routine)
            {
                float deltaTime = e.HasUnscaledDeltaTime() ?
                    Game.UnscaledDeltaTime : Game.DeltaTime;

                if (Game.Instance.IsSkippingDeltaTimeOnUpdate)
                {
                    deltaTime = 100;
                }

                routine.Tick(deltaTime);
            }
        }

        public void Exit(Context context)
        {
            Clean(context.Entities, force: true);
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

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            Cleanup(entities);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            Cleanup(entities);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            Cleanup(entities);
        }

        private void Cleanup(ImmutableArray<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                e.RemovePauseStateMachineUpdate();
            }
        }
    }
}