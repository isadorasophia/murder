using Bang.Components;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Core.Physics;
using Murder.Utilities;
using Murder.Utilities.Attributes;

namespace Murder.Components.Effects;

[Flags]
public enum OnEnterOnExitKind
{
    Player = 1,
    Actors = 0x10,
    Npc = Actors | 0x100
}

[CustomName($"\uf70c {nameof(OnEnterOnExitComponent)}")]
public readonly struct OnEnterOnExitComponent : IComponent
{
    public readonly TargetEntity Target = TargetEntity.Interactor;

    [Default("Add interaction on enter")]
    public readonly IInteractiveComponent? OnEnter = null;

    [Default("Add interaction on exit")]
    public readonly IInteractiveComponent? OnExit = null;

    public readonly OnEnterOnExitKind Kind { get; init; } = OnEnterOnExitKind.Player;

    [CollisionLayer]
    [Tooltip("Only applicable if entities other than player trigger this")]
    public readonly int? TriggerOnLayer = null;

    public OnEnterOnExitComponent() { }

    public OnEnterOnExitComponent(IInteractiveComponent onEnter, IInteractiveComponent onExit) =>
        (OnEnter, OnExit) = (onEnter, onExit);
}