using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Utilities.Attributes;
using Newtonsoft.Json;
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
        InteractOnlyOnce,

        /// <summary>
        /// Instead of removing this component once triggered, this will only disable it.
        /// </summary>
        InteractOnReload,

        /// <summary>
        /// Instead of removing this component once triggered, this will remove the entity.
        /// </summary>
        RemoveEntity,

        /// <summary>
        /// Always interact whenever the rule gets triggered (added or modified).
        /// </summary>
        Always
    }

    [Story]
    public readonly struct InteractOnRuleMatchComponent : IComponent
    {
        [Tooltip("When should this be triggered.")]
        public readonly InteractOn InteractOn = InteractOn.AddedOrModified;

        [Tooltip("Expected behavior once the rule is met.")]
        public readonly AfterInteractRule AfterInteraction = AfterInteractRule.InteractOnlyOnce;

        /// <summary>
        /// This will only be triggered once the component has been interacted with.
        /// Used if <see cref="AfterInteractRule.InteractOnReload"/> is set.
        /// </summary>
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public readonly bool Triggered = false;

        /// <summary>
        /// List of requirements which will trigger the interactive component within the same entity.
        /// </summary>
        public readonly ImmutableArray<CriterionNode> Requirements = ImmutableArray<CriterionNode>.Empty;

        public InteractOnRuleMatchComponent() { }

        public InteractOnRuleMatchComponent(InteractOn interactOn, AfterInteractRule after, ImmutableArray<CriterionNode> requirements) =>
            (InteractOn, AfterInteraction, Requirements) = (interactOn, after, requirements);

        public InteractOnRuleMatchComponent(AfterInteractRule after, bool triggered, ImmutableArray<CriterionNode> requirements) =>
            (AfterInteraction, Triggered, Requirements) = (after, triggered, requirements);

        public InteractOnRuleMatchComponent(params CriterionNode[] criteria) => Requirements = criteria.ToImmutableArray();

        public InteractOnRuleMatchComponent Disable() => new(AfterInteraction, triggered: true, Requirements);
    }
}