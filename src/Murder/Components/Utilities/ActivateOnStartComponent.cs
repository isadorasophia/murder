using Bang.Components;
using Murder.Core;

namespace Murder.Components.Utilities;

public readonly struct ActivateOnStartComponent : IComponent
{
    public readonly AfterInteractRule After = AfterInteractRule.Always;
    public readonly bool DeactivateInstead = false;

    public readonly ICondition? OnlyWhen = null;

    public ActivateOnStartComponent()
    {
        
    }
    public ActivateOnStartComponent(AfterInteractRule after, bool deactivateInstead)
    {
        After = after;
        DeactivateInstead = deactivateInstead;
    }
}
