using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Helpers;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems.Graphics
{
    [Filter(typeof(SpriteComponent), typeof(RandomizeSpriteComponent))]
    [Watch(typeof(RandomizeSpriteComponent))]
    public class RandomizeAsepriteSystem : IReactiveSystem
    {
        public void OnActivated(World world, ImmutableArray<Entity> entities)
        {
            OnAdded(world, entities);
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                var sprite = e.GetSprite();
                var randomizer = e.GetRandomizeSprite();

                if (randomizer.RandomizeAnimation || randomizer.RandomRotate || randomizer.RandomFlip)
                {
                    e.SetSprite(
                        new SpriteComponent(
                            sprite.AnimationGuid,
                            sprite.Offset,
                            randomizer.RandomizeAnimation ? GetRandomAnimationId(sprite.AnimationGuid) : sprite.NextAnimations,
                            sprite.YSortOffset,
                            randomizer.RandomRotate ? true : sprite.RotateWithFacing,
                            sprite.HighlightStyle,
                            sprite.TargetSpriteBatch
                        ));
                }

                if (randomizer.RandomizeAnimationStart)
                {
                    e.SetAnimationStarted(Game.Random.NextFloat(1f, 32f));
                }

                if (randomizer.RandomRotate)
                {
                    e.SetFacing(DirectionHelper.RandomCardinal());
                }

                if (randomizer.RandomFlip && Game.Random.FlipACoin())
                {
                    e.SetFlipSprite(Core.Graphics.ImageFlip.Horizontal);
                }
            }
        }

        private ImmutableArray<string> GetRandomAnimationId(Guid animationGuid)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(animationGuid) is not SpriteAsset sprite)
            {
                return [];
            }

            string? animation = sprite.Animations.Remove("").TryGetRandomKey(Game.Random);
            if (animation is null)
            {
                return [];
            }

            return [animation];
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }
    }
}