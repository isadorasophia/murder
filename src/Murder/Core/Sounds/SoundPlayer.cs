using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Murder.Diagnostics;

namespace Murder.Core.Sounds
{
    public class SoundPlayer : ISoundPlayer
    {
        private float _volume = 1;
        
        public void Initialize(string resourcesPath) { }

        public void Update() { }

        public async ValueTask PlayEvent(SoundEventId id, SoundProperties properties)
        {
            if (string.IsNullOrWhiteSpace(id.Path))
            {
                GameLogger.Fail("Played an invalid sound id.");
                return;
            }

            if (properties.HasFlag(SoundProperties.Persist))
            {
                await PlayMusic(id);
                return;
            }

            SoundEffect? sound = await Game.Data.TryFetchSound(id.Path);
            if (sound != null)
            {
                sound.Play(_volume, 0, 0);
            }
            else
            {
                await PlayMusic(id);
            }
        }

        private async ValueTask PlayMusic(SoundEventId sound)
        {
            if (_volume == 0 || string.IsNullOrWhiteSpace(sound.Path))
            {
                MediaPlayer.Stop();
                return;
            }

            Song? song = await Game.Data.TryFetchSong(sound.Path);
            if (song is null)
            {
                return;
            }

            MediaPlayer.Play(song);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = _volume;
        }
        
        /// <summary>
        /// Change volume.
        /// </summary>
        public void SetVolume(SoundEventId? _, float volume)
        {
            _volume = volume;
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
