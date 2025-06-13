using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems;

[Filter(typeof(InteractOnStartComponent))]
internal class InteractOnStartSystem : IStartupSystem
{
    public void Start(Context context)
    {
        foreach (Entity e in context.Entities)
        {
            e.SendInteractMessage();
        }
    }
}
