using Bang.Components;
using Murder.Core.Sounds;

namespace Murder.Components
{
    // I am not entirely sure how this component is supposed to be used yet.
    // I created it for testing purposes.
    public readonly struct SoundParameterComponent : IComponent
    {
        public readonly ParameterId? Parameter = default;

        public SoundParameterComponent() { }

        public SoundParameterComponent(ParameterId parameter)
        {
            Parameter = parameter;
        }
    }
}
