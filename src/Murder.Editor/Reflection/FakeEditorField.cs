using Murder.Diagnostics;

namespace Murder.Editor.Reflection
{
    internal class FakeEditorField : EditorMember
    {
        private readonly Type _actualType;
        private readonly string _displayName;
        private readonly bool _isReadOnly;
        private readonly Type? _customElement;

        internal FakeEditorField(EditorMember member, Type actualType, string displayName, bool isReadOnly, Type? customElement) : base(member.Member) 
        {
            _actualType = actualType;
            _displayName = displayName;
            _isReadOnly = isReadOnly;
            _customElement = customElement;
        }

        public override Type? CustomElementType => _customElement;

        public override string Name => _displayName;

        public override object? GetValue(object? obj)
        {
            GameLogger.Fail("If we reached this path, it means that we are doing more than we thought... We might need to refactor CustomFields to support this!");
            throw new NotImplementedException();
        }

        public override void SetValue(object? obj, object? value)
        {
            GameLogger.Fail("If we reached this path, it means that we are doing more than we thought... We might need to refactor CustomFields to support this!");
            throw new NotImplementedException();
        }

        public override Type Type => _actualType;

        public override bool IsReadOnly => _isReadOnly;
    }
}
