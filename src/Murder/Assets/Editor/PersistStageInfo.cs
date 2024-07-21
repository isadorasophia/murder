using Murder.Core.Geometry;

namespace Murder.Editor.Assets;

[Flags]
public enum StageSetting
{
    None = 0,
    ShowSound
}

public readonly struct PersistStageInfo
{
    public readonly Point Position;

    /// <summary>
    /// Size of the camera when this was saved.
    /// </summary>
    public readonly Point Size;

    public readonly int Zoom;

    public readonly StageSetting Settings;

    public PersistStageInfo(Point position, Point size, int zoom, StageSetting settings)
    {
        Position = position;
        Size = size;
        Zoom = zoom;
        Settings = settings;
    }
}