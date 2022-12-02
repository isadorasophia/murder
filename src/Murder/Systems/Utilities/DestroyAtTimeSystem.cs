using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Systems
{
    [Filter(typeof(DestroyAtTimeComponent))]
    public class DestroyAtTimeSystem : IUpdateSystem
    {
        public ValueTask Update(Context context)
        {
            foreach (var e in context.Entities)
            {
                if (e.GetDestroyAtTime().TimeToDestroy < Game.Now)
                {
                    DestroyEntity(context.World, e);
                }
            }

            return default;
        }
        
        protected virtual void DestroyEntity(World world, Entity e)
        {
            e.Destroy();
        }
    }
}
