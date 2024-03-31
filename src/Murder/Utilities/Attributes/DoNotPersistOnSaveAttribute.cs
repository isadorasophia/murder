namespace Murder.Attributes;

public class DoNotPersistOnSaveAttribute : Attribute 
{
    /// <summary>
    /// This will dismiss this attribute and persist the component on the serialization if the following IComponent type is present.
    /// </summary>
    public readonly Type? ExceptIfComponentIsPresent = null;

    public DoNotPersistOnSaveAttribute() { }

    public DoNotPersistOnSaveAttribute(Type exceptIfComponentIsPresent) => ExceptIfComponentIsPresent = exceptIfComponentIsPresent;
}
