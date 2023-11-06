namespace Murder.Attributes
{
    /// <summary>
    /// This gets rather complicated, but this will persist only one component for the entity in the save.
    /// These are for cases that we want to persist an entity id for an entity with a specific property,
    /// e.g. the player.
    /// </summary>
    public class OnlyPersistThisComponentForEntityOnSaveAttribute : Attribute { }
}