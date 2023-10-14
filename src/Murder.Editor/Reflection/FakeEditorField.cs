using Murder.Diagnostics;

namespace Murder.Editor.Reflection
{
    internal class FakeEditorField : EditorMember
    {
        private readonly Type _actualType;
        private readonly string _displayName;
        private readonly bool _isReadOnly;
        private readonly Type? _customElement;

        private readonly EditorMember _underlyingMember;

        internal FakeEditorField(EditorMember member, Type actualType, string displayName, bool isReadOnly, Type? customElement) : base(member.Member)
        {
            _actualType = actualType;
            _displayName = displayName;
            _isReadOnly = isReadOnly;
            _customElement = customElement;

            _underlyingMember = member;
        }

        public override Type? CustomElementType => _customElement;

        public override string Name => _displayName;

        public override object? GetValue(object? obj)
        {
            return _underlyingMember.GetValue(obj);
        }

        public override void SetValue(object? obj, object? value)
        {
            _underlyingMember.SetValue(obj, value);
        }

        public override Type Type => _actualType;

        public override bool IsReadOnly => _isReadOnly;
    }
}