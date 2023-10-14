namespace Murder.Attributes
{
    /// <summary>
    /// This is an attribute used for a field guid that points to another entity instance within the world.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class InstanceIdAttribute : Attribute { }
}