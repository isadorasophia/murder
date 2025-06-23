using Bang.Components;

namespace Murder.Components.Utilities;

public readonly struct ActivateOnStartComponent() : IComponent
{
    public readonly AfterInteractRule After = AfterInteractRule.Always;
    public readonly bool DeactivateInstead = false;
}
