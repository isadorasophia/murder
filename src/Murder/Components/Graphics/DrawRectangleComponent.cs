using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;

namespace Murder.Components
{
    public readonly struct DrawRectangleComponent : IComponent
    {
        public readonly bool Fill = false;
        public readonly int LineWidth = 1;
        public readonly Color Color = Color.Black;

        [Slider]
        public readonly float Sorting = 0.4f;

        public DrawRectangleComponent() { }
    }
}