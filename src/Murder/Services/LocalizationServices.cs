using Murder.Assets.Localization;

namespace Murder.Services
{
    public static class LocalizationServices
    {
        public static LocalizationAsset GetDefaultLocalization()
        {
            if (Game.Data.TryGetAsset<LocalizationAsset>(Game.Profile.DefaultLocalization) is not LocalizationAsset asset)
            {
                // Create a default one.
                asset = new();
                asset.Name = "Global";

                Game.Data.AddAsset(asset);
            }

            return asset;
        }
    }
}
