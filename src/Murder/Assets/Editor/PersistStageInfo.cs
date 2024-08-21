using Murder.Core.Geometry;

namespace Murder.Editor.Assets;

[Flags]
public enum StageSetting
{
    None = 0,
    ShowSound = 1 << 0,
    ShowSprite = 1 << 1,
    ShowCollider = 1 << 2,
}

public readonly struct PersistStageInfo
{
    public readonly Point Position;

    /// <summary>
    /// Size of the camera when this was saved.
    /// </summary>
    public readonly Point Size;

    public readonly int Zoom;

    public readonly StageSetting Settings = StageSetting.ShowCollider;

    public PersistStageInfo(Point position, Point size, int zoom, StageSetting settings)
    {
        Position = position;
        Size = size;
        Zoom = zoom;
        Settings = settings;
    }
}