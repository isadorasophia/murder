using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using System.Collections.Immutable;

namespace Murder.Systems.Graphics
{
    [Watch(typeof(AsepriteComponent))]
    internal class AsepriteAnimationManagerSystem : IReactiveSystem
    {
        public ValueTask OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                var anim = e.GetAseprite();

                if (anim.AnimationStartedTime == 0)
                {
                    e.ReplaceComponent(anim.StartNow(e.HasPauseAnimation() ? Game.Now : Game.NowUnescaled));
                }

                e.RemoveAnimationComplete();
            }
            return default;
        }

        public ValueTask OnModified(World world, ImmutableArray<Entity> entities)
        {
            return default;
        }

        public ValueTask OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            return default;
        }
    }
}
