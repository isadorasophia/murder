using Bang.Components;
using Murder.Core;
using Murder.Core.Dialogs;
using System.Collections.Immutable;

namespace Murder.Components;

public readonly struct OnlyApplyOnRuleComponent : IComponent
{
    /// <summary>
    /// List of requirements which will trigger the interactive component within the same entity.
    /// </summary>
    public readonly ImmutableArray<CriterionNode> Requirements = [];
    public readonly ImmutableArray<ICondition> Conditions = [];

    public OnlyApplyOnRuleComponent() { }
}
