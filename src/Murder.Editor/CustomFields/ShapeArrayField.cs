using Murder.Core.Geometry;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<IShape>))]
    internal class ShapeArrayField : ImmutableArrayField<IShape>
    {
        protected override bool Add(in EditorMember _, [NotNullWhen(true)] out IShape? element)
        {
            element = null;

            if (SearchBox.SearchShapes() is Type shapeType)
            {
                if (Activator.CreateInstance(shapeType) is IShape shape)
                {
                    element = shape;
                    return true;
                }
            }

            return false;
        }
    }
}
