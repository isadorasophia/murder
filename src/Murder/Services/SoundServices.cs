using Murder.Assets;
using Murder.Data;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Murder.Diagnostics;
using Murder.Utilities;

namespace Murder.Services
{
    public static class SoundServices
    {
        public static async ValueTask PlaySound(Guid guid)
        {
            if (Game.Data.TryGetAsset<SoundAsset>(guid) is SoundAsset asset)
            {
                var sound = asset.Sound();
                if (!string.IsNullOrWhiteSpace(sound))
                {
                    await PlaySound(sound);
                }
            }
            else
            {
                GameLogger.Fail($"Cannot find sound with guid {guid}");
            }
        }

        public static async ValueTask PlaySound(string name)
        {
            SoundEffect sound = await Game.Data.FetchSound(name);
            sound.Play(Game.Preferences.SoundVolume, 0, 0);
        }

        public static async ValueTask PlayMusic(string name)
        {
            if (Game.Preferences.SoundVolume == 0  || string.IsNullOrWhiteSpace(name))
            {
                MediaPlayer.Stop();
                return;
            }

            Song song = await Game.Data.FetchSong(name);

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

            MediaPlayer.Volume = Game.Preferences.SoundVolume;
        }

    }
}
