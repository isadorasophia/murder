// Gist from:
// https://gist.github.com/NoelFB/778d190e5d17f1b86ebf39325346fcc5

// File Format:
// https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md

// Note: I didn't test with with Indexed or Grayscale colors
// Only implemented the stuff I needed / wanted, other stuff is ignored

namespace Editor.Data.Aseprite
{
    public partial class Aseprite
    {
        public struct Tile
        {
            public int Id;
            public bool FlipX;
            public bool FlipY;
            public bool Rotate;
        }

    }
}