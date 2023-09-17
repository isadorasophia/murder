using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    public readonly struct GlobalShaderComponent : IComponent
    {
        [Slider]
        public readonly float DitherAmount = 0.9518f;

        public GlobalShaderComponent() { }
    }
}
