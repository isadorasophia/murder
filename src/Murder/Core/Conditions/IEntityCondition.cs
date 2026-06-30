using Bang;
using Bang.Entities;

namespace Murder.Core;

public interface IEntityCondition
{
    public bool IsSatisfiedBy(World world, Entity e);
}