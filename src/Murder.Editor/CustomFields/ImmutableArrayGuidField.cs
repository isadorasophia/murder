using Murder.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<Guid>))]
    internal class ImmutableArrayGuidField : ImmutableArrayField<Guid>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out Guid element)
        {
            element = Guid.Empty;
            if (AttributeExtensions.FindGameAssetType(member, out Type? assetType))
            {
                var changed = SearchBox.SearchAsset(ref element, assetType);
                if (changed)
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool DrawElement(ref Guid element, EditorMember member, int _)
        {
            if (AttributeExtensions.FindGameAssetType(member, out Type? assetType))
            {
                var changed = SearchBox.SearchAsset(ref element, assetType);
                if (changed)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
