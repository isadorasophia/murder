using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Components
{
    [RuntimeOnly, DoNotPersistOnSave, KeepOnReplace]
    public readonly struct InCameraComponent : IComponent
    {
        public readonly Vector2 RenderPosition = Vector2.Zero;

        public InCameraComponent(Vector2 renderPosition)
        {
            RenderPosition = renderPosition;
        }
    }
}