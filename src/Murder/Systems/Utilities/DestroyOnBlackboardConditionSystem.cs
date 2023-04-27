using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Save;
using Murder.Services;
using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Systems.Utilities
{
    [Filter(typeof(DestroyOnBlackboardConditionComponent))]
    [Watch(typeof(DestroyOnBlackboardConditionComponent))]
    internal class DestroyOnBlackboardConditionSystem : IReactiveSystem, IStartupSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                CheckRules(world, e);
            }
        }

        private void CheckRules(World world, Entity entity)
        {
            if (entity.IsDestroyed)
                return;
            
            var rules = entity.GetDestroyOnBlackboardCondition();
            BlackboardTracker tracker = MurderSaveServices.CreateOrGetSave().BlackboardTracker;

            foreach (var rule in rules.Rules)
            {
                if (!tracker.Matches(rule.Criterion, entity.TryGetSpeaker()?.Speaker, world, entity.EntityId, out _))
                {
                    // If any rule doesn't match, stop.
                    return;
                }
            }

            entity.Destroy();
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                CheckRules(world, e);
            }
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            // No need to check on removed, right? It's already gone.
        }

        public void Start(Context context)
        {
            foreach (var e in context.Entities)
            {
                CheckRules(context.World, e);
            }
        }
    }
}
