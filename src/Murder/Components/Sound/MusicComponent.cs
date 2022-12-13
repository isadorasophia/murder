using Murder.Attributes;
using Bang.Components;

namespace Murder.Components
{
    /// <summary>
    /// Music component which will be immediately played and destroyed.
    /// </summary>
    public readonly struct MusicComponent : IComponent
    {
        [Sound]
        public readonly string MusicName = string.Empty;

        public MusicComponent() { }
    }
}
