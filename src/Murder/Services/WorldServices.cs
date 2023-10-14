using Bang;
using Murder.Core;

namespace Murder.Services
{
    public static class WorldServices
    {
        public static Guid Guid(this World world) => ((MonoWorld)world).WorldAssetGuid;
    }
}