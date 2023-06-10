namespace Murder.Editor.Attributes
{
    /// <summary>
    /// Attribute for systems which will show up in the "World" mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class WorldEditorAttribute : Attribute 
    {
        public readonly bool StartActive = true;

        public WorldEditorAttribute(bool startActive) => StartActive = startActive;
    }
}
