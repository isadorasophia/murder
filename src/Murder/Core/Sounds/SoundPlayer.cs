using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Murder.Core.Sounds
{
    public class SoundPlayer : ISoundPlayer
    {
        public void Initialize(string resourcesPath) { }

        public void Update() { }

        public async ValueTask PlayEvent(string name, bool _)
        {
            SoundEffect? sound = await Game.Data.TryFetchSound(name);
            if (sound != null)
            {
                sound.Play(Game.Preferences.SoundVolume, 0, 0);
            }
            else
            {
                await PlayStreaming(name);
            }
        }

        public async ValueTask PlayStreaming(string name)
        {
            if (Game.Preferences.SoundVolume == 0 || string.IsNullOrWhiteSpace(name))
            {
                MediaPlayer.Stop();
                return;
            }

            Song? song = await Game.Data.TryFetchSong(name);
            if (song is null)
            {
                return;
            }

            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

            MediaPlayer.Volume = Game.Preferences.SoundVolume;
        }

        public void Stop(bool _)
        {
            MediaPlayer.Stop();
            foreach (var sound in Game.Data.CachedSounds)
            {
                sound.Dispose();
            }
        }
    }
}
