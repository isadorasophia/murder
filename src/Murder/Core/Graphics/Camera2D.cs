using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Numerics;
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
        private float _bump = 0;
        private float _bumpStart = 0;

        private Vector2 _position = Vector2.Zero;

        /// <summary>
        /// The rotation of the camera in degrees.
        /// </summary>
        private float _rotation = 0;
        private float _zoom = 1;

        public float ShakeIntensity = 0f;
        public float ShakeTime = 0f;

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

        public void ClearCache()
        {
            _cachedWorldViewProjection = null;
        }
        /// <summary>
        /// Get coordinates of the cursor in the world.
        /// Ideally you should use the EditorHook for this if you are in an editor System.
        /// </summary>
        public Point GetCursorWorldPosition(Point screenOffset, Point viewportSize)
        {
            Vector2 scale = new Vector2(Width, Height) / viewportSize;
            return ScreenToWorldPosition((Game.Input.CursorPosition - screenOffset) * scale).Point();
        }

        /// <summary>
        /// Get coordinates of the cursor in the world.
        /// </summary>
        public Vector2 ConvertWorldToScreenPosition(Vector2 position, Point viewportSize)
        {
            Vector2 scale = new Vector2(Width * 1f / viewportSize.X, Height * 1f / viewportSize.Y);
            return WorldToScreenPosition(position * scale);
        }

        public float Zoom
        {
            get => _zoom;
            set
            {
                float zoom = Math.Clamp(value, 0.1f, 500f);

                if (zoom != _zoom)
                {
                    _zoom = zoom;
                    _cachedWorldViewProjection = null;
                }
            }
        }

        /// <summary>
        /// The top left position of the camera in world space.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                // [PERF] This could be cached
                float bump = 0;
                if (_bumpStart > 0)
                {
                    float bumpDelta = Calculator.ClampTime(Game.NowUnscaled - _bumpStart, 0.3f);
                    bump = Ease.BackIn(1 - bumpDelta);
                    _cachedWorldViewProjection = null;
                }

                if (ShakeTime > 0)
                {
                    float shakeX = (float)Math.Sin((Game.NowAbsolute * 200f) * 31) * 2 - 1; // The numbers here can be adjusted for different shake patterns
                    float shakeY = (float)Math.Sin((Game.NowAbsolute * 200f) * 17) * 2 - 1; // These should ideally be irrational numbers
                    _cachedWorldViewProjection = null;

                    return _position + new Vector2(shakeX, shakeY) * ShakeIntensity + new Vector2(0, bump * _bump);
                }

                return _position + new Vector2(0, bump * _bump);
            }
            set
            {
                GameLogger.Verify(!_locked, "You shouldn't move the camera during a render call");

                // No operation if position is the same.
                if (_position == value) return;

                _position = value;
                _cachedWorldViewProjection = null;
            }
        }

        public Vector2 Center => Position + new Vector2(Size.X / 2, Size.Y / 2);
        public int HalfWidth => Calculator.RoundToInt(Width / 2f);

        public Point Size => new Point(Width, Height);

        public float Aspect => (float)Width / Height;

        public Camera2D(int width, int height)
        {
            (Width, Height) = (width, height);

            // Origin will be the center of the camera.
            _origin = new Vector2(0.5f, 0.5f);
        }
        public void Shake(float intensity, float time)
        {
            ShakeIntensity = intensity;
            ShakeTime = time;
        }

        public void Bump(float intensity = 1)
        {
            _bump = intensity;
            _bumpStart = Game.NowUnscaled;
        }

        public Vector2 ScreenToWorldPosition(Vector2 screenPosition)
        {
            return Microsoft.Xna.Framework.Vector2.Transform(
                screenPosition.ToXnaVector2(),
                Matrix.Invert(WorldViewProjection)).ToSysVector2();
        }

        public Vector2 WorldToScreenPosition(Vector2 screenPosition)
        {
            return Microsoft.Xna.Framework.Vector2.Transform(
                screenPosition.ToXnaVector2(),
                WorldViewProjection).ToSysVector2();
        }

        internal void UpdateSize(Point size)
        {
            Width = Math.Max(1, size.X);
            Height = Math.Max(1, size.Y);

            _cachedWorldViewProjection = null;
        }

        public void Rotate(float degrees)
        {
            _rotation = degrees;

            _cachedWorldViewProjection = null;
        }

        private Matrix GetWorldView()
        {
            Point position = Position.Round();
            Point center = (_origin * new Vector2(Width, Height)).Point();

            // First, let's start with our initial position.
            Matrix view = Matrix.CreateTranslation(
                xPosition: -position.X,
                yPosition: -position.Y,
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
            Microsoft.Xna.Framework.Vector2 topLeftCorner = Microsoft.Xna.Framework.Vector2.Transform(Microsoft.Xna.Framework.Vector2.Zero, inverseMatrix);
            // var topRightCorner = Vector2.Transform(new Vector2(Width, 0), inverseMatrix);
            // var bottomLeftCorner = Vector2.Transform(new Vector2(0, Height), inverseMatrix);
            Microsoft.Xna.Framework.Vector2 bottomRightCorner = Microsoft.Xna.Framework.Vector2.Transform(new Microsoft.Xna.Framework.Vector2(Width, Height), inverseMatrix);

            Bounds = new Rectangle(topLeftCorner.ToSysVector2(), (bottomRightCorner - topLeftCorner).ToSysVector2());
            SafeBounds = Bounds.Expand(Grid.CellSize * 2);
            return view;
        }

        public void Lock()
        {
            _locked = true;
        }

        public void Unlock()
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