namespace Murder.Attributes
{
    /// <summary>
    /// Overrides any other attribute and make sure this component is persisted in
    /// the entity serialization.
    /// </summary>
    public class PersistOnSaveAttribute : Attribute { }
}