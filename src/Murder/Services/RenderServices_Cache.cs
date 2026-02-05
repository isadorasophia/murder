using Bang.Entities;
using Murder.Components.Graphics;

namespace Murder.Services;

public static partial class RenderServices
{
    /// <summary>
    /// Perf: This is clumsy because we're doing for perf to avoid setting the sprite cache
    /// every single frame.
    /// We use a single reference for this component instead.
    /// </summary>
    public static void UpdateRenderedSpriteCache(Entity e, RenderedSpriteCache cache)
    {
        if (e.TryGetRenderedSpriteCache() is RenderedSpriteCacheComponent component)
        {
            component.Ref.Cache = cache;
        }
        else
        {
            e.SetRenderedSpriteCache(cache);
        }
    }
}
