namespace Murder.Editor.CustomFields
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CustomFieldOfAttribute : Attribute
    {
        public readonly Type OfType;

        public readonly int Priority;

        public CustomFieldOfAttribute(Type ofType, int priority = 1)
        {
            OfType = ofType;
            Priority = priority;
        }
    }
}
