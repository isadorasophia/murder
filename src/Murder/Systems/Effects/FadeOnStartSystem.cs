using Bang;
using Bang.Contexts;
using Bang.StateMachines;
using Bang.Systems;
using Murder.Services;

namespace Murder.Systems
{

    internal class FadeOnStartSystem : IStartupSystem
    {
        public ValueTask Start(Context context)
        {
            EffectsServices.FadeOut(context.World, 3, 100f);
            context.World.RunCoroutine(FadeOutAfterTime(context.World, 0.1f));

            return default;
        }

        private IEnumerator<Wait> FadeOutAfterTime(World world, float v)
        {
            yield return Wait.ForSeconds(0.5f);
            EffectsServices.FadeOut(world, 4);
        }
    }
}
