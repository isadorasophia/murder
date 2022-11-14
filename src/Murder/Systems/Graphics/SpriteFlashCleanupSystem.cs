using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems.Graphics
{
    [Filter(typeof(FlashSpriteComponent))]
    internal class SpriteFlashCleanupSystem : IUpdateSystem
    {
        public ValueTask Update(Context context)
        {
            foreach (var e in context.Entities)
            {
                var flash = e.GetFlashSprite();
                if (Game.Now> flash.DestroyAtTime)
                {
                    e.RemoveFlashSprite();
                }
            }
            return default;
        }
    }
}
