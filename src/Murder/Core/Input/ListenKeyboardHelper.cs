namespace Murder.Core.Input
{
    public class ListenKeyboardHelper : IDisposable
    {
        public ListenKeyboardHelper(int maxCharacters = 32)
        {
            Game.Input.ListenToKeyboardInput(enable: true, maxCharacters);
        }

        public void Dispose()
        {
            Game.Input.ListenToKeyboardInput(enable: false);
        }

        public static void Clamp(int length)
        {
            Game.Input.ClampText(length);
        }
    }
}