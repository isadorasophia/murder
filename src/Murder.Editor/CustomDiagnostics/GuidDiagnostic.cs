using Murder.Assets;

namespace Murder.Editor.CustomDiagnostics;

[CustomDiagnostic(typeof(Guid))]
internal class GuidDiagnostic : ICustomDiagnostic
{
    public bool IsValid(string identifier, in object target, bool outputResult)
    {
        Guid guid = (Guid)target;
        if (Game.Data.TryGetAsset(guid) is not GameAsset asset)
        {
            return false;
        }

        return true;
    }
}
