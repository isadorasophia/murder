using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using System.Collections.Immutable;

namespace Murder.Systems.Graphics
{
    [Watch(typeof(SpriteComponent))]
    internal class SpriteAnimationManagerSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                var anim = e.GetSprite();

                if (anim.AnimationStartedTime == 0)
                {
                    e.ReplaceComponent(anim.StartNow(e.HasPauseAnimation() ? Game.NowUnescaled : Game.Now));
                }

                e.RemoveAnimationComplete();
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }
    }
}
