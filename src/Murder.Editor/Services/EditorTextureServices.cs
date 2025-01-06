using Microsoft.Xna.Framework.Graphics;
using Murder.Editor.Utilities.Serialization;
using System.IO.Compression;

namespace Murder.Editor.Services;

internal static class EditorTextureServices
{
    /// <summary>
    /// Save a <paramref name="texture"/> as a QOI image.
    /// </summary>
    public static void SaveAsQoiGz(this Texture2D texture, string path)
    {
        byte[] buffer = new byte[texture.Width * texture.Height * (int)Channels.Rgba];
        texture.GetData(buffer);

        QoiImage image = new(buffer, texture.Width, texture.Height, Channels.Rgba);
        byte[] qoiData = QoiEncoder.Encode(image);

        {
            using FileStream stream = File.Open(path, FileMode.OpenOrCreate);
            using GZipStream gzipStream = new(stream, CompressionMode.Compress);

            gzipStream.Write(qoiData);

            gzipStream.Close();
            stream.Close();
        }
    }

    /// <summary>
    /// Save a <paramref name="stream"/> as a QOI image.
    /// </summary>
    public static void ConvertPngStreamToQuoiGz(MemoryStream stream, string path)
    {
        using Texture2D texture = Texture2D.FromStream(Architect.GraphicsDevice, stream);

        SaveAsQoiGz(texture, path);
    }

    public static void SaveAsPng(MemoryStream stream, string path)
    {
        using Texture2D texture = Texture2D.FromStream(Architect.GraphicsDevice, stream);

        {
            using FileStream fileStream = File.Open(path, FileMode.OpenOrCreate);
            texture.SaveAsPng(fileStream, texture.Width, texture.Height);
            fileStream.Close();
        }
    }
}
