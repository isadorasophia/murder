using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Save;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Systems.Utilities
{
    [Watch(typeof(RuleWatcherComponent))]
    public class QuestTrackerSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            UpdateQuests(world, entities);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            UpdateQuests(world, entities);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {

        }

        private static void UpdateQuests(World world, ImmutableArray<Entity> _)
        {
            var quests = world.GetEntitiesWith(typeof(QuestTrackerRuntimeComponent));
            BlackboardTracker tracker = MurderSaveServices.CreateOrGetSave().BlackboardTracker;

            foreach (var e in quests)
            {
                var quest = e.GetQuestTrackerRuntime();
                foreach (var stage in quest.QuestStages)
                {
                    stage.CheckAndRun(world, e, tracker);
                }
            }
        }

    }
}