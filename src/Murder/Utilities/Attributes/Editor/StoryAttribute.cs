namespace Murder.Utilities.Attributes
{
    /// <summary>
    /// Attribute used for IComponent structs that will change according to 
    /// a "story". This is used for debugging and filtering in editor.
    /// </summary>

    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class StoryAttribute : Attribute { }
}