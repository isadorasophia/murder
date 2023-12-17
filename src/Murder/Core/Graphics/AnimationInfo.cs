namespace Murder.Core.Graphics;

public readonly struct AnimationInfo
{
    public static readonly AnimationInfo Default = new();
    public static readonly AnimationInfo Ui = new()
    {
        UseScaledTime = true
    };

    public float Start { get; init; } = 0f;
    public float Duration { get; init; } = -1f;
    public bool UseScaledTime { get; init; } = false;
    public bool Loop { get; init; } = true;
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// If different than -1, it will ignore <see cref="UseScaledTime"/> and use the
    /// time specified in this field.
    /// </summary>
    public float OverrideCurrentTime { get; init; } = -1;

    public AnimationInfo()
    {
    }

    public AnimationInfo(string name, float start) : this()
    {
        Name = name;
        Start = start;
    }

    public AnimationInfo(string name) : this()
    {
        Name = name;
    }
}