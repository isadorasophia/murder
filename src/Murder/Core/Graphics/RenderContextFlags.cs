namespace Murder.Core.Graphics;

/// <summary>
/// These are the flags consumed by <see cref="RenderContext"/>.
/// </summary>
[Flags]
public enum RenderContextFlags
{
    None = 0,

    /// <summary>
    /// Whether it should apply custom shaders as part of the processing.
    /// </summary>
    CustomShaders = 0b1,

    /// <summary>
    /// Whether it should set the debug batches.
    /// </summary>
    Debug = 0b10,

    /// <summary>
    /// Whether it should set the debug batches.
    /// </summary>
    Editor = 0b100

}