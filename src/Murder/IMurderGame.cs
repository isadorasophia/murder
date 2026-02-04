using Bang;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets;
using Murder.Assets.Save;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Sounds;
using Murder.Data;
using Murder.Save;
using System.Text.Json;

namespace Murder
{
    /// <summary>
    /// This is the main loop of a murder game. This has the callbacks to relevant events in the game.
    /// </summary>
    public interface IMurderGame
    {
        /// <summary>
        /// Serialization options. This is generated automatically by the game based on the assets.
        /// </summary>
        public JsonSerializerOptions Options { get; }

        /// <summary>
        /// Cache of all the components. This is generated automatically by the game according to the Bang data.
        /// </summary>
        public ComponentsLookup ComponentsLookup { get; }

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
        public void AfterDraw() { }

        /// <summary>
        /// Called before a scene transition.
        /// </summary>
        public void OnSceneTransition() { }

        /// <summary>
        /// Called once the game exits.
        /// </summary>
        public void OnClose() { }

        /// <summary>
        /// Creates save data for the game.
        /// </summary>
        public SaveData CreateSaveData(int slot) => new(slot, Version);

        /// <summary>
        /// Creates save data for the game.
        /// </summary>
        public SaveDataInfo CreateSaveDataInfo(float version, string name) => new(version, name);

        /// <summary>
        /// Creates the client custom sound player.
        /// </summary>
        public ISoundPlayer CreateSoundPlayer() => new SoundPlayer();

        /// <summary>
        /// Create an instance preloading the game.
        /// </summary>
        public IPreloadGame? TryCreatePreload() => null;

        /// <summary>
        /// Creates a custom game profile for the game.
        /// </summary>
        public GameProfile CreateGameProfile() => new();

        /// <summary>
        /// Creates a custom game preferences for the game.
        /// </summary>
        public GamePreferences CreateGamePreferences() => new();

        /// <summary>
        /// Creates a custom world processor.
        /// </summary>
        public WorldProcessor CreateWorldProcessor() => new();

        /// <summary>
        /// Allow the game to override a font based on localization settings.
        /// </summary>
        public int GetLocalizedFont(int index) => index;

        /// <summary>
        /// Creates a custom render context for the game.
        /// </summary>
        public RenderContext CreateRenderContext(GraphicsDevice graphicsDevice, Camera2D camera, RenderContextFlags settings) =>
            new(graphicsDevice, camera, settings);

        public bool HasCursor => true;

        /// <summary>
        /// Whether the game supports saving.
        /// </summary>
        public bool CanSave => true;

        /// <summary>
        /// This is the name of the game, used when creating assets.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// This is the name of the game, used when loading save data.
        /// </summary>
        public string SaveName => Name;

        /// <summary>
        /// This is the version of the game, used when checking for save compatibility.
        /// </summary>
        public float Version => 0;
    }
}