using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Serialization;
using System.Collections.Immutable;

namespace Murder.Utilities
{
    public static class MurderAssetHelpers
    {
        public static T[] ToAssetArray<T>(this ImmutableArray<Guid> guids) where T : GameAsset
        {
            T[] assets = new T[guids.Length];
            for (int i = 0; i < guids.Length; ++i)
            {
                if (Game.Data.TryGetAsset<T>(guids[i]) is T asset)
                {
                    assets[i] = asset;
                }
                else
                {
                    GameLogger.Error($"Unable to fetch valid tileset of {guids[i]}.");
                }
            }

            return assets;
        }

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
                return Path.Join(Game.Data.SaveBasePath, asset.FilePath);
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

                if (!asset.Portraits.TryGetValue(portrait, out PortraitInfo result))
                {
                    return null;
                }

                return result.Portrait;
            }

            return null;
        }

        public static (SpriteAsset Asset, string Animation)? GetSpriteAssetForPortrait(Portrait portrait)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(portrait.Sprite) is SpriteAsset asset)
            {
                return (asset, portrait.AnimationId);
            }

            return null;
        }
    }
}