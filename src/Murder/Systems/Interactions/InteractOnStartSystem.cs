using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Utilities;

namespace Murder.Systems;

[Filter(typeof(InteractOnStartComponent))]
internal class InteractOnStartSystem : IStartupSystem
{
    public void Start(Context context)
    {
        foreach (Entity e in context.Entities)
        {
            InteractOnStartComponent onStart = e.GetInteractOnStart();
            if (onStart.Conditions is not null && 
                !BlackboardHelpers.IsSatisfied(context.World, onStart.Conditions.Value))
            {
                continue;
            }

            e.SendInteractMessage();

            if (onStart.Flags.HasFlag(InteractOnStartFlags.OnlyOnce))
            {
                e.Destroy();
            }
        }
    }
}
