using Microsoft.Xna.Framework.Input;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets;

public class InputGraphicsAsset : GameAsset
{
    public override char Icon => '';
    public override string EditorFolder => "#Ui";
    public override Vector4 EditorColor => "#f4eb6f".ToVector4Color();

    public ImmutableArray<ButtonGraphics> Graphics = ImmutableArray<ButtonGraphics>.Empty;

    public Portrait KeyboardDefault = new();
    public Portrait GamepadDefault = new();
    public Portrait MouseDefault = new();
    public Portrait GamepadAxisDefault = new();

    public readonly struct ButtonGraphics
    {
        public readonly Portrait Icon = new();
        public readonly InputButton InputButton = new();
        public readonly string? Text = null;
        public ButtonGraphics()
        {

        }
    }
}
