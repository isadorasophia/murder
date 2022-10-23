namespace Editor.CustomComponents
{
    internal class CustomComponentOfAttribute : Attribute
    {
        public readonly Type OfType;

        public readonly int Priority;

        public CustomComponentOfAttribute(Type ofType, int priority = 1)
        {
            OfType = ofType;
            Priority = priority;
        }
    }
}
