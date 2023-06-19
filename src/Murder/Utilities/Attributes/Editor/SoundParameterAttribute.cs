namespace Murder.Utilities.Attributes
{
    /// <summary>
    /// Attribute used for IComponent structs that will change according to 
    /// a "story". This is used for debugging and filtering in editor.
    /// </summary>

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class SoundParameterAttribute : Attribute 
    {
        public readonly bool IsGlobal;

        public SoundParameterAttribute(bool isGlobal) => IsGlobal = isGlobal;
    }
}
