using Microsoft.Xna.Framework;
using static System.Net.Mime.MediaTypeNames;

// Gist from:
// https://gist.github.com/NoelFB/778d190e5d17f1b86ebf39325346fcc5

// File Format:
// https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md

// Note: I didn't test with with Indexed or Grayscale colors
// Only implemented the stuff I needed / wanted, other stuff is ignored

namespace Murder.Editor.Data.Graphics;


public struct UserData
{
    public string Text = String.Empty;
    public Color Color = Color.Transparent;
    

    public UserData()
    {
    }
}

public partial class Aseprite
{
    public interface IUserData
    {
        public UserData UserData { get; set; }
        public Guid Guid { get; set; }

        const string GUID_KEYWORD = "guid:";
        const int GUID_KEYWORD_LENGTH = 5;
        internal Guid GetGuid()
        {
            if (UserData.Text != null && UserData.Text.StartsWith(GUID_KEYWORD, StringComparison.OrdinalIgnoreCase))
            {
                return new Guid(UserData.Text.Substring(GUID_KEYWORD_LENGTH));
            }
            return Guid.Empty;
        }
    }

}