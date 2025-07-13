using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Components;

namespace Murder.Editor.CustomDiagnostics;

[CustomDiagnostic(typeof(AgentSpriteComponent))]
internal class AgentSpriteComponentDiagnostic : ICustomDiagnostic
{
    public bool IsValid(string identifier, in object target, bool outputResult)
    {
        if (target is null)
        {
            return true;
        }

        AgentSpriteComponent agent = (AgentSpriteComponent)target;
        if (agent.AnimationGuid == Guid.Empty ||
            Game.Data.TryGetAsset<SpriteAsset>(agent.AnimationGuid) is not SpriteAsset asset)
        {
            return false;
        }

        return true;
    }
}
