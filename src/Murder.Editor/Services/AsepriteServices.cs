using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Utilities;
using Murder.Utilities;

namespace Murder.Editor.Services
{
    public static class AsepriteServices
    {
        public static void BakeAllAsepriteFileGuid()
        {
            List<(AsepriteFileInfo FileInfo, Guid Guid)> assetsToBake = new();

            foreach (GameAsset asset in Game.Data.FilterAllAssets(typeof(SpriteAsset)).Values)
            {
                SpriteAsset sprite = (SpriteAsset)asset;
                if (sprite.AsepriteFileInfo != null)
                {
                    assetsToBake.Add((sprite.AsepriteFileInfo.Value, sprite.Guid));
                }
            }

            if (assetsToBake.Count == 0)
            {
                return;
            }

            string scriptPath = Path.Join(Architect.EditorSettings.BinResourcesPath, Architect.EditorSettings.LuaScriptsPath, "BakeGuid");
            if (!File.Exists(scriptPath))
            {
                GameLogger.Error($"Unable to find script at path: {scriptPath}. Please specify a valid script on Editor Settings for Lua Scripts.");
                return;
            }

            foreach ((AsepriteFileInfo fileInfo, Guid guid) in assetsToBake)
            {
                BakeAsepriteFileGuid(scriptPath, fileInfo, guid);
            }
        }

        public static void BakeAsepriteFileGuid(string scriptPath, AsepriteFileInfo info, Guid guid)
        {
            string command =
                $"{Architect.EditorSettings.AsepritePath} -b -script-param filename={info.Source} -script-param output={info.Source} -script-param layer={info.Layer} -script-param slice={info.SliceIndex} -script-param guid={guid} -script {scriptPath}.lua";

            string directoryPath = Path.GetDirectoryName(info.Source)!;
            ShellServices.ExecuteCommand(command, directoryPath);
        }
    }
}