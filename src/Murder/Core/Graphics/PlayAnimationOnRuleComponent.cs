using Bang.Components;
using Murder.Core.Dialogs;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Core.Graphics;

public readonly struct PlayAnimationOnRuleComponent : IComponent
{
    public readonly ImmutableArray<AnimationAndRule> Rules = ImmutableArray<AnimationAndRule>.Empty;

    public PlayAnimationOnRuleComponent()
    {
    }
}

[RuntimeOnly]
public readonly struct AnimationRuleMatchedComponent : IComponent
{
    public readonly int RuleIndex;

    public AnimationRuleMatchedComponent(int ruleIndex)
    {
        RuleIndex = ruleIndex;
    }
}

public readonly struct AnimationAndRule
{
    public readonly ImmutableArray<string> Animation = ImmutableArray<string>.Empty;
    public readonly ImmutableArray<CriterionNode> Requirements = ImmutableArray<CriterionNode>.Empty;

    public AnimationAndRule()
    {
    }
}
