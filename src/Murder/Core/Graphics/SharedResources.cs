﻿using Microsoft.Xna.Framework.Graphics;

namespace Murder.Core.Graphics
{
    /// <summary>
    /// Shared resources used per game instance.
    /// TODO: Move to RenderContext?
    /// </summary>
    public static class SharedResources
    {
        /// Textures 2D do not need to be created for each spritebatch, only for different graphics devices.
        private static Texture2D _pixel = null!;

        public static Texture2D GetOrCreatePixel()
        {
            if (_pixel == null)
            {
                _pixel = new(Game.GraphicsDevice, 1, 1);
                _pixel.SetData(new Microsoft.Xna.Framework.Color[] { Microsoft.Xna.Framework.Color.White });
            }

            return _pixel;
        }
    }
}