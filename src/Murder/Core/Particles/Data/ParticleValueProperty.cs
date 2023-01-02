using Murder.Utilities;
using Newtonsoft.Json;

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

        public float GetValue(Random random)
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
        
        public float CalculateMaxValue()
        {
            switch (Kind)
            {
                case ParticleValuePropertyKind.Constant:
                    return _constant;

                case ParticleValuePropertyKind.Range:
                    return _rangeEnd;

                case ParticleValuePropertyKind.RangedStartAndRangedEnd:
                    return _rangeEndMax;

                case ParticleValuePropertyKind.Curve:
                default:
                    throw new NotImplementedException("Curve is not implemented yet.");
            }
        }
    }
}
