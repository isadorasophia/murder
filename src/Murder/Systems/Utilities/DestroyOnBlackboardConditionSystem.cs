using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets;
using Murder.Components;
using Murder.Save;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems.Utilities
{
    [Filter(typeof(RuleWatcherComponent))]
    [Watch(typeof(RuleWatcherComponent))]
    internal class DestroyOnBlackboardConditionSystem : IReactiveSystem
    {
        private int _contextDestroyOnBlackboardCondition = -1;

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            CheckRules(world);
        }

        private void CheckRules(World world)
        {
            SaveData save = MurderSaveServices.CreateOrGetSave();
            BlackboardTracker tracker = save.BlackboardTracker;

            if (_contextDestroyOnBlackboardCondition == -1)
            {
                _contextDestroyOnBlackboardCondition = world.GetOrCreateContextIdFrom(ContextAccessorFilter.AllOf, typeof(DestroyOnBlackboardConditionComponent));
            }

            // Fetch all entities which might be affected by a rule change.
            ImmutableArray<Entity> interactives = world.GetEntitiesFrom(_contextDestroyOnBlackboardCondition);
            foreach (Entity e in interactives)
            {
                if (!e.IsActive)
                {
                    continue;
                }

                // Match each of its requirements.
                DestroyOnBlackboardConditionComponent ruleComponent = e.GetDestroyOnBlackboardCondition();

                bool matched = BlackboardHelpers.Match(world, tracker, ruleComponent.Rules);
                if (matched)
                {
                    RemoveStyle style = ruleComponent.RemoveWith ?? RemoveStyle.Destroy;
                    switch (style)
                    {
                        case RemoveStyle.Deactivate:
                            e.Deactivate();
                            break;

                        default:
                            e.Destroy();
                            break;
                    }
                }
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            CheckRules(world);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            // No need to check on removed, right? It's already gone.
        }
    }
}