namespace Murder.Editor
{
    /// <summary>
    /// Scan a field for inconsistencies and report those.
    /// </summary>
    public interface ICustomDiagnostic
    {
        public bool IsValid(string identifier, in object target, bool outputResult);
    }
}