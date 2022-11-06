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
    internal class DestroyAtTimeSystem : IUpdateSystem
    {
        public ValueTask Update(Context context)
        {
            foreach (var e in context.Entities)
            {
                if (e.GetDestroyAtTime().TimeToDestroy < Game.Now)
                {
                    e.Destroy();
                }
            }

            return default;
        }
    }
}
