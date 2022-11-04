namespace Murder.Services.Console
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public readonly string Help;

        public CommandAttribute(string help) => Help = help;
    }
}
