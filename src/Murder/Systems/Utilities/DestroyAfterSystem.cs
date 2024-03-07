using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Utilities;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.AllOf, typeof(DestroyAfterSecondsComponent), typeof(CreatedAtComponent))]
internal class DestroyAfterSystem : IFixedUpdateSystem
{
    public void FixedUpdate(Context context)
    {
        foreach (var e in context.Entities)
        {
            if (e.GetCreatedAt().When + e.GetDestroyAfterSeconds().Lifespan < Game.Now)
            {
                e.Destroy();
            }
        }
    }
}
