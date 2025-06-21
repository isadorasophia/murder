using Murder.Attributes;
using Murder.Core.Geometry;
using System.Numerics;

namespace Murder.Core.Particles
{
    public readonly struct Emitter
    {
        [Tooltip("Maximum of particles that this emitter may have.")]
        public readonly int MaxParticlesPool = 100;

        [Tooltip("The shape which will be used to fire particles from.")]
        public readonly EmitterShape Shape = default;

        [Tooltip("Angle which the particles will be fired.")]
        [Angle]
        public readonly ParticleValueProperty Angle = ParticleValueProperty.Empty;

        [Tooltip("Amount of particles which will be fired every second.")]
        public readonly ParticleValueProperty ParticlesPerSecond = ParticleValueProperty.Empty;

        [Tooltip("Amount of particles which will be fired at once.")]
        public readonly ParticleIntValueProperty Burst = ParticleIntValueProperty.Empty;

        [Tooltip("Speed that will fire the particles.")]
        public readonly ParticleValueProperty Speed = ParticleValueProperty.Empty;

        [Tooltip("If true Particles will only spawn and move using scaled time")]
        public readonly bool ScaledTime = true;

        public Emitter() { }

        public Emitter(int maxParticles, EmitterShape shape, ParticleValueProperty angle, ParticleValueProperty particlesPerSecond,
            ParticleIntValueProperty burst, ParticleValueProperty speed)
        {
            MaxParticlesPool = maxParticles;
            Shape = shape;
            Angle = angle;
            ParticlesPerSecond = particlesPerSecond;
            Burst = burst;
            Speed = speed;
        }

        public Emitter WithShape(EmitterShape shape) =>
            new(MaxParticlesPool, shape, Angle, ParticlesPerSecond, Burst, Speed);

        internal Rectangle BoundingBoxSize()
        {
            if (Shape.Kind == EmitterShapeKind.Rectangle)
            {
                return Shape.Rectangle;
            }
            else if (Shape.Kind == EmitterShapeKind.Point)
            {
                return new Rectangle(0, 0, 1, 1);
            }
            else if (Shape.Kind == EmitterShapeKind.Line)
            {
                return new Rectangle(
                    Shape.Line.Top,
                    Shape.Line.Left,
                    Shape.Line.Bottom,
                    Shape.Line.Right
                    );
            }
            else if (Shape.Kind == EmitterShapeKind.Circle)
            {
                return Rectangle.CenterRectangle(new Point(Shape.Circle.X, Shape.Circle.Y), Shape.Circle.Radius, Shape.Circle.Radius);
            }
            else //if (Shape.Kind == EmitterShapeKind.CircleOutline)
            {
                return Rectangle.CenterRectangle(new Point(Shape.Circle.X, Shape.Circle.Y), Shape.Circle.Radius, Shape.Circle.Radius);
            }
        }
    }
}