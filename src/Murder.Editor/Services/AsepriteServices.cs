using Murder.Assets.Graphics;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Utilities;

namespace Murder.Editor.Services
{
    public static class AsepriteServices
    {
        public static void BakeAllAsepriteFileGuid()
        {

            foreach (var asset in Game.Data.FilterAllAssets(typeof(SpriteAsset)))
            {
                var sprite = (SpriteAsset)asset.Value;
                if (sprite.AsepriteFileInfo != null)
                {
                    BakeAsepriteFileGuid(sprite.AsepriteFileInfo.Value, asset.Value.Guid);
                }
            }
        }


        public static void BakeAsepriteFileGuid(AsepriteFileInfo info, Guid guid)
        {
            string script = Path.Join(Architect.EditorSettings.LuaScriptsPath, "BakeGuid");
            string command =
                $"{Architect.EditorSettings.AsepritePath} -b -script-param filename={info.Source} -script-param output={info.Source} -script-param layer={info.Layer} -script-param slice={info.SliceIndex} -script-param guid={guid} -script {script}.lua";

            var directory = Path.GetDirectoryName(info.Source)!;
            ShellServices.ExecuteCommand(command, directory);
        }
    }
}