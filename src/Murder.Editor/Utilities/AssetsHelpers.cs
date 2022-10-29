using Murder.Assets;
using Murder.Data;
using Murder.Serialization;

namespace Murder.Editor.Utilities
{
    internal static class AssetsHelpers
    {
        /// <summary>
        /// Get the path to load or save <paramref name="asset"/>.
        /// </summary>
        public static string? GetAssetPath(this GameAsset asset, bool useBinPath = false)
        {
            if (string.IsNullOrEmpty(asset.FilePath) && Path.IsPathRooted(asset.FilePath))
            {
                // Not valid for bin paths.
                return useBinPath ? null : asset.FilePath;
            }

            if (asset.IsStoredInSaveData)
            {
                // Not valid for bin paths.
                return useBinPath ? null : Path.Join(GameDataManager.SaveBasePath, asset.SaveLocation, asset.FilePath);
            }

            return FileHelper.GetPath(
                useBinPath ? Architect.EditorSettings.BinResourcesPath : Architect.EditorSettings.SourceResourcesPath,
                asset.StoreInDatabase ? Game.Profile.AssetResourcesPath : string.Empty,
                asset.SaveLocation,
                asset.FilePath);
        }

        public static string? GetAssetDirectoryPath(this GameAsset asset)
        {
            if (string.IsNullOrEmpty(asset.FilePath) && Path.IsPathRooted(asset.FilePath))
            {
                return Path.GetDirectoryName(asset.FilePath);
            }

            if (asset.IsStoredInSaveData)
            {
                return Path.GetDirectoryName(
                    Path.Join(GameDataManager.SaveBasePath, asset.SaveLocation, asset.FilePath));
            }

            return Path.GetDirectoryName(FileHelper.GetPath(
                Architect.EditorSettings.SourceResourcesPath, 
                Game.Profile.AssetResourcesPath,
                asset.SaveLocation,
                asset.FilePath));
        }
    }
}
