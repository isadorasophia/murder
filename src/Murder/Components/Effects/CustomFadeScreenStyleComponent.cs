using Bang.Components;
using Murder.Core.Sounds;

namespace Murder.Components;

[Unique]
public readonly struct CustomFadeScreenStyleComponent: IComponent
{
    public readonly string? CustomFadeImage = null;
    public readonly SoundEventId? CustomSoundFadeIn = null;

    public CustomFadeScreenStyleComponent() { }
}
