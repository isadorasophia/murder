using Murder.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Murder.Serialization
{
    /// <summary>
    /// Custom contract resolver for serializing our game assets.
    /// This currently filters out getters and filters in readonly fields.
    /// </summary>
    public class WritablePropertiesOnlyResolver : DefaultContractResolver
    {
        /// <summary>
        /// Only create properties that are able to be set.
        /// See: https://stackoverflow.com/a/18548894.
        /// </summary>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            return properties.Where(p => p.Writable).ToList();
        }

        /// <summary>
        /// While we ignore getter properties, we do not want to ignore readonly fields.
        /// </summary>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    property.Readable = true;
                    property.Writable = true;
                    break;
            }

            return property;
        }

        /// <summary>
        /// Make sure we fetch all the fields that define [ShowInEditor] in addition to JsonProperty.
        /// </summary>
        protected override List<MemberInfo> GetSerializableMembers(Type t)
        {
            List<MemberInfo> members = base.GetSerializableMembers(t);

            // Return fields that have the ShowInEditor attribute as well.
            var fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => Attribute.IsDefined(f, typeof(ShowInEditorAttribute)));

            return members.Union(fields).ToList();
        }
    }
}
