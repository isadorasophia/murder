using Microsoft.Xna.Framework.Input;

namespace Murder.Core.Input;

/// <summary>
/// Represents a sequence of Key with optional modifiers.
/// </summary>
/// <param name="key">Key that must be pressed to trigger this chord.</param>
/// <param name="modifiers">Optional modifiers that need to be pressed along with the key.</param>
public sealed class Chord(Keys key, params Keys[] modifiers)
{
    /// <summary>
    /// The key that needs to be pressed to trigger this chord.
    /// </summary>
    public Keys Key { get; } = key;
    
    /// <summary>
    /// A list of optional modifiers that need to be pressed along with <see cref="Key"/> in order to trigger this chord.
    /// </summary>
    public Keys[] Modifiers { get; } = modifiers;

    ///  <inheritdoc cref="object"/>
    public override string ToString()
        => Modifiers.Length > 0 ? string.Join("+", Modifiers.ToList().Append(Key).Select(PrettyKeyName)) : Key.ToString();

    /// <summary>
    /// Returns a printable name for the passed key
    /// </summary>
    private static string PrettyKeyName(Keys key) => key switch
    {
        Keys.LeftShift or Keys.RightShift => "Shift",
        Keys.LeftControl or Keys.RightControl => "Ctrl",
        Keys.LeftWindows or Keys.RightWindows => "Cmd",
        _ => key.ToString()
    };

    /// <summary>
    /// Converts a single key into a chord.
    /// </summary>
    /// <returns>A chord triggered by the single key without modifiers.</returns>
    public static implicit operator Chord(Keys key) => new(key);
}