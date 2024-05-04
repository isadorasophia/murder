using System.Buffers.Binary;

namespace Murder.Editor.Utilities.Serialization;

/// <summary>
/// QOI Codec.
/// </summary>
public static class QoiCodec
{
    public const byte Index = 0x00;
    public const byte Diff = 0x40;
    public const byte Luma = 0x80;
    public const byte Run = 0xC0;
    public const byte Rgb = 0xFE;
    public const byte Rgba = 0xFF;
    public const byte Mask2 = 0xC0;

    /// <summary>
    /// 2GB is the max file size that this implementation can safely handle. We guard
    /// against anything larger than that, assuming the worst case with 5 bytes per 
    /// pixel, rounded down to a nice clean value. 400 million pixels ought to be 
    /// enough for anybody.
    /// </summary>
    public const int MaxPixels = 400_000_000;

    public const int HashTableSize = 64;

    public const byte HeaderSize = 14;
    public const string MagicString = "qoif";

    public static readonly int Magic = 'q' << 24 | 'o' << 16 | 'i' << 8 | 'f';
    public static readonly byte[] Padding = { 0, 0, 0, 0, 0, 0, 0, 1 };
    internal static readonly ReadOnlyMemory<byte> ReadOnlyPadding = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 };

    public static int CalculateHashTableIndex(int r, int g, int b, int a) =>
        ((r & 0xFF) * 3 + (g & 0xFF) * 5 + (b & 0xFF) * 7 + (a & 0xFF) * 11) % HashTableSize * 4;

    public static bool IsValidMagic(ReadOnlySpan<byte> magic) => BinaryPrimitives.ReadInt32BigEndian(magic) == Magic;
}

public enum ColorSpace : byte
{
    SRgb = 0,
    Linear = 1
}

public enum Channels : byte
{
    Rgb = 3,
    Rgba = 4
}
