using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems
{
    [Filter(typeof(DestroyAtTimeComponent))]
    public class DestroyAtTimeSystem : IUpdateSystem
    {
        public void Update(Context context)
        {
            foreach (var e in context.Entities)
            {
                var component = e.GetDestroyAtTime();
                if (component.TimeToDestroy < Game.Now)
                {
                    switch (component.Style)
                    {
                        case RemoveStyle.Destroy:
                            e.Destroy();
                            break;
                        case RemoveStyle.Deactivate:
                            e.Deactivate();
                            break;
                            
                        case RemoveStyle.None:
                        default:
                            break;
                    }
                }
            }
        }
        
        protected virtual void DestroyEntity(World world, Entity e)
        {
            e.Destroy();
        }
    }
}
