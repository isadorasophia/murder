namespace Murder.Editor.Attributes
{
    /// <summary>
    /// Indicates that a system will only be displayed on debug.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class OnlyShowOnDebugViewAttribute : Attribute
    {
    }
}