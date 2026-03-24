using Bang.Components;
using Murder.Attributes;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Components;

public enum MoveToPerfectSettings
{
    None = 0,

    /// <summary>
    /// This allows the entity to push ANY actor in the way.
    /// Usually used for objects, and should be very careful since this may affect
    /// any actor.
    /// </summary>
    AvoidActors = 1
}

/// <summary>
/// This is a move to component that is not tied to any agent and
/// that matches perfectly the target.
/// </summary>
[RuntimeOnly]
[PersistOnSave]
public readonly struct MoveToPerfectComponent : IComponent
{
    public readonly Vector2 Target;
    public readonly Vector2? StartPosition { get; init; } = null;

    public readonly float StartTime;
    public readonly float Duration;

    public readonly EaseKind EaseKind;

    public readonly MoveToPerfectSettings Settings { get; init; } = MoveToPerfectSettings.None;

    private MoveToPerfectComponent(in Vector2 target, in Vector2 startPosition,
        float startTime, float duration, EaseKind ease) =>
        (Target, StartPosition, StartTime, Duration, EaseKind) =
        (target, startPosition, startTime, duration, ease);

    public MoveToPerfectComponent(in Vector2 target, float duration, EaseKind ease) : this(target, duration, ease, MoveToPerfectSettings.None) { }

    public MoveToPerfectComponent(in Vector2 target, float duration, EaseKind ease, MoveToPerfectSettings settings)
    {
        Target = target;

        StartTime = Game.Now;
        Duration = duration;

        EaseKind = ease;
        Settings = settings;
    }

    public MoveToPerfectComponent WithStartPosition(in Vector2 startPosition) =>
        new(Target, startPosition, StartTime, Duration, EaseKind);
}