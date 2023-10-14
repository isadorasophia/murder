namespace Murder.Attributes
{
    /// <summary>
    /// This signalizes that a component is an intrinsic characteristic of the entity and 
    /// that it does not distinct as a separate entity.
    /// An entity with only intrinsic components will not be serialized.
    /// </summary>
    public class IntrinsicAttribute : Attribute { }
}