using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems;

[Filter(typeof(IgnoreUntilComponent))]
public class IgnoreUntilSystem : IUpdateSystem
{
    public void Update(Context context)
    {
        foreach (var e in context.Entities)
        {
            var time = e.GetIgnoreUntil().Until;
            if (time <= Game.Now && time >= 0)
                e.RemoveIgnoreUntil();
        }
    }
}
