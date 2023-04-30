using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Data;
using Murder.Serialization;

namespace Murder.Utilities
{
    public static class AssetHelpers
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

        public static Portrait? GetPortraitForLine(Line line)
        {
            if (line.Speaker is Guid speakerAsset)
            {
                SpeakerAsset? asset = Game.Data.TryGetAsset<SpeakerAsset>(speakerAsset);
                if (asset is null)
                {
                    return null;
                }

                string? portrait = line.Portrait ?? Game.Data.TryGetAsset<SpeakerAsset>(speakerAsset)?.DefaultPortrait;
                if (portrait is null)
                {
                    return null;
                }

                if (!asset.Portraits.TryGetValue(portrait, out Portrait result))
                {
                    return null;
                }

                return result;
            }

            return null;
        }

        public static (SpriteAsset Asset, string Animation)? GetSpriteAssetForPortrait(Portrait portrait)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(portrait.Aseprite) is SpriteAsset asset)
            {
                return (asset, portrait.AnimationId);
            }

            return null;
        }
    }
}