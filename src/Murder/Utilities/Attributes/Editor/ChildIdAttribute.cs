namespace Murder.Utilities.Attributes
{
    /// <summary>
    /// Attribute for string fields that point to an entity child id.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class ChildIdAttribute : Attribute { }
}
