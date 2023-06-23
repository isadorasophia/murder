namespace Murder.Editor
{
    public class CustomDiagnosticAttribute : Attribute
    {
        public readonly Type Target;

        public CustomDiagnosticAttribute(Type target) => Target = target;
    }
}
