using Bang.Components;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Utilities;
using Murder.Utilities.Attributes;

namespace Murder.Components.Effects
{
    [CustomName("\uf70c On enter/exit interaction")]
    public readonly struct OnEnterOnExitComponent : IComponent
    {
        public readonly TargetEntity Target = TargetEntity.Interactor;

        [Default("Add interaction on enter")]
        public readonly IInteractiveComponent? OnEnter = null;

        [Default("Add interaction on exit")]
        public readonly IInteractiveComponent? OnExit = null;

        public OnEnterOnExitComponent() { }

        public OnEnterOnExitComponent(IInteractiveComponent onEnter, IInteractiveComponent onExit) =>
            (OnEnter, OnExit) = (onEnter, onExit);
    }
}