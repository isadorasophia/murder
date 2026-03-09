using Murder.Core.Geometry;
using Murder.Core.Graphics;

namespace Murder.Core;

public enum ScreenUpdatedKind
{
    NotifyOnly = 0,
    Reset = 1,
    FullScreen = 2,
    NotifyAndApply = 3,
    ScalePreferenceModified = 4
}

public readonly struct WindowChangeNotification
{
    public readonly ScreenUpdatedKind Kind = ScreenUpdatedKind.NotifyOnly;

    public readonly Point? ApplySizeTo => ApplyToSettings?.Size;
    public readonly WindowChangeSettings? ApplyToSettings = null;

    public WindowChangeNotification() { }

    public WindowChangeNotification(ScreenUpdatedKind kind) =>
        Kind = kind;

    public WindowChangeNotification(Point applySizeTo)
    {
        Kind = ScreenUpdatedKind.NotifyAndApply;
        ApplyToSettings = new(applySizeTo);
    }

    public WindowChangeNotification(WindowChangeSettings settings)
    {
        Kind = ScreenUpdatedKind.NotifyAndApply;
        ApplyToSettings = settings;
    }
}
