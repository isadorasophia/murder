using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Core.Dialogs;
using Murder.Core.MurderActions;
using Murder.Save;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components
{
    [RuntimeOnly]
    public readonly struct QuestTrackerRuntimeComponent : IComponent
    {
        public readonly ImmutableArray<QuestStageRuntime> QuestStages = ImmutableArray<QuestStageRuntime>.Empty;

        public QuestTrackerRuntimeComponent()
        {
        }

        public QuestTrackerRuntimeComponent(ImmutableArray<QuestStageRuntime> questStages)
        {
            QuestStages = questStages;
        }
    }

    public readonly struct QuestStageRuntime
    {
        public readonly ImmutableArray<CriterionNode> Requirements = ImmutableArray<CriterionNode>.Empty;
        public readonly ImmutableArray<MurderTargetedRuntimeAction> Actions = ImmutableArray<MurderTargetedRuntimeAction>.Empty;

        public QuestStageRuntime() { }

        public QuestStageRuntime(ImmutableArray<CriterionNode> requirements, ImmutableArray<MurderTargetedRuntimeAction> actions)
        {
            Requirements = requirements;
            Actions = actions;
        }

        internal void CheckAndRun(World world, Entity entity, BlackboardTracker tracker)
        {
            foreach (var rule in Requirements)
            {
                if (!tracker.Matches(rule.Criterion, null, world, null, out _))
                {
                    // If any rule doesn't match, stop.
                    return;
                }
            }

            foreach (var action in Actions)
            {
                if (world.TryGetEntity(action.EntityId) is Entity target)
                {
                    foreach (var interaction in action.Interaction)
                    {
                        interaction.Interact(world, entity, target);
                    }
                }
                else
                {
                    // Target lost
                }
            }
        }
    }
}
