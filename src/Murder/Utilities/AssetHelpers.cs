using Murder.Assets;
using Murder.Data;
using Murder.Serialization;

namespace Murder.Utilities
{
    internal static class AssetHelpers
    {
        /// <summary>
        /// Get the path to load or save <paramref name="asset"/>.
        /// </summary>
        public static string? GetGameAssetPath(this GameAsset asset)
        {
            if (!string.IsNullOrEmpty(asset.FilePath) && Path.IsPathRooted(asset.FilePath))
            {
                return asset.FilePath;
            }

            if (asset.IsStoredInSaveData)
            {
                return Path.Join(GameDataManager.SaveBasePath, asset.FilePath);
            }

            return FileHelper.GetPath(
                Game.Data.AssetsBinDirectoryPath,
                asset.StoreInDatabase ? Game.Profile.AssetResourcesPath : string.Empty,
                asset.SaveLocation,
                asset.FilePath);
        }
    }
}