namespace Murder.Editor;

/// <summary>
/// Scan a field for inconsistencies and report those.
/// </summary>
public interface ICustomDiagnostic
{
    /// <summary>
    /// Whether we should run diagnostic on its fields as well.
    /// </summary>
    public bool Propagate => true;

    public bool IsValid(string identifier, in object target, bool outputResult);
}