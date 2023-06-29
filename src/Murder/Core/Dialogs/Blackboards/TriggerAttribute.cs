namespace Murder.Core.Dialogs
{
    /// <summary>
    /// Attribute of fields that should be immediately set off once they are
    /// turned on.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TriggerAttribute : Attribute
    {
    }
}
