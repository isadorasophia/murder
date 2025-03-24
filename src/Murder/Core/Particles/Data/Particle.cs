using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Core.Particles;

public readonly struct Particle
{
    [SpriteBatchReference]
    public readonly int SpriteBatch;

    public readonly ParticleTexture Texture = new();

    [Tooltip("This is how long this particle lives.")]
    public readonly ParticleValueProperty LifeTime = ParticleValueProperty.Empty;

    [Tooltip("Constant velocity applied to the particles.")]
    public readonly ParticleVectorValueProperty Gravity = ParticleVectorValueProperty.Empty;

    [Tooltip("Whether this particle has a fixed position or it will follow the entity.")]
    public readonly bool FollowEntityPosition = false;

    // TODO: Color gradient?
    // public readonly Color[] Colors;
    [PaletteColor]
    public readonly ImmutableArray<Color> Colors = new Color[] { Color.White }.ToImmutableArray();

    // TODO: Make this value random too?
    public readonly ImmutableArray<Vector2> Scale = new Vector2[] { Vector2.One }.ToImmutableArray();

    [Slider(0, 1)]
    public readonly ParticleValueProperty Alpha = ParticleValueProperty.Empty;

    public readonly ParticleValueProperty Acceleration = ParticleValueProperty.Empty;

    [Slider(0, 1)]
    public readonly ParticleValueProperty Friction = ParticleValueProperty.Empty;

    [Tooltip("Start velocity, might be added on top of the emitter speed.")]
    public readonly ParticleValueProperty StartVelocity = ParticleValueProperty.Empty;

    public readonly ParticleValueProperty RotationSpeed = ParticleValueProperty.Empty;

    [Tooltip("Rotation when the particle is instantiated.")]
    [Angle]
    public readonly ParticleValueProperty Rotation = ParticleValueProperty.Empty;

    public readonly bool RotateWithVelocity = false;
    public readonly float SortOffset;

    public readonly MurderBlendState BlendState = MurderBlendState.AlphaBlend;

    public Particle() { }

    public Particle(
        ParticleTexture texture,
        ImmutableArray<Color> colors,
        ImmutableArray<Vector2> scale,
        ParticleValueProperty alpha,
        ParticleValueProperty acceleration,
        ParticleValueProperty friction,
        ParticleValueProperty startVelocity,
        ParticleValueProperty rotationSpeed,
        ParticleValueProperty rotation,
        ParticleValueProperty lifeTime,
        bool rotateWithVelocity,
        float sortOffset)
    {
        Texture = texture;
        Colors = colors;
        Scale = scale;
        Alpha = alpha;
        Acceleration = acceleration;
        Friction = friction;
        StartVelocity = startVelocity;
        RotationSpeed = rotationSpeed;
        Rotation = rotation;
        LifeTime = lifeTime;
        RotateWithVelocity = rotateWithVelocity;
        SortOffset = sortOffset;
    }

    public Particle WithTexture(ParticleTexture texture) =>
        new Particle(texture, Colors, Scale, Alpha, Acceleration, Friction, StartVelocity, RotationSpeed, Rotation, LifeTime, RotateWithVelocity, SortOffset);

    public Particle WithRotation(float rotation) =>
        new Particle(Texture, Colors, Scale, Alpha, Acceleration, Friction, StartVelocity, RotationSpeed, new(rotation), LifeTime, RotateWithVelocity, SortOffset);

    /// <summary>
    /// Calculate the color of a particle in a <paramref name="delta"/> with internal {0, 1}.
    /// </summary>
    /// <param name="delta">Delta from 0 to 1.</param>
    public Color CalculateColor(float delta)
    {
        if (Colors.Length == 0) return Color.White;
        if (Colors.Length == 1) return Colors[0];

        float interval = 1f / Colors.Length;

        int target = Calculator.FloorToInt(delta / interval);
        if (target >= Colors.Length - 1)
        {
            return Colors[Colors.Length - 1];
        }

        float remaining = delta % interval;
        float internalDelta = Calculator.Clamp01(remaining / interval);

        return Color.Lerp(Colors[target], Colors[target + 1], internalDelta);
    }

    /// <summary>
    /// Calculate the scale of a particle in a <paramref name="delta"/> with internal {0, 1}.
    /// </summary>
    /// <param name="delta">Delta from 0 to 1.</param>
    public Vector2 CalculateScale(float delta)
    {
        if (Scale.Length == 0) return Vector2.One;
        if (Scale.Length == 1) return Scale[0];

        float interval = 1f / Scale.Length;

        int target = Calculator.FloorToInt(delta / interval);
        if (target >= Scale.Length - 1)
        {
            return Scale[Scale.Length - 1];
        }

        float remaining = delta % interval;
        float internalDelta = Calculator.Clamp01(remaining / interval);

        return Vector2.Lerp(Scale[target], Scale[target + 1], internalDelta);
    }
}