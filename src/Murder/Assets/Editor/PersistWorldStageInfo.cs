namespace Murder.Editor.Assets;

public struct PersistWorldStageInfo
{
    public HashSet<string> LockedGroups { get; init; } = [];
    public HashSet<string> HiddenGroups { get; init; } = [];

    public int? HiddenTiles { get; init; } = null;

    public PersistWorldStageInfo(HashSet<string> lockedGroups, HashSet<string> hiddenGroups, int? hiddenTiles)
    {
        LockedGroups = lockedGroups;
        HiddenGroups = hiddenGroups;

        HiddenTiles = hiddenTiles;
    }
}
