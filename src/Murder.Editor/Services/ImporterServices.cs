using Murder.Assets;
using Murder.Assets.Editor;
using Murder.Assets.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Utilities;

namespace Murder.Editor.Services
{
    public static class ImporterServices
    {

        private static SpritePathsTrackerAsset? _tracker = null;

        public static SpritePathsTrackerAsset CreateNewSpritePathsTracker()
        {
            SpritePathsTrackerAsset instance = new();
            instance.Name = "_SpritePaths";
            instance.MakeGuid();

            Architect.EditorData.SaveAsset(instance);

            _tracker = instance;
            return _tracker;
        }

        public static SpritePathsTrackerAsset? TryGetSpritePathTrackerAsset()
        {
            if (Game.Data.LoadContentProgress is not null && !Game.Data.LoadContentProgress.IsCompleted)
            {
                return null;
            }

            if (_tracker is null)
            {
                foreach ((Guid g, GameAsset a) in Game.Data.FilterAllAssets(typeof(SpritePathsTrackerAsset)))
                {
                    _tracker = (SpritePathsTrackerAsset)a;
                }
            }

            if (_tracker is SpritePathsTrackerAsset tracker)
            {
                return tracker;
            }

            // Otherwise, this means we need to actually create one...
            return CreateNewSpritePathsTracker();
        }

        // GUID backing is deprecated, for now
        //public static void BakeAllAsepriteFileGuid()
        //{
        //    List<(AsepriteFileInfo FileInfo, Guid Guid)> assetsToBake = new();

        //    foreach (GameAsset asset in Game.Data.FilterAllAssets(typeof(SpriteAsset)).Values)
        //    {
        //        SpriteAsset sprite = (SpriteAsset)asset;
        //        if (sprite.AsepriteFileInfo != null && !sprite.AsepriteFileInfo.Value.Baked)
        //        {
        //            assetsToBake.Add((sprite.AsepriteFileInfo.Value, sprite.Guid));
        //        }
        //    }

        //    if (assetsToBake.Count == 0)
        //    {
        //        return;
        //    }

        //    string scriptPath = Path.Join(Architect.EditorSettings.BinResourcesPath, Architect.EditorSettings.LuaScriptsPath, "BakeGuid.lua");
        //    if (!File.Exists(scriptPath))
        //    {
        //        GameLogger.Error($"Unable to find script at path: {scriptPath}. Please specify a valid script on Editor Settings for Lua Scripts.");
        //        return;
        //    }

        //    foreach ((AsepriteFileInfo fileInfo, Guid guid) in assetsToBake)
        //    {
        //        BakeAsepriteFileGuid(scriptPath, fileInfo, guid);
        //    }
        //}

        //public static void BakeAsepriteFileGuid(string scriptPath, AsepriteFileInfo info, Guid guid)
        //{
        //    string command =
        //        $"{Architect.EditorSettings.AsepritePath} -b -script-param filename={info.Source} -script-param output={info.Source} -script-param layer={info.Layer} -script-param slice={info.SliceIndex} -script-param guid={guid} -script {FileHelper.GetPath(scriptPath)}";

        //    string directoryPath = Path.GetDirectoryName(info.Source)!;
        //    ShellServices.ExecuteCommand(command, directoryPath);
        //    GameLogger.Log($"Baked GUIDS in {info.Source}");
        //}
    }
}