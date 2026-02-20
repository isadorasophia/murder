using Bang.Components;
using Murder.Core.Dialogs;
using Murder.Utilities;

namespace Murder.Components;

public readonly struct TrackDistanceWithParameterComponent : IComponent
{
    public readonly float MinRange = 30;
    public readonly float MaxRange = 200;

    public readonly EaseKind EaseKind = EaseKind.Linear;
    public readonly Fact Parameter = new();

    public readonly ListenerKind Listener = ListenerKind.Camera;

    public TrackDistanceWithParameterComponent() { }
}
