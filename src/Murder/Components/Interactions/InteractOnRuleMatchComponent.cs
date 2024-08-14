using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    public enum InteractOn
    {
        AddedOrModified,

        Modified
    }

    public enum AfterInteractRule
    {
        InteractOnlyOnce = 0,

        /// <summary>
        /// Always interact whenever the rule gets triggered (added or modified).
        /// </summary>
        Always = 3,

        /// <summary>
        /// Remove InteractOnRuleMatchComponent after this is triggered.
        /// </summary>
        RemoveComponent = 4
    }

    [Story]
    public readonly struct InteractOnRuleMatchComponent : IComponent
    {
        [Tooltip("When should this be triggered.")]
        public readonly InteractOn InteractOn = InteractOn.AddedOrModified;

        [Tooltip("Expected behavior once the rule is met.")]
        public readonly AfterInteractRule AfterInteraction = AfterInteractRule.RemoveComponent;

        /// <summary>
        /// List of requirements which will trigger the interactive component within the same entity.
        /// </summary>
        public readonly ImmutableArray<CriterionNode> Requirements = ImmutableArray<CriterionNode>.Empty;

        public InteractOnRuleMatchComponent() { }

        public InteractOnRuleMatchComponent(InteractOn interactOn, AfterInteractRule after, ImmutableArray<CriterionNode> requirements) =>
            (InteractOn, AfterInteraction, Requirements) = (interactOn, after, requirements);

        public InteractOnRuleMatchComponent(AfterInteractRule after, ImmutableArray<CriterionNode> requirements) =>
            (AfterInteraction, Requirements) = (after, requirements);

        public InteractOnRuleMatchComponent(params CriterionNode[] criteria) => Requirements = criteria.ToImmutableArray();
    }
}