using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Utilities;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.AllOf, typeof(ActivateOnStartComponent))]
public class ActivateOnStartSystem : IStartupSystem
{
    public void Start(Context context)
    {
        foreach (Entity e in context.Entities)
        {
            ActivateOnStartComponent activate = e.GetActivateOnStart();
            if (activate.DeactivateInstead)
            {
                e.Deactivate();
            }
            else
            {
                e.Activate();
            }

            CleanupRuleMatchEntity(e, activate);
        }
    }

    private void CleanupRuleMatchEntity(Entity e, ActivateOnStartComponent activate)
    {
        switch (activate.After)
        {
            case AfterInteractRule.InteractOnlyOnce:
            case AfterInteractRule.RemoveComponent:
                e.RemoveActivateOnStart();
                break;

            case AfterInteractRule.Destroy:
                e.Destroy();
                break;
        }
    }
}
