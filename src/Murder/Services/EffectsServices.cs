using Bang;
using Bang.Contexts;
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
        public static void FadeIn(World world, float time, Color color, bool destroyAfterFinished = true)
        {
            var e = world.AddEntity();
            e.SetFadeScreen(new(FadeType.In, Game.NowUnescaled, time, color, destroyAfterFinished));
        }

        /// <summary>
        /// Add an entity which will apply a "fade-out" effect. Clearing the screeen.
        /// </summary>
        public static void FadeOut(World world, float time, Color color, float delay = 0, bool destroyAfterFinished = true)
        {
            var e = world.AddEntity();
            e.SetFadeScreen(new(FadeType.Out, Game.NowUnescaled + delay, time, color, destroyAfterFinished));
        }

        public static void ApplyHighlight(World world, Entity e, HighlightSpriteComponent highlight)
        {
            if (e.HasHighlightOnChildren())
            {
                foreach (int childId in e.Children)
                {
                    world.TryGetEntity(childId)?.SetHighlightSprite(highlight);
                }
            }
            else
            {
                e.SetHighlightSprite(highlight);
                e.TryFetchParent()?.SetHighlightSprite(highlight);
            }
        }

        public static void RemoveHighlight(Entity e)
        {
            if (e.HasHighlightOnChildren())
            {
                foreach (int childId in e.Children)
                {
                    e.TryFetchChild(childId)?.RemoveHighlightSprite();
                }
            }
            else
            {
                e.RemoveHighlightSprite();
                e.TryFetchParent()?.RemoveHighlightSprite();
            }
        }
    }
}
