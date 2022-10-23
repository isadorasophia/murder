using Editor.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System
{
    internal abstract class AttributeExtensions
    {
        public static bool IsDefined(EditorMember element, Type attributeType) =>
            Attribute.IsDefined(element.Member, attributeType);

        public static bool TryGetAttribute<T>(MemberInfo member, [NotNullWhen(true)] out T? attribute) where T : Attribute
        {
            if (Attribute.IsDefined(member, typeof(T)) &&
                Attribute.GetCustomAttribute(member, typeof(T)) is T customAttribute)
            {
                attribute = customAttribute;
                return true;
            }

            attribute = default;
            return false;
        }

        public static bool TryGetAttribute<T>(EditorMember member, [NotNullWhen(true)] out T? attribute) where T : Attribute
            => TryGetAttribute(member.Member, out attribute);
    }
}