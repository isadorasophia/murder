using System.ComponentModel;

namespace Murder.Data
{
    // Description must be atlas file name
    public enum AtlasId
    {
        None,
        [Description("atlas")]
        Gameplay,
        [Description("editor")]
        Editor,
        [Description("static")]
        Static,
        //[Description("main_menu")]
        //MainMenu,
        //[Description("portraits")]
        //Portraits,
    }
}