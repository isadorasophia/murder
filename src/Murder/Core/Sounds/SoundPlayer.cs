using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Murder.Core.Sounds
{
    public class SoundPlayer : ISoundPlayer
    {
        private float _volume = 1;
        
        public void Initialize(string resourcesPath) { }

        public void Update() { }

        public async ValueTask PlayEvent(string name, bool _)
        {
            SoundEffect? sound = await Game.Data.TryFetchSound(name);
            if (sound != null)
            {
                sound.Play(_volume, 0, 0);
            }
            else
            {
                await PlayStreaming(name);
            }
        }

        public async ValueTask PlayStreaming(string name)
        {
            if (_volume == 0 || string.IsNullOrWhiteSpace(name))
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

            MediaPlayer.Volume = _volume;
        }
        
        /// <summary>
        /// Change volume.
        /// </summary>
        public void SetVolume(string? busName, float volume)
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
