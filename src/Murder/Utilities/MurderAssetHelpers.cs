using Murder.Assets;
using Murder.Diagnostics;
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
    }
}
