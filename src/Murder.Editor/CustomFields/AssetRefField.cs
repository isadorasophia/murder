using ImGuiNET;
using Murder.Assets;
using Murder.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Utilities;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(AssetRef<>))]
    public class AssetRefField<T> : CustomField where T : GameAsset 
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            AssetRef<T> assetRef = (AssetRef<T>)fieldValue!;

            bool changed = DrawAssetRefField(ref assetRef);
            return (changed, assetRef);
        }

        public static bool DrawAssetRefField(ref AssetRef<T> assetRef)
        {
            Guid guid = assetRef.Guid;

            bool changed = SearchBox.SearchAsset(ref guid, typeof(T));
            if (changed)
            {
                assetRef = new(guid);
            }
            
            return changed;
        }
    }
}