namespace Murder.Core.Input
{
    /// <summary>
    /// Base class for input button constants, numbers from 100 to 120 are reserved for the engine.
    /// We recomend that if you need to create new constants for more gameplay buttons, start at 0.
    /// </summary>
    public class MurderInputButtons
    {
        public const int Debug = 100;
        public const int PlayGame = 101;
        public const int LeftClick = 102;
        public const int RightClick = 103;
        public const int MiddleClick = 104;

        // Navigation
        public const int Submit = 105;
        public const int Cancel = 106;
        public const int Pause = 107;

        public const int Shift = 108;
        public const int Esc = 109;
        public const int Delete = 110;
        public const int Ctrl = 111;
        public const int Space = 112;
        public const int Backspace = 112;
    }
}