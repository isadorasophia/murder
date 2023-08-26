using Bang.Components;
using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;

namespace Murder.Components;

[RuntimeOnly]
public readonly struct TextureComponent : IComponent
{
    public readonly Texture2D Texture;
    public readonly TargetSpriteBatches TargetSpriteBatch = TargetSpriteBatches.Gameplay;
    
    public readonly bool AutoDispose = true;

    public TextureComponent(Texture2D texture, TargetSpriteBatches targetSpriteBatch)
    {
        Texture = texture;
        TargetSpriteBatch = targetSpriteBatch;
    }
}
