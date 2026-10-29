using Bang.Components;
using Murder.Attributes;
using Murder.Core.Sounds;
using Murder.Utilities.Attributes;

namespace Murder.Components;

[Unique]
[RuntimeOnly]
public readonly struct BlockPlayEventsComponent : IComponent
{
    public readonly SoundLayer? Layer = null;

    public BlockPlayEventsComponent() { }

    public BlockPlayEventsComponent(SoundLayer layer) => Layer = layer;
}
