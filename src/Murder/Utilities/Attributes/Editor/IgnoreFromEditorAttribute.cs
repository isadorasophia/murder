namespace Murder.Attributes
{
    /// <summary>
    /// Force ignore a system from showing up in the diagnostics stage.
    /// </summary>
    public class IgnoreFromEditorAttribute : Attribute
    {
        public IgnoreFromEditorAttribute() { }
    }
}