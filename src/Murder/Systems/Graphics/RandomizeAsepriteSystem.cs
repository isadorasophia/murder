using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using System.Collections.Immutable;
using Murder.Utilities;
using Murder.Helpers;

namespace Murder.Systems.Graphics
{
    [Filter(typeof(AsepriteComponent), typeof(RandomizeAsepriteComponent))]
    [Watch(typeof(RandomizeAsepriteComponent))]
    public class RandomizeAsepriteSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                var ase = e.GetAseprite();
                var randomizer = e.GetRandomizeAseprite();
                e.SetAseprite(
                    new AsepriteComponent(
                        ase.AnimationGuid,
                        ase.Offset,
                        randomizer.RandomizeAnimation? GetRandomAnimationId(ase.AnimationGuid) : ase.NextAnimations,
                        ase.YSortOffset,
                        randomizer.RandomRotate? true : ase.RotateWithFacing,
                        ase.FlipWithFacing,
                        randomizer.RandomizeAnimationStart ? Game.Random.Next(1, 32) : ase.AnimationStartedTime, 
                        ase.TargetSpriteBatch 
                    ));

                if (randomizer.RandomRotate)
                {
                    e.SetFacing(DirectionHelper.RandomCardinal());
                }
            }
        }

        private ImmutableArray<string> GetRandomAnimationId(Guid animationGuid)
        {
            var ase = Game.Data.GetAsset<SpriteAsset>(animationGuid);
            var animation = ase.Animations.Remove("").GetRandomKey(Game.Random);

            return ImmutableArray.Create(animation);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }
    }
}
