using Murder.Core.Sounds;
using Murder.Diagnostics;

namespace Murder.Services
{
    public static class SoundServices
    {
        public static async ValueTask Play(SoundEventId id, SoundProperties properties = SoundProperties.None)
        {
            if (!id.IsGuidEmpty)
            {
                await Game.Sound.PlayEvent(id, properties);
            }
        }

        public static async ValueTask PlayMusic(SoundEventId id)
        {
            await Game.Sound.PlayEvent(id, SoundProperties.Persist);
        }

        public static float GetGlobalParameter(ParameterId id)
        {
            return Game.Sound.GetGlobalParameter(id);
        }

        public static void SetGlobalParameter<T>(ParameterId id, T value)
        {
            try
            {
                Game.Sound.SetGlobalParameter(id, Convert.ToSingle(value));
            }
            catch (Exception e) when (e is FormatException || e is OverflowException)
            {
                GameLogger.Warning($"{value} is not a valid float.");
            }
        }

        public static void Stop(SoundEventId? id, bool fadeOut)
        {
            Game.Sound.Stop(id, fadeOut, out _);
        }

        /// <summary>
        /// Stop all the ongoing events.
        /// </summary>
        /// <param name="fadeOut">Whether it should fade out in fmod.</param>
        /// <returns>List of all the events which were stopped.</returns>
        public static SoundEventId[] StopAll(bool fadeOut)
        {
            Game.Sound.Stop(fadeOut, out SoundEventId[] stoppedEvents);

            return stoppedEvents;
        }
    }
}