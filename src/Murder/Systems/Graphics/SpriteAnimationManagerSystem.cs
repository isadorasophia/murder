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
                UpdateSprite(e);
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                UpdateSprite(e);
            }
        }
        private static void UpdateSprite(Entity e)
        {
            var anim = e.GetSprite();

            if (anim.AnimationStartedTime == null)
            {
                e.ReplaceComponent(anim.StartNow(e.HasPauseAnimation() ? Game.NowUnscaled : Game.Now));
            }

            e.RemoveAnimationComplete();
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }
    }
}