using Bang.Components;
using Bang.Entities;
using Murder.Core.Geometry;
using System.Text.Json.Serialization;

namespace Murder.Components;

public enum CameraStyle
{
    DeadZone,
    Center,
    Perfect,
    KeepPosition
}

/// <summary>
/// Component used by the camera for tracking its target position.
/// </summary>
[Unique]
public readonly struct CameraFollowComponent : IComponent
{
    public readonly bool Enabled = true;

    [JsonIgnore]
    public readonly Entity? SecondaryTarget;

    [JsonIgnore]
    public readonly Point? TargetPosition;

    /// <summary>
    /// Force to centralize the camera without a dead zone.
    /// </summary>
    public readonly CameraStyle Style = CameraStyle.DeadZone;

    public CameraFollowComponent() { }

    public CameraFollowComponent(bool enabled)
    {
        Enabled = enabled;
    }
    public CameraFollowComponent(bool enabled, Entity secondaryTarget)
    {
        Enabled = enabled;
        SecondaryTarget = secondaryTarget;
    }

    public CameraFollowComponent(bool enabled, CameraStyle style)
    {
        Enabled = enabled;
        Style = style;
    }

    public CameraFollowComponent(Point targetPosition)
    {
        Enabled = true;
        Style = CameraStyle.Center;

        TargetPosition = targetPosition;
    }
}