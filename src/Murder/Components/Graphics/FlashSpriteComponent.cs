using Bang.Components;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [RuntimeOnly]
    public readonly struct FlashSpriteComponent : IComponent
    {
        public readonly float DestroyAtTime;

        public FlashSpriteComponent(float destroyTimer)
        {
            DestroyAtTime = destroyTimer;
        }
    }
}