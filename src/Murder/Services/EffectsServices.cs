using Bang;
using Bang.Entities;
using Murder.Components;

namespace Murder.Services
{
    internal static class EffectsServices
    {
        /// <summary>
        /// Add an entity which will apply a "fade-in" effect. Darkening the screen to black.
        /// </summary>
        public static void FadeIn(World world, float time)
        {
            if (world.TryGetUniqueEntity<FadeScreenComponent>() is not Entity e)
            {
                e = world.AddEntity();
            }
            
            e.SetFadeScreen(new(FadeType.In, Game.NowUnescaled, time));
        }

        /// <summary>
        /// Add an entity which will apply a "fade-out" effect. Clearing the screeen.
        /// </summary>
        public static void FadeOut(World world, float time, float delay = 0)
        {
            if (world.TryGetUniqueEntity<FadeScreenComponent>() is not Entity e)
            {
                e = world.AddEntity();
            }

            e.SetFadeScreen(new(FadeType.Out, Game.NowUnescaled + delay, time));
        }
    }
}
