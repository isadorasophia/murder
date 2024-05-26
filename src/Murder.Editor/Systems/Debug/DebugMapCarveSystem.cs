using Murder.Editor.Attributes;
using Murder.Systems;

namespace Murder.Editor.Systems
{
    [PathfindEditor]
    [WorldEditor(startActive: true)]
    internal class DebugMapCarveSystem : MapCarveCollisionSystem
    {
    }
}