using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Bang.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.AllOf, typeof(TweenComponent))]
public class TweenSystem : IUpdateSystem
{
    public void Update(Context context)
    {
        foreach (var e in context.Entities)
        {
            TweenComponent tween = e.GetTween();
            float progress = Calculator.ClampTime(Game.Now - tween.Time.Start, tween.Time.Start- tween.Time.End, tween.Ease);
            
            if (progress>= 1)
            {
                ApplyTween(e, tween, 1);
            }
            else
            {
                ApplyTween(e, tween, progress);
            }
        }
    }

    private void ApplyTween(Entity e, TweenComponent tween, float progress)
    {
        if (tween.Position?.Get(progress) is Vector2 position)
        {
            e.SetPosition(position);
        }

        if (tween.Scale?.Get(progress) is Vector2 scale)
        {
            e.SetPosition(scale);
        }
    }
}
