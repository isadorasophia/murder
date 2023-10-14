namespace Murder.Core.Dialogs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BlackboardAttribute : Attribute
    {
        public readonly string Name;

        public readonly bool IsDefault = false;

        public BlackboardAttribute(string name) => Name = name;

        public BlackboardAttribute(string name, bool @default) =>
            (Name, IsDefault) = (name, @default);
    }
}