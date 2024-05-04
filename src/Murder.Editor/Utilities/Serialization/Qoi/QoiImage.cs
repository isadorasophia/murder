namespace Murder.Editor.Utilities.Serialization;

public readonly struct QoiImage
{
    public readonly byte[] Data;
    public readonly int Width;
    public readonly int Height;
    public readonly Channels Channels;
    public readonly ColorSpace ColorSpace;

    public QoiImage(byte[] data, int width, int height, Channels channels, ColorSpace colorSpace = ColorSpace.SRgb)
    {
        Data = data;
        Width = width;
        Height = height;
        Channels = channels;
        ColorSpace = colorSpace;
    }
}
