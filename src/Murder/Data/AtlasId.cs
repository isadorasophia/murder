using System.ComponentModel;

namespace Murder.Data
{
    public enum AtlasId
    {
        None,
        [Description("atlas")]
        Gameplay,
        [Description("editor")]
        Editor,
        //[Description("generic")]
        //Generic,
        //[Description("main_menu")]
        //MainMenu,
        //[Description("portraits")]
        //Portraits,
    }
}