using Bang;
using Bang.StateMachines;
using Murder.Core.Sounds;

namespace Murder.Services
{
    public static class LevelServices
    {
        public static ValueTask SwitchScene(Guid nextWorldGuid)
        {
            // TODO: Do something fancier?
            Game.Instance.QueueWorldTransition(nextWorldGuid);

            return default;
        }

        public static ValueTask SwitchSceneAfterSeconds(World world, Guid nextWorldGuid, float seconds)
        {
            world.RunCoroutine(SwitchSceneOnSecondsCoroutine(nextWorldGuid, seconds));

            return default;
        }

        public static IEnumerator<Wait> SwitchSceneOnSecondsCoroutine(Guid nextWorldGuid, float seconds)
        {
            yield return Wait.ForSeconds(seconds);

            SoundServices.Stop(SoundLayer.Ambience, fadeOut: true);
            SoundServices.Stop(SoundLayer.Sfx, fadeOut: true);

            SwitchScene(nextWorldGuid);
        }
    }
}