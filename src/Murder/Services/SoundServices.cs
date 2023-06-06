using Murder.Assets;
using Murder.Core.Sounds;
using Murder.Diagnostics;

namespace Murder.Services
{
    public static class SoundServices
    {
        public static async ValueTask Play(SoundEventId id, SoundProperties properties = SoundProperties.None)
        {
            if (!id.IsGuidEmpty)
            {
                await Game.Sound.PlayEvent(id, properties);
            }
        }

        public static async ValueTask PlayMusic(SoundEventId id)
        {
            await Game.Sound.PlayEvent(id, SoundProperties.Persist);
        }

        public static void StopAll()
        {
            Game.Sound.Stop(fadeOut: true);
        }
    }
}
