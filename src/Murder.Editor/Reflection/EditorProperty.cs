using System.Reflection;

namespace Murder.Editor.Reflection
{
    internal class EditorProperty : EditorMember
    {
        private readonly PropertyInfo _property;

        public EditorProperty(PropertyInfo property) : base(property) =>
            _property = property;

        public override object? GetValue(object? obj)
        {
            return _property.GetValue(obj);
        }

        public override void SetValue(object? obj, object? value)
        {
            _property.SetValue(obj, value);
        }

        public override Type Type => _property.PropertyType;

        /// <summary>
        /// Do not modify properties without a setter.
        /// </summary>
        public override bool IsReadOnly => _property.GetSetMethod() is null;
    }
}