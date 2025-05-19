using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Murder.Services
{
    public static class HapticsServices
    {
        public static void StopVibration() => StopVibration(PlayerIndex.One);

        public static void Vibrate(float vibration, float balance, float durationSeconds, PlayerIndex player = PlayerIndex.One)
        {
            Game.Instance.Haptics.Add(new Core.Input.HapticsOrder()
            {
                StartVibration = vibration,
                MiddleVibration = vibration,
                EndVibration = 0,
                StartTime = Game.NowUnscaled,
                EndTime = Game.NowUnscaled + durationSeconds,
                BalanceStart = balance,
                BalanceEnd = balance,
            });
        }

        public static void VibrateInversePulse(float vibration,  float durationSeconds, PlayerIndex player = PlayerIndex.One)
        {
            Game.Instance.Haptics.Add(new Core.Input.HapticsOrder()
            {
                StartVibration = vibration,
                MiddleVibration = 0,
                EndVibration = vibration,
                StartTime = Game.NowUnscaled,
                EndTime = Game.NowUnscaled + durationSeconds,
                BalanceStart = 0,
                BalanceEnd = 1,
            });
        }

        public static void HitRight(float vibration, float duration, PlayerIndex player = PlayerIndex.One)
        {
            Game.Instance.Haptics.Add(new Core.Input.HapticsOrder()
            {
                StartVibration = vibration * 0.5f,
                MiddleVibration = vibration,
                EndVibration = 0,
                StartTime = Game.NowUnscaled,
                EndTime = Game.NowUnscaled + duration,
                BalanceStart = 1f,
                BalanceMiddle = 0.5f,
                BalanceEnd = 0.65f,

            });
        }

        public static void HitLeft(float vibration, float duration, PlayerIndex player = PlayerIndex.One)
        {
            Game.Instance.Haptics.Add(new Core.Input.HapticsOrder()
            {
                StartVibration = vibration * 0.5f,
                MiddleVibration = vibration,
                EndVibration = 0,
                StartTime = Game.NowUnscaled,
                EndTime = Game.NowUnscaled + duration,
                BalanceStart = 1f,
                BalanceMiddle = 0.5f,
                BalanceEnd = 0.4f,
            });
        }

        public static void StopVibration(PlayerIndex player)
        {
            Game.Instance.Haptics.ClearVibration(player);
        }
    }

}
