using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using System.Collections.Immutable;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(SpriteComponent), typeof(AnimationCompleteComponent))]
    internal class SpriteCleanupSystem : IUpdateSystem
    {
        public void Update(Context context)
        {
            foreach (var e in context.Entities)
            {
                e.RemoveAnimationComplete();
            }
        }
    }
}