using Microsoft.Xna.Framework;

// Gist from:
// https://gist.github.com/NoelFB/778d190e5d17f1b86ebf39325346fcc5

// File Format:
// https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md

// Note: I didn't test with with Indexed or Grayscale colors
// Only implemented the stuff I needed / wanted, other stuff is ignored

namespace Murder.Editor.Data.Graphics
{
    public partial class Aseprite
    {
        public class Tileset
        {
            public string Name = string.Empty;
            public int dataLength;
            public Color[] Pixels = null!;
            internal uint Id;
            internal ushort TileWidth;
            internal ushort TileHeight;

            public int TileCount { get; internal set; }
        }

    }
}