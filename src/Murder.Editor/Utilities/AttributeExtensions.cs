﻿using Murder.Attributes;
using Murder.Editor.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System
{
    public abstract class AttributeExtensions
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

        /// <summary>
        /// Find out the corresponding game asset id if the <paramref name="member"/> has an 
        /// <see cref="GameAssetIdAttribute"/> or <see cref="GameAssetDictionaryIdAttribute"/>.
        /// </summary>
        public static bool FindGameAssetType(EditorMember member, [NotNullWhen(true)] out GameAssetIdInfo? info)
        {
            if (TryGetAttribute(member, out GameAssetIdAttribute? gameAssetAttr))
            {
                info = new(gameAssetAttr.AssetType, gameAssetAttr.AllowInheritance);
                return true;
            }

            if (TryGetAttribute(member, out GameAssetDictionaryIdAttribute? gameAssetDictAttr))
            {
                Type t;
                if (member.Name == "Key")
                {
                    t = gameAssetDictAttr.Key;
                }
                else
                {
                    t = gameAssetDictAttr.Value;
                }

                info = new(t, allowInheritance: false);
                return true;
            }

            info = default;
            return false;
        }
    }
}