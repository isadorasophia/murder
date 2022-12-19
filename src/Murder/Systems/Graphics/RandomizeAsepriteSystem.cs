using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Systems.Graphics
{
    public class RandomizeAsepriteSystem : IReactiveSystem
    {
        public ValueTask OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                var ase = e.GetAseprite();
                var randomizer = e.GetRandomizeAseprite();
                e.SetAseprite(
                    new AsepriteComponent(
                        ase.AnimationGuid,
                        ase.Offset,
                        ase.NextAnimations,
                        ase.YSortOffset,
                        ase.RotateWithFacing,
                        ase.FlipWithFacing,
                        randomizer.RandomizeAnimation? Game.Random.Next(32) : ase.AnimationStartedTime, 
                        ase.TargetSpriteBatch 
                    ));
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
