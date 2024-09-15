using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components.Utilities;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.AllOf, typeof(ActivateOnStartComponent))]
public class ActivateOnStartSystem : IStartupSystem
{
    public void Start(Context context)
    {
        foreach (var e in context.Entities)
        {
            var activate = e.GetActivateOnStart();
            if (activate.DeactivateInstead)
            {
                e.Deactivate();
            }
            else
            {
                e.Activate();
            }
        }
    }
}
