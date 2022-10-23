namespace Murder.Attributes
{
    /// <summary>
    /// Attribute that a field references a sound.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SoundAttribute : Attribute { }
}
