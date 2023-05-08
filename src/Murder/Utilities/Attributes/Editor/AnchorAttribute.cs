namespace Murder.Utilities.Attributes
{
    /// <summary>
    /// Attribute for string fields that are actually an anchor of a state machine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class AnchorAttribute : Attribute { }
}
