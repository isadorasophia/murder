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

                if (squish.EaseIn is EaseKind easeIn)
                {
                    EaseKind easeOut = squish.EaseOut ?? EaseKind.Linear;
                    var ease = time > 0.5f ? easeOut : easeIn;
                    float rate = 1 - MathF.Abs(1 - time * 2);
                    float easedRate = Ease.Evaluate(rate, ease);
                    e.SetScale(Vector2Helper.Squish(easedRate * squish.Amount));
                }
                else if (squish.EaseOut is EaseKind easeOut)
                {
                    float rate = 1 - time;
                    float easedRate = Ease.Evaluate(rate, easeOut);
                    e.SetScale(Vector2Helper.Squish(easedRate * squish.Amount));

                }


                if (time >= 1)
                {
                    e.RemoveSquish();
                    e.RemoveScale();
                }
            }
        }
    }
}
