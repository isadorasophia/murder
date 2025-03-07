using Murder.Prefabs;

namespace Murder.Editor.Utilities;

public interface IStageCustomHelper
{
    public bool AddEventsForSelectedEntity(IEntity target, ref HashSet<string>? events);
}
