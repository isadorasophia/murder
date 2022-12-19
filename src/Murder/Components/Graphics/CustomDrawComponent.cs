using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct CustomDrawComponent : IComponent
    {
        public readonly Action<RenderContext> Draw;

        public CustomDrawComponent(Action<RenderContext> draw)
        {
            Draw = draw;
        }
    }
}
