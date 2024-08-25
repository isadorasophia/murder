using Murder.Assets;
using Murder.Editor.Attributes;
using Murder.Editor.CustomFields;
using Murder.Utilities;

namespace Murder.Editor.CustomComponents
{
    [CustomComponentOf(typeof(AssetRef<>))]
    public class AssetRefComponent<T> : CustomComponent where T : GameAsset
    {
        protected override bool DrawAllMembersWithTable(ref object target)
        {
            AssetRef<T> assetRef = (AssetRef<T>)target;

            bool changed = AssetRefField<T>.DrawAssetRefField(ref assetRef);
            if (changed)
            {
                target = assetRef;
            }

            return changed;
        }
    }
}