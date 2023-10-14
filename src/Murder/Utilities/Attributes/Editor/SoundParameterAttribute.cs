namespace Murder.Utilities.Attributes
{
    public enum SoundParameterKind
    {
        Local = 0,
        Global = 1,
        All = 2
    }

    /// <summary>
    /// Attribute used for IComponent structs that will change according to 
    /// a "story". This is used for debugging and filtering in editor.
    /// </summary>

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class SoundParameterAttribute : Attribute
    {
        public readonly SoundParameterKind Kind;

        public SoundParameterAttribute(SoundParameterKind kind) => Kind = kind;
    }
}