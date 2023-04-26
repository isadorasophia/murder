using Murder.Core.Geometry;

namespace Murder.Core.Graphics
{
    /// <summary>
    /// Generic struct for drawing things without cluttering methods full of arguments.
    /// Note that not all fields are supported by all methods.
    /// Tip: Create a new one like this: <code>new DrawInfo(){ Color = Color.Red, Sort = 0.2f}</code>
    /// </summary> 
    public readonly struct DrawInfo
    {
        public enum BlendStyle
        {
            Normal,
            Wash,
            Color
        }
        public static DrawInfo Ui => new() { UseScaledTime = false };

        public static DrawInfo Default => new();

        /// <summary>
        /// The origin of the image. From 0 to 1. Vector2.Center is the center.
        /// </summary>
        public Vector2 Origin { get; init; } = Vector2.Zero;
        
        /// <summary>
        /// An offset to draw this image. In pixels
        /// </summary>
        public Vector2 Offset { get; init; } = Vector2.Zero;

        /// <summary>
        /// If this is an animation, will it use scaled time?
        /// </summary>
        public bool UseScaledTime { get; init; } = false;
        
        /// <summary>
        /// In degrees.
        /// </summary>
        public float Rotation { get; init; } = 0;
        public Vector2 Scale { get; init; } = Vector2.One;
        public Color Color { get; init; } = Color.White;
        public float Sort { get; init; } = 0.5f;

        public Color? Outline { get; init; } = null;

        public BlendStyle BlendMode { get; init; } = BlendStyle.Normal;
        public bool FlippedHorizontal { get; init; } = false;

        public DrawInfo()
        {
        }
        public DrawInfo(float sort)
        {
            Sort = sort;
        }

        public Microsoft.Xna.Framework.Vector3 GetBlendMode()
        {
            switch (BlendMode)
            {
                case BlendStyle.Normal: return new (1, 0, 0);
                case BlendStyle.Wash: return new (0, 1, 0);
                case BlendStyle.Color: return new (0, 0, 1);
                default:
                    throw new Exception("Blend mode not supported!");
            }
        }

        internal DrawInfo WithScale(Vector2 size)
        {
            return new DrawInfo()
            {
                Origin = Origin,
                UseScaledTime = UseScaledTime,
                Rotation = Rotation,
                Color = Color,
                Sort = Sort,
                Scale = Scale * size,
                Offset = Offset
            };
        }

        internal DrawInfo WithOffset(Vector2 offset)
        {
            return new DrawInfo()
            {
                UseScaledTime = UseScaledTime,
                Rotation = Rotation,
                Color = Color,
                Sort = Sort,
                Scale = Scale,
                Origin = Origin,
                Offset = offset
            };
        }
        public DrawInfo WithSort(float sort) => new DrawInfo()
        {
            UseScaledTime = UseScaledTime,
            Rotation = Rotation,
            Color = Color,
            Sort = sort,
            Scale = Scale,
            Origin = Origin,
            Offset = Offset
        };

    }

}
