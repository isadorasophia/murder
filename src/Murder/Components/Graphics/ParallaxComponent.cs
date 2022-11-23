using Bang.Components;
using Murder.Attributes;

namespace Murder.Components.Graphics
{
    public readonly struct ParallaxComponent : IComponent
    {
        /// <summary>
        /// How much parallax this entity has. 0 never moves, 1 moves normaly and more than 1 is the foreground.
        /// </summary>
        [Slider(0,2)]
        public readonly float Factor = 1;

        public ParallaxComponent() { }
    }
}
