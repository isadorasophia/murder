using Murder.Core.Geometry;
using Murder.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Core.Particles
{
    public readonly struct ParticleVectorValueProperty
    {
        [JsonProperty]
        public readonly ParticleValuePropertyKind Kind;

        /// <summary>
        /// Constant value set when <see cref="ParticleValuePropertyKind.Constant"/>.
        /// </summary>
        [JsonProperty]
        private readonly Vector2 _constant;

        /// <summary>
        /// Range value set when <see cref="ParticleValuePropertyKind.Range"/>.
        /// </summary>
        [JsonProperty]
        private readonly Vector2 _rangeStart;
        [JsonProperty]
        private readonly Vector2 _rangeEnd;

        /// <summary>
        /// Range value set when <see cref="ParticleValuePropertyKind.RangedStartAndRangedEnd"/>.
        /// </summary>
        [JsonProperty]
        private readonly Vector2 _rangeStartMin;
        [JsonProperty]
        private readonly Vector2 _rangeStartMax;
        [JsonProperty]
        private readonly Vector2 _rangeEndMin;
        [JsonProperty]
        private readonly Vector2 _rangeEndMax;

        // TODO: Curve.

        [JsonConstructor]
        public ParticleVectorValueProperty() { }

        public static ParticleVectorValueProperty Empty => new(constant: Vector2.Zero);

        public ParticleVectorValueProperty(Vector2 constant)
        {
            Kind = ParticleValuePropertyKind.Constant;
            _constant = constant;
        }

        public ParticleVectorValueProperty(Vector2 rangeStart, Vector2 rangeEnd)
        {
            Kind = ParticleValuePropertyKind.Range;

            _rangeStart = rangeStart;
            _rangeEnd = rangeEnd;
        }

        public ParticleVectorValueProperty(Vector2 rangeStartMin, Vector2 rangeStartMax, Vector2 rangeEndMin, Vector2 rangeEndMax)
        {
            Kind = ParticleValuePropertyKind.Range;

            _rangeStartMin = rangeStartMin;
            _rangeStartMax = rangeStartMax;
            _rangeEndMin = rangeEndMin;
            _rangeEndMax = rangeEndMax;
        }

        public Vector2 GetRandomValue(Random random)
        {
            switch (Kind)
            {
                case ParticleValuePropertyKind.Constant:
                    return _constant;

                case ParticleValuePropertyKind.Range:
                    return Vector2.Lerp(_rangeStart, _rangeEnd, random.NextFloat());

                case ParticleValuePropertyKind.RangedStartAndRangedEnd:
                    (Vector2 min, Vector2 max) = random.TryWithChanceOf(50) ?
                        (_rangeStartMin, _rangeStartMax) :
                        (_rangeEndMin, _rangeEndMax);

                    return Vector2.Lerp(min, max, random.NextFloat());

                default:
                    // Curve is not implemented yet.
                    return Vector2.Zero;
            };
        }

        /// <summary>
        /// Get the value of this property over a delta lifetime.
        /// </summary>
        /// <param name="delta">Delta ranges from 0 to 1.</param>
        public Vector2 GetValueAt(float delta)
        {
            switch (Kind)
            {
                case ParticleValuePropertyKind.Constant:
                    return _constant;

                case ParticleValuePropertyKind.Range:
                    return Vector2.LerpSnap(_rangeStart, _rangeEnd, delta);

                case ParticleValuePropertyKind.RangedStartAndRangedEnd:
                    // TODO: Actually implement this...?
                    return Vector2.LerpSnap(_rangeStartMin, _rangeEndMax, delta);

                default:
                    // Curve is not implemented yet.
                    return Vector2.Zero;
            };
        }
    }
}
