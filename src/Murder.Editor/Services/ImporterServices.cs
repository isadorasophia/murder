using Murder.Assets;
using Murder.Assets.Editor;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Utilities;

namespace Murder.Editor.Services
{
    public static class ImporterServices
    {
        private readonly static object _trackerLock = new();
        private static volatile SpritePathTrackerAsset? _tracker = null;

        public static SpritePathTrackerAsset GetOrCreateSpritePathTracker()
        {
            lock (_trackerLock)
            {
                if (_tracker is null)
                {
                    foreach ((Guid g, GameAsset a) in Game.Data.FilterAllAssets(typeof(SpritePathTrackerAsset)))
                    {
                        _tracker = (SpritePathTrackerAsset)a;
                        break;
                    }
                }

                if (_tracker is null)
                {
                    _tracker = new();

                    _tracker.MakeGuid();

                    _tracker.Name = "_spritePathTracker";
                    _tracker.FilePath = $"{_tracker.Name}.json";

                    if (_tracker.GetEditorAssetPath(useBinPath: true) is string existingPath && File.Exists(existingPath))
                    {
                        // delete previous one
                        File.Delete(existingPath);
                    }
                }

                return _tracker;
            }
        }

        public static IReadOnlySet<string>? FetchSpritePathsForGuid(Guid guid)
        {
            if (Game.Data.LoadContentProgress is not null && !Game.Data.LoadContentProgress.IsCompleted)
            {
                return null;
            }

            SpritePathTrackerAsset tracker = GetOrCreateSpritePathTracker();
            return tracker.FetchAllPathsWith(guid);
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