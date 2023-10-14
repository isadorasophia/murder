﻿using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Interactions;
using Bang.Systems;
using Murder.Assets;
using Murder.Components;
using Murder.Core;
using Murder.Diagnostics;
using Murder.Interactions;
using Murder.Messages;
using Murder.Save;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Filter(typeof(RuleWatcherComponent))]
    [Watch(typeof(RuleWatcherComponent))]
    public class InteractOnRuleMatchSystem : IStartupSystem, IReactiveSystem
    {
        public void Start(Context context)
        {
            GameLogger.Verify(context.World.TryGetUniqueEntity<RuleWatcherComponent>() is not Entity,
                "Why did we already add an existing rule watcher component!?");

            _ = context.World.AddEntity(new RuleWatcherComponent());
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            CheckAndTriggerRules(world, modified: false);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            CheckAndTriggerRules(world, modified: true);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities) { }

        private void CheckAndTriggerRules(World world, bool modified)
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

                bool matched = CheckEntity(world, tracker, ruleComponent, modified);
                bool triggered = false;

                if (matched)
                {
                    e.SendMessage(new InteractMessage(e));
                    triggered = true;
                }

                if (e.TryGetPickEntityToAddOnStart() is PickEntityToAddOnStartComponent pickEntityOnStart)
                {
                    Guid target = matched ? pickEntityOnStart.OnMatchPrefab : pickEntityOnStart.OnNotMatchPrefab;

                    e.SetInteractive(new InteractiveComponent<AddEntityOnInteraction>(new(target)));
                    e.SendMessage(new InteractMessage(e));

                    triggered = true;
                }

                if (triggered)
                {
                    CleanupRuleMatchEntity(world, e, ruleComponent);
                }
            }

            // Fetch all entities which might be affected by a rule change.
            interactives = world.GetEntitiesWith(typeof(InteractOnRuleMatchCollectionComponent));
            foreach (Entity e in interactives)
            {
                if (e.IsDestroyed)
                {
                    continue;
                }

                InteractOnRuleMatchCollectionComponent ruleCollection = e.GetInteractOnRuleMatchCollection();
                ImmutableArray<InteractOnRuleMatchComponent> requirements = ruleCollection.Requirements;

                bool matched = false;

                Stack<int> triggeredRequirements = new();
                for (int i = 0; i < requirements.Length; ++i)
                {
                    if (CheckEntity(world, tracker, requirements[i], modified))
                    {
                        matched = true;
                        triggeredRequirements.Push(i);
                    }
                }

                if (matched)
                {
                    e.SendMessage(new InteractMessage(e));
                }

                if (triggeredRequirements.Count == 0)
                {
                    continue;
                }

                while (triggeredRequirements.TryPeek(out int next))
                {
                    InteractOnRuleMatchComponent rule = requirements[next];
                    switch (rule.AfterInteraction)
                    {
                        case AfterInteractRule.InteractOnReload:
                            requirements = requirements.SetItem(next, rule.Disable());
                            break;

                        case AfterInteractRule.RemoveEntity:
                            e.Destroy();
                            break;

                        case AfterInteractRule.InteractOnlyOnce:
                            requirements = requirements.RemoveAt(next);

                            if (rule.InteractOn == InteractOn.AddedOrModified)
                            {
                                // Right now, this will remove the entity from the save.
                                // Maybe we want to do something else?
                                e.RecordAndMaybeDestroy(world, destroy: false);
                            }

                            break;
                    }

                    triggeredRequirements.Pop();
                }

                e.SetInteractOnRuleMatchCollection(requirements);
            }

            tracker.ResetPendingTriggers();
        }

        private bool CheckEntity(World world, BlackboardTracker tracker, InteractOnRuleMatchComponent ruleComponent, bool wasRuleModified)
        {
            if (ruleComponent.Triggered)
            {
                // Skip components that have already been triggered and are persist because *reasons* (probably
                // so the save is consistent).
                return false;
            }

            if (ruleComponent.InteractOn == InteractOn.Modified && !wasRuleModified)
            {
                // Do not trigger when rule watcher has been added (probably due to a map load).
                return false;
            }

            return BlackboardHelpers.Match(world, tracker, ruleComponent.Requirements);
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