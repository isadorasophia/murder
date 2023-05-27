using Murder.Assets;
using Murder.Core.Sounds;
using Murder.Save;

namespace Murder
{
    /// <summary>
    /// This is the main loop of a murder game. This has the callbacks to relevant events in the game.
    /// </summary>
    public interface IMurderGame
    {
        /// <summary>
        /// Called once, when the executable for the game starts and initializes.
        /// </summary>
        public void Initialize() { }

        /// <summary>
        /// This loads all the content within the game. Called after <see cref="Initialize"/>.
        /// </summary>
        public Task LoadContentAsync() => Task.CompletedTask;
        
        /// <summary>
        /// Called after each update.
        /// </summary>
        public void OnUpdate() { }

        /// <summary>
        /// Called after each draw.
        /// </summary>
        public void OnDraw() { }

        /// <summary>
        /// Called before a scene transition.
        /// </summary>
        public void OnSceneTransition() { }

        /// <summary>
        /// Called once the game exits.
        /// </summary>
        public void OnExit() { }
        
        /// <summary>
        /// Creates save data for the game.
        /// </summary>
        public SaveData CreateSaveData(string name) => new(name);

        /// <summary>
        /// Creates the client custom sound player.
        /// </summary>
        public ISoundPlayer CreateSoundPlayer() => new SoundPlayer();

        /// <summary>
        /// Creates a custom game profile for the game.
        /// </summary>
        public GameProfile CreateGameProfile() => new();

        /// <summary>
        /// Creates a custom game preferences for the game.
        /// </summary>
        public GamePreferences CreateGamePreferences() => new();

        public bool HasCursor => true;

        /// <summary>
        /// This is the name of the game, used when creating assets and loading save data.
        /// </summary>
        public string Name { get; }
    }
}
