namespace Murder.Editor.Assets;

public struct PersistWorldStageInfo
{
    public HashSet<string> LockedGroups { get; init; } = [];
    public HashSet<string> HiddenGroups { get; init; } = [];
    
    public PersistWorldStageInfo(HashSet<string> lockedGroups, HashSet<string> hiddenGroups)
    {
        LockedGroups = lockedGroups;
        HiddenGroups = hiddenGroups;
    }
}
