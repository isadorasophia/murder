using Bang;
using Murder.Assets;
using Murder.Core;

namespace Murder.Editor.Services
{
    internal static class EditorStoryServices
    {
        public static WorldAsset GetWorldAsset(World world)
        {
            Guid guid = ((MonoWorld)world).WorldAssetGuid;
            return Game.Data.GetAsset<WorldAsset>(guid);
        }
    }
}