
using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using System.Collections.Immutable;

namespace Murder.Components
{
    public readonly struct DestroyOnBlackboardConditionComponent : IComponent
    {
        /// <summary>
        /// List of requirements for destroying this object.
        /// </summary>
        [ShowInEditor, Tooltip("List of rules for destroying this object.")]
        public readonly ImmutableArray<CriterionNode> Rules = ImmutableArray<CriterionNode>.Empty;

        public DestroyOnBlackboardConditionComponent()
        {
        }

        public DestroyOnBlackboardConditionComponent(CriterionNode node)
        {
            Rules = [node];
        }
    }
}