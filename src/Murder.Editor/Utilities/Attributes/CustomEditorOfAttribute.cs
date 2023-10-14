namespace Murder.Editor.Attributes
{
    public class CustomEditorOfAttribute : Attribute
    {
        public int Priority;
        public Type OfType;

        public CustomEditorOfAttribute(Type ofType, int priority = 1)
        {
            OfType = ofType;
            Priority = priority;
        }
    }
}