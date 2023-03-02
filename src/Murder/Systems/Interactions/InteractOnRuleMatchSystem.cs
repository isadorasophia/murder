using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using System.Collections.Immutable;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using Murder.Messages;
using Murder.Save;
using Murder.Utilities;
using Murder.Assets;
using Bang.Interactions;
using Murder.Interactions;

namespace Murder.Systems
{
    [Filter(typeof(RuleWatcherComponent))]
    [Watch(typeof(RuleWatcherComponent))]
    internal class InteractOnRuleMatchSystem : IStartupSystem, IReactiveSystem
    {
        public void Start(Context context)
        {
            GameLogger.Verify(context.World.TryGetUniqueEntity<RuleWatcherComponent>() is not Entity,
                "Why did we already add an existing rule watcher component!?");

            _ = context.World.AddEntity(new RuleWatcherComponent());
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            CheckAndTriggerRules(world);
        }
        
        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            CheckAndTriggerRules(world);
        }
        
        public void OnRemoved(World world, ImmutableArray<Entity> entities) { }

        private void CheckAndTriggerRules(World world)
        {
            SaveData save = MurderSaveServices.CreateOrGetSave();
            BlackboardTracker tracker = save.BlackboardTracker;

            // Fetch all entities which might be affected by a rule change.
            ImmutableArray<Entity> interactives = world.GetEntitiesWith(typeof(InteractOnRuleMatchComponent));
            foreach (Entity e in interactives)
            {
                if (e.IsDestroyed)
                {
                    continue;
                }
                
                // Match each of its requirements.
                InteractOnRuleMatchComponent ruleComponent = e.GetInteractOnRuleMatch();
                if (ruleComponent.Triggered)
                {
                    // Skip components that have already been triggered and are persist because *reasons* (probably
                    // so the save is consistent).
                    continue;
                }

                bool match = BlackboardHelpers.Match(world, tracker, ruleComponent.Requirements);
                bool triggered = false;

                // If we have a match, trigger the rule and clean up the rule triggers.
                if (match && e.HasInteractive())
                {
                    e.SendMessage(new InteractMessage(e));
                    triggered = true;
                }

                // We do not have an interactive or a match, but if this is actually an *either* component, manually add one.
                if (e.TryGetPickEntityToAddOnStart() is PickEntityToAddOnStartComponent pickEntityOnStart)
                {
                    Guid target = match ? pickEntityOnStart.OnMatchPrefab : pickEntityOnStart.OnNotMatchPrefab;
                    
                    e.SetInteractive(new InteractiveComponent<AddEntityOnInteraction>(new(target)));
                    e.SendMessage(new InteractMessage(e));
                    triggered = true;
                }
                
                if (triggered)
                {
                    CleanupRuleMatchEntity(world, e, ruleComponent);
                }
            }
        }
        
        private void CleanupRuleMatchEntity(World world, Entity e, InteractOnRuleMatchComponent rule)
        {
            switch (rule.AfterInteraction)
            {
                case AfterInteractRule.InteractOnReload:
                    e.SetInteractOnRuleMatch(rule.Disable());
                    break;

                case AfterInteractRule.RemoveEntity:
                    e.Destroy();
                    break;

                case AfterInteractRule.InteractOnlyOnce:
                    e.RecordAndMaybeDestroy(world, destroy: false);

                    e.RemoveInteractOnRuleMatch();
                    break;
            }
        }
    }
}
