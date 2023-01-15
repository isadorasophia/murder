using Murder.Assets;
using Murder.Diagnostics;

namespace Murder.Services
{
    public static class SoundServices
    {
        public static async ValueTask Play(Guid guid, bool persist)
        {
            if (Game.Data.TryGetAsset<SoundAsset>(guid) is SoundAsset asset)
            {
                var sound = asset.Sound();
                if (!string.IsNullOrWhiteSpace(sound))
                {
                    await PlaySound(sound, persist);
                }
            }
            else
            {
                GameLogger.Fail($"Cannot find sound with guid {guid}");
            }
        }

        public static async ValueTask PlaySound(string name, bool persist)
        {
            if (!string.IsNullOrEmpty(name))
                await Game.Sound.PlayEvent(name, isLoop: persist);
        }

        public static async ValueTask PlayMusic(string name)
        {
            await Game.Sound.PlayStreaming(name);
        }

        public static void StopAll()
        {
            Game.Sound.Stop(fadeOut: true);
        }
    }
}
