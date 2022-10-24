using Bang.Components;
using Murder.Core.Graphics;

namespace Murder.Components
{
    public readonly struct CustomDrawComponent : IComponent
    {
        public readonly Action<RenderContext> Draw;

        public CustomDrawComponent(Action<RenderContext> draw)
        {
            Draw = draw;
        }
    }
}
