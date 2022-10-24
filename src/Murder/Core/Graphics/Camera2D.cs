using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Utilities;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace Murder.Core.Graphics
{
    /// <summary>
    /// Creates a camera 2D world view for our game.
    /// </summary>
    public class Camera2D
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        // public float Scale { get; private set; }
        public Rectangle Bounds { get; private set; }
        public Rectangle SafeBounds { get; private set; }

        private readonly Vector2 _origin = Vector2.Zero;

        private Vector2 _position = Vector2.Zero;

        /// <summary>
        /// The rotation of the camera in degrees.
        /// </summary>
        private float _rotation = 0;
        private float _zoom = 1;

        private float RotationRad => _rotation * MathF.PI / 180;

        private Matrix? _cachedWorldViewProjection;

        private bool _locked;
        public Matrix WorldViewProjection
        {
            get
            {
                _cachedWorldViewProjection ??= GetWorldView();

                return _cachedWorldViewProjection.Value;
            }
        }

        /// <summary>
        /// Get coordinates of the cursor in the world.
        /// </summary>
        public Point GetCursorWorldPosition(Point screenOffset, Point viewportSize)
        {
            var scale = new Vector2(Width, Height) / viewportSize;
            return ScreenToWorldPosition((Game.Input.CursorPosition - screenOffset) * scale);
        }

        public float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = Math.Clamp(value, 0.1f, 500f);
                _cachedWorldViewProjection = null;
            }
        }

        public Vector2 Position
        {
            get => _position;
            set
            {
                GameLogger.Verify(!_locked, "You shouldn't move the camera during a render call");

                // No operation if position is the same.
                if (_position == value) return;

                _position = value;
                _cachedWorldViewProjection = null;
            }
        }

        public int HalfWidth => Calculator.RoundToInt(Width/2f);

        public Point Size => new Point(Width, Height);

        public float Aspect => (float)Width / Height;

        public Camera2D(int width, int height, int scale)
        {
            (Width, Height) = (width, height);

            //Scale = scale;

            // Origin will be the center of the camera.
            _origin = new Vector2(0.5f, 0.5f);
        }

        public bool IsInCamera(Rectangle rectangle)
        {
            return SafeBounds.Touches(rectangle);
        }
        public bool IsInCamera(Point rectangle)
        {
            return SafeBounds.Contains(rectangle);
        }

        public Vector2 ScreenToWorldPosition(Vector2 screenPosition)
        {
            return Microsoft.Xna.Framework.Vector2.Transform(screenPosition, Matrix.Invert(WorldViewProjection));
        }
        public Vector2 WorldToScreenPosition(Vector2 screenPosition)
        {
            return Microsoft.Xna.Framework.Vector2.Transform(screenPosition, WorldViewProjection);
        }

        internal void UpdateSize(int width, int height)
        {
            Width = Math.Max(1, width);
            Height = Math.Max(1, height);

            _cachedWorldViewProjection = null;
        }

        public void Rotate(float degrees)
        {
            _rotation = degrees;

            _cachedWorldViewProjection = null;
        }

        private Matrix GetWorldView()
        {
            var position = _position.Round();
            var center = (_origin * new Vector2(Width, Height));

            // First, let's start with our initial position.
            Matrix view = Matrix.CreateTranslation(
                xPosition: - position.X,
                yPosition: - position.Y,
                zPosition: 0);

            // Now, overcompensate the origin by changing our relative position.
            // This will make sure we are ready for any rotation and scale operations
            // with the correct relative position.
            view *= Matrix.CreateTranslation(
                xPosition: -center.X,
                yPosition: -center.Y,
                zPosition: 0);

            // Now, we will apply the scale operation.
            view *= Matrix.CreateRotationZ(RotationRad);

            // And our zoom!
            view *= Matrix.CreateScale(_zoom, _zoom, 1);

            // Okay, we are done. Now go back to our correct position.
            view *= Matrix.CreateTranslation(
                xPosition: center.X,
                yPosition: center.Y,
                zPosition: 0);

            var inverseMatrix = Matrix.Invert(view);
            var topLeftCorner =  Microsoft.Xna.Framework.Vector2.Transform(new Vector2(0, 0), inverseMatrix);
            // var topRightCorner = Vector2.Transform(new Vector2(Width, 0), inverseMatrix);
            // var bottomLeftCorner = Vector2.Transform(new Vector2(0, Height), inverseMatrix);
            var bottomRightCorner =  Microsoft.Xna.Framework.Vector2.Transform(new Vector2(Width, Height), inverseMatrix);

            Bounds = new Rectangle(topLeftCorner.ToPoint(),(bottomRightCorner - topLeftCorner).ToPoint());
            SafeBounds = Bounds.Expand(Grid.CellSize * 2);
            return view;
        }

        internal void Lock()
        {
            _locked = true;
        }

        internal void Unlock()
        {
            _locked = false;
        }

        internal void Reset()
        {
            Unlock();
            Position = Vector2.Zero;
            Zoom = 1;
        }
    }
}
