
using Murder.Assets;

namespace Murder.Core.Smart;
public struct SmartFloat
{
    public readonly Guid Asset;
    public readonly int Index;
    public readonly float Custom;

    private SmartFloatAsset? _cachedAsset = null;

    public SmartFloat()
    {
    }
    public SmartFloat(float custom) : this(Guid.Empty, 0, custom) { }

    public SmartFloat(Guid guid, int index, float custom) : this()
    {
        Asset = guid;
        Index = index;
        Custom = custom;
    }

    public float Float
    {
        get
        {
            if (_cachedAsset is not SmartFloatAsset asset)
            {
                if (Asset != Guid.Empty && Game.Data.TryGetAsset<SmartFloatAsset>(Asset) is SmartFloatAsset loaded)
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