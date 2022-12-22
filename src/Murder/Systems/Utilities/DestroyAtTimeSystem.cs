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
                if (e.GetDestroyAtTime().TimeToDestroy < Game.Now)
                {
                    DestroyEntity(context.World, e);
                }
            }
        }
        
        protected virtual void DestroyEntity(World world, Entity e)
        {
            e.Destroy();
        }
    }
}
