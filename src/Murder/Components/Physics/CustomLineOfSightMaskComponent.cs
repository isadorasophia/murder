using Bang.Components;
using Murder.Core.Physics;
using Murder.Utilities.Attributes;

namespace Murder.Components;

public readonly struct CustomLineOfSightMaskComponent : IComponent
{
    [CollisionLayer]
    public readonly int Mask = CollisionLayersBase.SOLID | CollisionLayersBase.CARVE | 
        CollisionLayersBase.BLOCK_VISION | CollisionLayersBase.HOLE;

    public CustomLineOfSightMaskComponent() { }

    public CustomLineOfSightMaskComponent(int mask)
    {
        Mask = mask;
    }
}
