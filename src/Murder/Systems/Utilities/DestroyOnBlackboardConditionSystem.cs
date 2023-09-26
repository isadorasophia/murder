using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Interactions;
using Bang.Systems;
using Murder.Assets;
using Murder.Components;
using Murder.Interactions;
using Murder.Messages;
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
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            CheckRules(world);
        }

        private void CheckRules(World world)
        {
            SaveData save = MurderSaveServices.CreateOrGetSave();
            BlackboardTracker tracker = save.BlackboardTracker;

            // Fetch all entities which might be affected by a rule change.
            ImmutableArray<Entity> interactives = world.GetEntitiesWith(typeof(DestroyOnBlackboardConditionComponent));
            foreach (Entity e in interactives)
            {
                if (e.IsDestroyed)
                {
                    continue;
                }

                // Match each of its requirements.
                DestroyOnBlackboardConditionComponent ruleComponent = e.GetDestroyOnBlackboardCondition();

                bool matched = BlackboardHelpers.Match(world, tracker, ruleComponent.Rules);

                if (matched)
                {
                    e.Destroy();
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
