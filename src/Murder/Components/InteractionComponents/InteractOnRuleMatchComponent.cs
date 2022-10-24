using Bang.Components;
using Murder.Core.Dialogs;
using System.Collections.Immutable;

namespace Murder.Components
{
    internal readonly struct InteractOnRuleMatchComponent : IComponent
    {
        /// <summary>
        /// List of requirements which will trigger the interactive component within the same entity.
        /// </summary>
        public readonly ImmutableArray<CriterionNode> Requirements = ImmutableArray<CriterionNode>.Empty;

        public InteractOnRuleMatchComponent() { }

        public InteractOnRuleMatchComponent(params CriterionNode[] criteria) => Requirements = criteria.ToImmutableArray();
    }
}
