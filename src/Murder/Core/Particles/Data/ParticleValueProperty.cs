using Murder.Utilities;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Core.Particles
{
    public readonly struct ParticleValueProperty
    {
        [JsonProperty]
        public readonly ParticleValuePropertyKind Kind;
        
        /// <summary>
        /// Constant value set when <see cref="ParticleValuePropertyKind.Constant"/>.
        /// </summary>
        [JsonProperty]
        private readonly float _constant;

        /// <summary>
        /// Range value set when <see cref="ParticleValuePropertyKind.Range"/>.
        /// </summary>
        [JsonProperty]
        private readonly float _rangeStart;
        [JsonProperty]
        private readonly float _rangeEnd;

        /// <summary>
        /// Range value set when <see cref="ParticleValuePropertyKind.RangedStartAndRangedEnd"/>.
        /// </summary>
        [JsonProperty]
        private readonly float _rangeStartMin;
        [JsonProperty]
        private readonly float _rangeStartMax;
        [JsonProperty]
        private readonly float _rangeEndMin;
        [JsonProperty]
        private readonly float _rangeEndMax;

        // TODO: Curve.
        [JsonProperty]
        private readonly ImmutableArray<float> _curvePoints = ImmutableArray<float>.Empty;

        [JsonConstructor]
        public ParticleValueProperty() { }

        public static ParticleValueProperty Empty => new(constant: 1);

        public ParticleValueProperty(float constant)
        {
            Kind = ParticleValuePropertyKind.Constant;
            _constant = constant;
        }

        public ParticleValueProperty(float rangeStart, float rangeEnd)
        {
            Kind = ParticleValuePropertyKind.Range;

            _rangeStart = rangeStart;
            _rangeEnd = rangeEnd;
        }

        public ParticleValueProperty(float rangeStartMin, float rangeStartMax, float rangeEndMin, float rangeEndMax)
        {
            Kind = ParticleValuePropertyKind.Range;

            _rangeStartMin = rangeStartMin;
            _rangeStartMax = rangeStartMax;
            _rangeEndMin = rangeEndMin;
            _rangeEndMax = rangeEndMax;
        }

        public float GetRandomValue(Random random)
        {
            switch (Kind)
            {
                case ParticleValuePropertyKind.Constant:
                    return _constant;
                    
                case ParticleValuePropertyKind.Range:
                    return random.NextFloat(_rangeStart, _rangeEnd);
                    
                case ParticleValuePropertyKind.RangedStartAndRangedEnd:
                    (float min, float max) = random.TryWithChanceOf(50) ? 
                        (_rangeStartMin, _rangeStartMax) :
                        (_rangeEndMin, _rangeEndMax);
                    
                    return random.NextFloat(min, max);
                default:
                    // Curve is not implemented yet.
                    return 1;
            };
        }
        
        /// <summary>
        /// Get the value of this property over a delta lifetime.
        /// </summary>
        /// <param name="delta">Delta ranges from 0 to 1.</param>
        public float GetValueAt(float delta)
        {
            switch (Kind)
            {
                case ParticleValuePropertyKind.Constant:
                    return _constant;

                case ParticleValuePropertyKind.Range:
                    return Calculator.LerpSnap(_rangeStart, _rangeEnd, delta);

                case ParticleValuePropertyKind.RangedStartAndRangedEnd:
                    // TODO: Actually implement this...?
                    return Calculator.LerpSnap(_rangeStartMin, _rangeEndMax, delta);

                case ParticleValuePropertyKind.Curve:
                    return Calculator.InterpolateSmoothCurve(_curvePoints, delta);
                    
                default:
                    // Curve is not implemented yet.
                    return 1;
            };
        }
    }
}
