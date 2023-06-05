using Bang.Components;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [RuntimeOnly, DoNotPersistOnSave]
    
    public readonly struct InCameraComponent : IComponent
    {
        public readonly Vector2 RenderPosition = Vector2.Zero;

        public InCameraComponent(Vector2 renderPosition)
        {
            RenderPosition = renderPosition;
        }
    }
}
