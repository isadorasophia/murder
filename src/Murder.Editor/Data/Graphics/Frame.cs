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
        public class Frame
        {
            public Aseprite Sprite;
            public int Duration;
            public Color[] Pixels = null!;
            public Dictionary<int, Cel> Cels;

            public Frame(Aseprite sprite)
            {
                Sprite = sprite;
                Cels = new Dictionary<int, Cel>();
            }
        }

    }
}