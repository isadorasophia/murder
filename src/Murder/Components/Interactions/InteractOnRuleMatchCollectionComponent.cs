using Bang.Components;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    [Story]
    public readonly struct InteractOnRuleMatchCollectionComponent : IComponent
    {
        /// <summary>
        /// List of interactions that will be triggered.
        /// </summary>
        public readonly ImmutableArray<InteractOnRuleMatchComponent> Requirements =
            ImmutableArray<InteractOnRuleMatchComponent>.Empty;

        public InteractOnRuleMatchCollectionComponent() { }

        public InteractOnRuleMatchCollectionComponent(ImmutableArray<InteractOnRuleMatchComponent> requirements) =>
            Requirements = requirements;
    }
}