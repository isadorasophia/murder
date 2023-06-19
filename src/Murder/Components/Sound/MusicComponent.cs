using Bang.Components;
using Murder.Core.Sounds;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// Music component which will be immediately played and destroyed.
    /// </summary>
    [Sound]
    public readonly struct MusicComponent : IComponent
    {
        public readonly SoundEventId Id = new();

        public MusicComponent() { }
    }
}
