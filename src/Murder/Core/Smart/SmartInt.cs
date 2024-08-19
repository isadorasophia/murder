using Murder.Assets;

namespace Murder.Core.Smart;

public  struct SmartInt
{
    public readonly Guid Asset;
    public readonly int Index;
    public readonly int Custom;

    private SmartIntAsset? _cachedAsset = null;

    public SmartInt()
    {
    }
    public SmartInt(int custom) : this(Guid.Empty, 0, custom) { }

    public SmartInt(Guid guid, int index, int custom) : this()
    {
        Asset = guid;
        Index = index;
        Custom = custom;
    }

    public int Int
    {
        get
        {
            if (_cachedAsset is not SmartIntAsset asset)
            {
                if (Asset != Guid.Empty && Game.Data.TryGetAsset<SmartIntAsset>(Asset) is SmartIntAsset loaded)
                {
                    _cachedAsset = loaded;
                    asset = loaded;
                }
                else
                {
                    return Custom;
                }
            }
            return asset.Values[Index];
        }
    }
}
