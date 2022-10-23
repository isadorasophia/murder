namespace Murder.Attributes
{
    /// <summary>
    /// This signalizes that an entity should be skipped altogether if
    /// it has a component with that attribute.
    /// </summary>
    public class DoNotPersistEntityOnSaveAttribute : Attribute { }
}
