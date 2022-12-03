namespace Murder.Editor.Attributes
{
    /// <summary>
    /// Attribute for systems which will show up in the "Tile Editor" mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CutsceneEditorAttribute : Attribute { }
}
