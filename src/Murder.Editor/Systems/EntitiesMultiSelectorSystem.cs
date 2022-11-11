using Bang.Systems;
using Murder.Components;
using Murder.Editor.Attributes;
using Bang.Contexts;

namespace Murder.Editor.Systems
{
    [TileEditor]
    [Filter(ContextAccessorFilter.AllOf, typeof(PositionComponent))]
    public class EntitiesMultiSelectorSystem : IRenderSystem
    {
    }
}
