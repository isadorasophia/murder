using Murder.Assets;
using Murder.Core.Sounds;
using Murder.Diagnostics;

namespace Murder.Services
{
    public static class SoundServices
    {
        public static async ValueTask Play(Guid guid, bool persist)
        {
            if (Game.Data.TryGetAsset<SoundAsset>(guid) is SoundAsset asset)
            {
                SoundEventId sound = asset.Sound();
                await PlaySound(sound, persist);
            }
            else
            {
                GameLogger.Fail($"Cannot find sound with guid {guid}");
            }
        }

        public static async ValueTask PlaySound(SoundEventId id, bool loop)
        {
            if (!id.IsGuidEmpty)
            {
                await Game.Sound.PlayEvent(id, isLoop: loop);
            }
        }

        public static async ValueTask PlayMusic(SoundEventId id)
        {
            await Game.Sound.PlayStreaming(id);
        }

        public static void StopAll()
        {
            Game.Sound.Stop(fadeOut: true);
        }
    }
}
