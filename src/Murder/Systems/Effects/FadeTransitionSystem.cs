using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using System.Collections.Immutable;
using Murder.Components;
using Murder.Utilities;
using Bang;

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
                    if (e.TryGetComponent<AlphaComponent>() is AlphaComponent alpha)
                    {
                        e.SetFadeTransition(fadeout.Duration, alpha.Get(AlphaSources.Fade), fadeout.TargetAlpha, fadeout.DestroyEntityOnEnd);
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
                FadeTransitionComponent fade = e.GetFadeTransition();
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

                e.SetAlpha(AlphaSources.Fade, alpha);
            }
        }
    }
}
