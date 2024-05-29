namespace Murder.Editor.Attributes;

/// <summary>
/// Attribute for systems which will show up in the "Sound Editor" mode.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class PathfindEditorAttribute : Attribute { }