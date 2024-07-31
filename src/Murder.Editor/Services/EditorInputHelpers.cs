using Microsoft.Xna.Framework.Input;
using Murder.Core.Input;
using Murder.Utilities;

namespace Murder.Editor.Services;

public static class EditorInputHelpers
{
    public static Chord CTRL_C = new Chord(Keys.C, InputHelpers.OSActionModifier);
    public static Chord CTRL_V = new Chord(Keys.V, InputHelpers.OSActionModifier);
}
