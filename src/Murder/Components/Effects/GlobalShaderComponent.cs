using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    internal struct GlobalShaderComponent : IComponent
    {
        [Slider]
        public float DitherAmount = 0.9518f;

        public GlobalShaderComponent() { }
    }
}
