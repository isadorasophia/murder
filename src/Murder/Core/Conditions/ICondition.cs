using Bang;

namespace Murder.Core;

/// <summary>
/// A custom interface that checks if a condition is true.
/// </summary>
public interface ICondition
{
    public bool IsSatisfiedBy(World world);
}