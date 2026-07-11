using Bang;
using Bang.Entities;
using Murder.Services;

namespace Murder.Core;

public readonly struct IsPlayingAnimationCondition : IEntityCondition
{
    public readonly bool IsNot = false;
    public readonly string Animation = string.Empty;

    public IsPlayingAnimationCondition() { }

    public bool IsSatisfiedBy(World world, Entity e)
    {
        if (string.IsNullOrEmpty(Animation))
        {
            return false;
        }

        Entity root = EntityServices.FindRootEntity(e);

        bool isPlaying = EntityServices.IsAnimatingStartingWith(root, Animation);
        return IsNot ? !isPlaying : isPlaying;
    }
}
