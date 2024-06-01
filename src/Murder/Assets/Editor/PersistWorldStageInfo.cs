

namespace Murder.Editor.Assets;

public readonly struct PersistWorldStageInfo
{
    public readonly HashSet<string> LockedGroups { get; init; } = [];
    public readonly HashSet<string> HiddenGroups { get; init; } = [];

    public PersistWorldStageInfo(HashSet<string> lockedGroups, HashSet<string> hiddenGroups)
    {
        LockedGroups = lockedGroups;
        HiddenGroups = hiddenGroups;
    }
}
