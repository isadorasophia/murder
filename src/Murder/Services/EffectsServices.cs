using Bang;
using Bang.Entities;
using Murder.Components;
using Murder.Core.Graphics;

namespace Murder.Services
{
    public static class EffectsServices
    {
        /// <summary>
        /// Add an entity which will apply a "fade-in" effect. Darkening the screen to black.
        /// </summary>
        public static void FadeIn(World world, float time, Color color)
        {
            if (world.TryGetUniqueEntity<FadeScreenComponent>() is not Entity e)
            {
                e = world.AddEntity();
            }
            
            e.SetFadeScreen(new(FadeType.In, Game.NowUnescaled, time, color));
        }

        /// <summary>
        /// Add an entity which will apply a "fade-out" effect. Clearing the screeen.
        /// </summary>
        public static void FadeOut(World world, float time, Color color, float delay = 0)
        {
            if (world.TryGetUniqueEntity<FadeScreenComponent>() is not Entity e)
            {
                e = world.AddEntity();
            }

            e.SetFadeScreen(new(FadeType.Out, Game.NowUnescaled + delay, time, color));
        }
    }
}
