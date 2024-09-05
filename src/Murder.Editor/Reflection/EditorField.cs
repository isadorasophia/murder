using System.Reflection;

namespace Murder.Editor.Reflection
{
    internal class EditorField : EditorMember
    {
        private readonly FieldInfo _field;

        public EditorField(FieldInfo field) : base(field) =>
            _field = field;

        public override object? GetValue(object? obj)
        {
            return _field.GetValue(obj);
        }

        public override void SetValue(object? obj, object? value)
        {
            try
            {
                _field.SetValue(obj, value);
            }
            catch (ArgumentException)
            {
                if (_field.DeclaringType is not null && !_field.DeclaringType.IsEnum && 
                    value is not null && value.GetType().IsEnum)
                {
                    value = Convert.ToInt32(value);
                    _field.SetValue(obj, value);
                }
            }
        }

        public override Type Type => _field.FieldType;

        public override bool IsReadOnly => false;
    }
}