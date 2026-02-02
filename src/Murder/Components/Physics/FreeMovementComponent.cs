using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components;

public enum FreeMovementFlags
{
    None = 0,

    /// <summary>
    /// Do not automatically clear this.
    /// </summary>
    DoNotClear = 1 << 0,
    RespectTiles = 1 << 1,
}

[DoNotPersistOnSave, HideInEditor]
public readonly struct FreeMovementComponent : IComponent
{
    public readonly FreeMovementFlags Flags = FreeMovementFlags.None;

    public FreeMovementComponent()
    {
    }

    public FreeMovementComponent(FreeMovementFlags flags)
    {
        Flags = flags;
    }
}