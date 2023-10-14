using System.Reflection;

namespace Murder.Editor.Reflection
{
    public abstract class EditorMember
    {
        private readonly MemberInfo _member;

        public MemberInfo Member => _member;

        public static EditorMember Create(MemberInfo member)
        {
            return member switch
            {
                FieldInfo field => new EditorField(field),
                PropertyInfo property => new EditorProperty(property),
                _ => throw new NotImplementedException($"Editor does not implement serializer for {member.GetType().Name}"),
            };
        }

        protected EditorMember(MemberInfo member) =>
            _member = member;

        public abstract object? GetValue(object? obj);

        public abstract void SetValue(object? obj, object? value);

        public virtual void SetValue<T>(ref T t, object? value)
        {
            object? obj = t;
            SetValue(obj, value);

            t = (T)obj!;
        }

        public virtual string Name => _member.Name;

        public abstract Type Type { get; }

        public abstract bool IsReadOnly { get; }

        /// <summary>
        /// Field used when the type hash a custom element type.
        /// For example, on a collection of interfaces.
        /// </summary>
        public virtual Type? CustomElementType => default;

        public EditorMember CreateFrom(Type type, string name, bool isReadOnly = false, Type? element = default) =>
            new FakeEditorField(this, type, name, isReadOnly, element);
    }
}