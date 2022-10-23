using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Murder.Diagnostics;
using Murder.Serialization;
using System.Collections.Immutable;

namespace Murder.Data
{
    public partial class GameDataManager
    {
        /// <summary>
        /// TODO: Limit cache size.
        /// </summary>
        private readonly Dictionary<string, SoundEffect> _cachedSounds = new();
        
        private readonly Dictionary<string, Song> _cachedSongs = new();

        /// <summary>
        /// Sound database.
        /// Maps:
        ///   [Sound name -> Sound path]
        /// </summary>
        private ImmutableDictionary<string, string> _soundDatabase = ImmutableDictionary<string, string>.Empty;

        public ImmutableArray<string> Sounds { get; private set; } = ImmutableArray<string>.Empty;

        /// <summary>
        /// This will load all the sounds to the game.
        /// </summary>
        public ValueTask LoadSounds()
        {
            _cachedSounds.Clear();
            _soundDatabase = _soundDatabase.Clear();

            const string soundDirectoryName = "sounds";

            GameLogger.Verify(_contentDirectoryPath is not null, "Why hasn't LoadContent() been called?");

            var builder = ImmutableDictionary.CreateBuilder<string, string>();

            string soundDirectory = Path.Join(_contentDirectoryPath, soundDirectoryName);

            List<FileInfo> soundFiles = FileHelper.GetAllFilesInFolder(soundDirectory, "*.wav", recursive: true).ToList();
            soundFiles.AddRange(FileHelper.GetAllFilesInFolder(soundDirectory, "*.ogg", recursive: true).ToList());
                
            foreach (FileInfo soundFile in soundFiles)
            {
                string name = Path.GetFileNameWithoutExtension(soundFile.Name);

#if DEBUG
                if (_soundDatabase.ContainsKey(name))
                {
                    GameLogger.Warning($"Duplicate name for sound! {soundFile.Name} will be overridden.");
                }
#endif

                builder.Add(name, soundFile.FullName);
                _soundDatabase.Add(name, soundFile.FullName);
            }

            GameLogger.Log($"Loaded a total of {builder.Count} sounds!");

            _soundDatabase = builder.ToImmutable();
            Sounds = _soundDatabase.Keys.ToImmutableArray();

            return default;
        }

        public ValueTask<SoundEffect> FetchSound(string name)
        {
            if (!_cachedSounds.TryGetValue(name, out SoundEffect? sound))
            {
                try
                {
                    using FileStream stream = File.Open(_soundDatabase[name], FileMode.Open, FileAccess.Read);

                    sound = SoundEffect.FromStream(stream);
                    _cachedSounds.Add(name, sound);

                    return new(sound);
                }
                catch
                {
                    GameLogger.Fail($"Cannot find sound file with id '{name}' at path {_soundDatabase[name]}");
                }
            }
            if (sound != null)
            {
                return new(sound);
            }
            else
                throw new Exception($"Cannot find sound file with id '{name}' at path {_soundDatabase[name]}");
        }

        public ValueTask<Song> FetchSong(string name)
        {
            if (!_cachedSongs.TryGetValue(name, out Song? song))
            {
                var path = _soundDatabase[name];
                song = Song.FromUri(name, new Uri(path, UriKind.Absolute));
                _cachedSongs.Add(name, song);
            }

            return new(song);
        }

        public SoundEffect Sound(string name)
        {
            if (_cachedSounds.TryGetValue(name, out SoundEffect? sound))
            {
                return sound;
            }

            GameLogger.Fail($"Should have fetched {name} first!");
            throw new ArgumentException("Sound has not been loaded yet.", nameof(name));
        }
    }
}