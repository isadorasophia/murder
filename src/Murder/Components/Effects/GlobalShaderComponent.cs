using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    public struct GlobalShaderComponent : IComponent
    {
        [Slider]
        public float DitherAmount = 0.9518f;

        public GlobalShaderComponent() { }
    }
}
