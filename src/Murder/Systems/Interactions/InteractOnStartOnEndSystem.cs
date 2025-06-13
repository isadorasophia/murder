using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems;

[Filter(typeof(InteractOnStartOnEndComponent))]
internal class InteractOnStartOnEndSystem : IStartupSystem, IExitSystem
{
    public void Start(Context context)
    {
        foreach (Entity e in context.Entities)
        {
            e.SendInteractMessage();
        }
    }

    public void Exit(Context context)
    {
        foreach (Entity e in context.Entities)
        {
            e.SendOnExitMessage();
            e.SendInteractMessage();
        }
    }
}
