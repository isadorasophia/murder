using Bang;
using Bang.StateMachines;
using Murder.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;
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

        private static readonly Assembly _systemAssembly = typeof(object).Assembly;
        private static readonly Dictionary<Type, List<FieldInfo>?> _typeMembers = [];

        /// <summary>
        /// Make sure we fetch all the fields that define [ShowInEditor] or [Serialize] in addition to JsonProperty.
        /// </summary>
        protected override List<MemberInfo> GetSerializableMembers(Type t)
        {
            List<MemberInfo> members = base.GetSerializableMembers(t);

            Type? tt = t;

            HashSet<string>? membersCache = null;
            while (tt is not null && tt.Assembly != _systemAssembly)
            {
                if (_typeMembers.TryGetValue(tt, out List<FieldInfo>? fields))
                {
                    if (fields is not null)
                    {
                        members.AddRange(fields);
                    }
                }
                else
                {
                    List<FieldInfo>? extraFields = null;
                    foreach (FieldInfo f in tt.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (Attribute.IsDefined(f, typeof(ShowInEditorAttribute)) || Attribute.IsDefined(f, typeof(SerializeAttribute)))
                        {
                            if (membersCache is null)
                            {
                                membersCache = [];

                                foreach (MemberInfo m in members)
                                {
                                    membersCache.Add(m.Name);
                                }
                            }

                            if (!membersCache.Contains(f.Name))
                            {
                                membersCache.Add(f.Name);
                                members.Add(f);

                                extraFields ??= [];
                                extraFields.Add(f);
                            }
                        }
                    }

                    _typeMembers[tt] = extraFields;
                }

                tt = tt.BaseType;
            }

            return members;
        }
    }
}