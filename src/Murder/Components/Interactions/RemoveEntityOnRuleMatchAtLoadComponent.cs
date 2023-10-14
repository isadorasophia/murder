using Bang.Components;
using Murder.Core.Dialogs;
using System.Collections.Immutable;

namespace Murder.Components
{
    /// <summary>
    /// This will remove the entity that contains this component as soon as the entity is serialized
    /// into an actual world instance.
    /// </summary>
    public readonly struct RemoveEntityOnRuleMatchAtLoadComponent : IComponent
    {
        /// <summary>
        /// List of requirements which will trigger the interactive component within the same entity.
        /// </summary>
        public readonly ImmutableArray<CriterionNode> Requirements = ImmutableArray<CriterionNode>.Empty;

        public RemoveEntityOnRuleMatchAtLoadComponent() { }
    }
}