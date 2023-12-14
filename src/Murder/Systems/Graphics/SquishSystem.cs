using Bang.Contexts;
using Murder.Core.Graphics;
using Bang.Entities;
using Bang.Systems;
using Bang.Components;
using Murder.Components;
using Murder.Utilities;
using Murder.Diagnostics;

namespace Murder.Systems
{
    [Filter(ContextAccessorFilter.AllOf, typeof(ITransformComponent), typeof(SquishComponent))]
    internal class SquishSystem : IMonoPreRenderSystem
    {
        public void BeforeDraw(Context context)
        {
            foreach (var e in context.Entities)
            {
                var squish = e.GetSquish();
                float time = Calculator.ClampTime((squish.ScaledTime ? Game.Now : Game.NowUnscaled) - squish.Start, squish.Duration);
                
                var ease = time>0.5f? squish.EaseOut : squish.EaseIn;
                float rate = 1 - MathF.Abs(1 - time * 2);
                float easedRate = Ease.Evaluate(rate, ease);
                e.SetScale(Vector2Helper.Squish(easedRate * squish.Amount));

                if (time >= 1)
                {
                    e.RemoveSquish();
                    e.RemoveScale();
                }
            }
        }
    }
}
