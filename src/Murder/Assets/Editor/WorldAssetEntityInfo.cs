using Murder.Assets;
using Murder.Attributes;

namespace Murder.Assets;

public readonly struct WorldAssetEntityInfo
{
    [GameAssetId<WorldAsset>]
    public Guid World { get; init; } = Guid.Empty;
    public Guid Entity { get; init; } = Guid.Empty;

    public WorldAssetEntityInfo() { }
}
