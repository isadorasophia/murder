using Murder.Core.Geometry;
using Murder.Editor.Reflection;
using Murder.ImGuiExtended;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<Point>))]
    internal class ImmutableArrayPointField : ImmutableArrayField<Point>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out Point element)
        {
            element = Point.Zero;
            if (ImGuiHelpers.IconButton('', $"{member.Name}_add", Game.Data.GameProfile.Theme.Accent))
            {
                return true;
            }
            return false;
        }
    }
}
