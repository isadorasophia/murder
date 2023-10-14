
using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Core.MurderActions;
using Murder.Save;
using System.Collections.Immutable;

namespace Murder.Components
{
    public readonly struct QuestTrackerComponent : IComponent
    {
        public readonly string Name = string.Empty;

        public readonly ImmutableArray<QuestStage> QuestStages = ImmutableArray<QuestStage>.Empty;

        public QuestTrackerComponent() { }
    }

    public readonly struct QuestStage
    {
        public readonly string Commentary = string.Empty;

        [ShowInEditor, Tooltip("Rule requirements that need to be matched for the actions/interactions to happen")]
        public readonly ImmutableArray<CriterionNode> Requirements = ImmutableArray<CriterionNode>.Empty;
        public readonly ImmutableArray<MurderTargetedAction> Actions = ImmutableArray<MurderTargetedAction>.Empty;

        public QuestStage() { }
    }
}