using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems
{
    /// <summary>
    /// System responsible for fading in and out entities.
    /// This is not responsible for the screen fade transition.
    /// </summary>
    [Filter(typeof(FadeTransitionComponent))]
    [Watch(typeof(FadeTransitionComponent))]
    public class FadeTransitionSystem : IUpdateSystem, IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                var fadeout = e.GetFadeTransition();
                if (fadeout.StartTime == 0)
                {
                    if (e.TryGetAlpha() is AlphaComponent alpha)
                    {
                        e.SetFadeTransition(fadeout.Duration, alpha.Alpha, fadeout.TargetAlpha, fadeout.DestroyEntityOnEnd);
                    }
                    else
                    {
                        e.SetFadeTransition(fadeout.Duration, 1f, fadeout.TargetAlpha, fadeout.DestroyEntityOnEnd);
                    }
                }
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities) { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities) { }

        public void Update(Context context)
        {
            foreach (var e in context.Entities)
            {
                // TODO: Removed in the same frame?
                if (e.TryGetFadeTransition() is not FadeTransitionComponent fade)
                {
                    continue;
                }

                if (fade.Duration + fade.StartTime < Game.NowUnscaled)
                {
                    if (fade.DestroyEntityOnEnd)
                    {
                        e.Destroy();
                    }
                    else
                    {
                        e.RemoveFadeTransition();
                    }
                }

                float percentage = Calculator.Clamp01((Game.NowUnscaled - fade.StartTime) / fade.Duration);
                float alpha = Calculator.Lerp(fade.StartAlpha, fade.TargetAlpha, Ease.CubeOut(percentage));

                e.SetAlpha(alpha);
            }
        }
    }
}