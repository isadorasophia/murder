using Murder.Utilities;
using Newtonsoft.Json;

namespace Murder.Core.Particles
{
    public readonly struct ParticleIntValueProperty
    {
        [JsonProperty]
        public readonly ParticleValuePropertyKind Kind;

        /// <summary>
        /// Constant value set when <see cref="ParticleValuePropertyKind.Constant"/>.
        /// </summary>
        [JsonProperty]
        private readonly int _constant;

        /// <summary>
        /// Range value set when <see cref="ParticleValuePropertyKind.Range"/>.
        /// </summary>
        [JsonProperty]
        private readonly int _rangeStart;
        [JsonProperty]
        private readonly int _rangeEnd;

        /// <summary>
        /// Range value set when <see cref="ParticleValuePropertyKind.RangedStartAndRangedEnd"/>.
        /// </summary>
        [JsonProperty]
        private readonly int _rangeStartMin;
        [JsonProperty]
        private readonly int _rangeStartMax;
        [JsonProperty]
        private readonly int _rangeEndMin;
        [JsonProperty]
        private readonly int _rangeEndMax;

        // TODO: Curve.

        [JsonConstructor]
        public ParticleIntValueProperty() { }

        public static ParticleIntValueProperty Empty => new(constant: 1);

        public ParticleIntValueProperty(int constant)
        {
            Kind = ParticleValuePropertyKind.Constant;
            _constant = constant;
        }

        public ParticleIntValueProperty(int rangeStart, int rangeEnd)
        {
            Kind = ParticleValuePropertyKind.Range;

            _rangeStart = rangeStart;
            _rangeEnd = rangeEnd;
        }

        public ParticleIntValueProperty(int rangeStartMin, int rangeStartMax, int rangeEndMin, int rangeEndMax)
        {
            Kind = ParticleValuePropertyKind.Range;

            _rangeStartMin = rangeStartMin;
            _rangeStartMax = rangeStartMax;
            _rangeEndMin = rangeEndMin;
            _rangeEndMax = rangeEndMax;
        }

        public int GetValue(Random random)
        {
            switch (Kind)
            {
                case ParticleValuePropertyKind.Constant:
                    return _constant;
                    
                case ParticleValuePropertyKind.Range:
                    return random.Next(_rangeStart, _rangeEnd);
                    
                case ParticleValuePropertyKind.RangedStartAndRangedEnd:
                    (int min, int max) = random.TryWithChanceOf(50) ? 
                        (_rangeStartMin, _rangeStartMax) :
                        (_rangeEndMin, _rangeEndMax);
                    
                    return random.Next(min, max);
                        
                default:
                    throw new NotImplementedException("Curve is not implemented yet.");
            };
        }
        
        public int CalculateMaxValue()
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
