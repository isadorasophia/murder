namespace Murder.Core.Geometry
{
    public struct Padding
    {
        public int Top = 0;
        public int Left = 0;
        public int Right = 0;
        public int Bottom = 0;

        public Padding(int border) : this(border, border, border, border) {}

        public Padding(int top, int left, int right, int bottom)
        {
            Top = top;
            Left = left;
            Right = right;
            Bottom = bottom;
        }

        public int Height => Top + Bottom;
        public int Width => Left + Right;

        static public Rectangle operator +(Rectangle rectangle, Padding rect) => rect + rectangle;
        static public Rectangle operator -(Rectangle rectangle, Padding rect) => rect - rectangle;
        static public Rectangle operator +(Padding rect, Rectangle rectangle) => new Rectangle(
            rectangle.X + rect.Left,
            rectangle.Y + rect.Top,
            rectangle.Width - rect.Left - rect.Right,
            rectangle.Height - rect.Top - rect.Bottom);
        static public Rectangle operator -(Padding rect, Rectangle rectangle) => new Rectangle(
            rectangle.X - rect.Left,
            rectangle.Y - rect.Top,
            rectangle.Width + rect.Left + rect.Right,
            rectangle.Height + rect.Top + rect.Bottom);
    }
}