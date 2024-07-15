using Murder.Data;
using System.Collections.Immutable;

namespace Murder.Core.Sounds
{
    [Flags]
    public enum SoundProperties
    {
        None = 0,
        Persist = 0b1,
        SkipIfAlreadyPlaying = 0b10,
        StopOtherEventsInLayer = 0b100
    }

    public struct PlayEventInfo
    {
        public SoundProperties Properties { get; init; } = SoundProperties.None;

        /// <summary>
        /// Event layer, only persisted if <see cref="Properties"/> has <see cref="SoundProperties.Persist"/> set.
        /// </summary>
        public SoundLayer Layer { get; init; } = SoundLayer.Any;

        public SoundSpatialAttributes? Attributes { get; init; } = null;

        public PlayEventInfo() { }
    }

    public interface ISoundPlayer
    {
        /// <summary>
        /// This will initialize the fmod libraries, but not load any banks.
        /// </summary>
        /// <param name="resourcesPath">The relative path to the resource binaries.</param>
        public void Initialize(string resourcesPath);

        /// <summary>
        /// This will load the actual bank content asynchonously.
        /// </summary>
        public Task LoadContentAsync(PackedSoundData? packedData);

        /// <summary>
        /// This will reload the content of all the fmod banks in the application.
        /// </summary>
        public Task ReloadAsync();

        /// <summary>
        /// Update listener information (e.g. position and facing).
        /// </summary>
        /// <param name="attributes">Attributes of the origin of this sound.</param>
        public void UpdateListener(SoundSpatialAttributes attributes);

        /// <summary>
        /// Update spatial attributes for a specific event instance.
        /// </summary>
        /// <param name="id">Event event instance id.</param>
        /// <param name="attributes">Attributes of the origin of this sound.</param>
        /// <returns>
        /// False if no event instance is found in the world, or something failed.
        /// </returns>
        public bool UpdateEvent(SoundEventId id, SoundSpatialAttributes attributes);

        public void Update();

        /// <summary>
        /// Play a sound/event with the id of <paramref name="id"/>.
        /// </summary>
        public ValueTask PlayEvent(SoundEventId id, PlayEventInfo info);

        public void SetParameter(SoundEventId instance, ParameterId parameter, float value);

        public void SetGlobalParameter(ParameterId parameter, float value);

        public float GetGlobalParameter(ParameterId parameter);

        public bool Resume(SoundLayer layer);

        public bool Pause(SoundLayer layer);

        /// <summary>
        /// Stop all sounds tied to <paramref name="layer"/>.
        /// </summary>
        /// <param name="layer">The target sound layer.</param>
        /// <param name="fadeOut">Whether it should stop with a fade out.</param>
        /// <returns>Whether it successfully stopped.</returns>
        public bool Stop(SoundLayer layer, bool fadeOut);

        /// <summary>
        /// Stop a specific sound event id.
        /// If <paramref name="fadeOut"/> is set, this will stop with a fadeout.
        /// </summary>
        /// <param name="id">Whether it should stop all events or only a specific one.</param>
        /// <param name="fadeOut">Apply fadeout on stop.</param>
        public bool Stop(SoundEventId? id, bool fadeOut);

        /// <summary>
        /// Change volume.
        /// </summary>
        public void SetVolume(SoundEventId? id, float volume);

        /// <summary>
        /// Fetch a list of all the banks when serializing it, separated by the supported platform.
        /// </summary>
        public ImmutableDictionary<string, List<string>> FetchAllBanks();

        /// <summary>
        /// Fetch a list of all the plugins when serializing it.
        /// </summary>
        public ImmutableArray<string> FetchAllPlugins();
    }
}