using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Components;

namespace Murder.Editor.CustomDiagnostics;

[CustomDiagnostic(typeof(SpriteComponent))]
internal class SpriteComponentDiagnostic : ICustomDiagnostic
{
    public bool IsValid(string identifier, in object target, bool outputResult)
    {
        if (target is null)
        {
            return true;
        }

        SpriteComponent sprite = (SpriteComponent)target;
        if (sprite.AnimationGuid == Guid.Empty ||
            Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid) is not SpriteAsset asset)
        {
            return false;
        }

        if (!asset.Animations.ContainsKey(sprite.CurrentAnimation))
        {
            return false;
        }

        return true;
    }
}
