
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Murder.Utilities;

namespace Murder.Services
{
    public static class TextureServices
    {
        //
        // Summary:
        //     Creates a Microsoft.Xna.Framework.Graphics.Texture2D from a file, supported formats
        //     bmp, gif, jpg, png, tif and dds (only for simple textures). May work with other
        //     formats, but will not work with tga files. This internally calls Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(Microsoft.Xna.Framework.Graphics.GraphicsDevice,System.IO.Stream).
        //
        // Parameters:
        //   graphicsDevice:
        //     The graphics device to use to create the texture.
        //
        //   path:
        //     The path to the image file.
        //
        // Returns:
        //     The Microsoft.Xna.Framework.Graphics.Texture2D created from the given file.
        //
        // Remarks:
        //     Note that different image decoders may generate slight differences between platforms,
        //     but perceptually the images should be identical.
        public static Texture2D FromFile(GraphicsDevice graphicsDevice, string path, bool premultiplyAlpha)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            using FileStream file = File.OpenRead(path);
            var texture = Texture2D.FromStream(graphicsDevice, file);

            if (premultiplyAlpha)
            {
                var data = new Color[texture.Width * texture.Height];
                texture.GetData(data);

                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = data[i].MultiplyAlpha();
                }
            }
            return texture;
        }
    }
}
