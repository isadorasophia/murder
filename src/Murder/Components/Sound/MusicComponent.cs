using Bang.Components;
using Murder.Core.Sounds;

namespace Murder.Components
{
    /// <summary>
    /// Music component which will be immediately played and destroyed.
    /// </summary>
    public readonly struct MusicComponent : IComponent
    {
        public readonly SoundEventId Id = new();

        public MusicComponent() { }
    }
}
