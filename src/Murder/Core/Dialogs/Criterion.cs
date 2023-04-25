namespace Murder.Core.Dialogs
{
    public readonly struct Criterion
    {
        public readonly Fact Fact = new();

        public readonly CriterionKind Kind = CriterionKind.Is;

        public readonly string? StrValue = null;

        public readonly int? IntValue = null;

        public readonly bool? BoolValue = null;

        public Criterion() { }
        
        /// <summary>
        /// Creates a fact of type <see cref="FactKind.Weight"/>.
        /// </summary>
        public static Criterion Weight => new(Fact.Weight, CriterionKind.Is, 1);

        /// <summary>
        /// Creates a fact of type <see cref="FactKind.Component"/>.
        /// </summary>
        public static Criterion Component => new(Fact.Component, CriterionKind.Is, true);

        public Criterion(Fact fact, CriterionKind kind, bool? @bool, int? @int, string? @string)
        {
            (Fact, Kind, StrValue, IntValue, BoolValue) = (fact, kind, @string, @int, @bool);
        }
        
        public Criterion(Fact fact, CriterionKind kind, object @value)
        {
            bool? @bool = null;
            int? @int = null;
            string? @string = null;

            // Do not propagate previous values.
            switch (fact.Kind)
            {
                case FactKind.Bool:
                    @bool = (bool)@value;
                    break;

                case FactKind.Int:
                    @int = (int)@value;
                    break;

                case FactKind.String:
                    @string = (string)@value;
                    break;

                case FactKind.Weight:
                    @int = (int)@value;
                    break;
            }

            (Fact, Kind, StrValue, IntValue, BoolValue) = (fact, kind, @string, @int, @bool);
        }

        public Criterion WithFact(Fact fact)
        {
            bool? @bool = BoolValue;
            int? @int = IntValue;
            string? @string = StrValue;

            switch (fact.Kind)
            {
                case FactKind.Bool:
                    @bool ??= false;
                    break;

                case FactKind.Int:
                    @int ??= 0;
                    break;

                case FactKind.String:
                    @string ??= string.Empty;
                    break;
            }

            return new(fact, Kind, @bool, @int, @string);
        }

        public Criterion WithKind(CriterionKind kind)
        {
            return new(Fact, kind, BoolValue, IntValue, StrValue);
        }

        /// <summary>
        /// This returns a list of all the valid criteria kind for the fact.
        /// </summary>
        /// <returns></returns>
        public CriterionKind[] FetchValidCriteriaKind()
        {
            switch (Fact.Kind)
            {
                case FactKind.Bool:
                case FactKind.Component:
                    return new CriterionKind[] { CriterionKind.Is };

                case FactKind.Int:
                    return new CriterionKind[] { CriterionKind.Less, CriterionKind.LessOrEqual, CriterionKind.Is, CriterionKind.Different, CriterionKind.BiggerOrEqual, CriterionKind.Bigger };

                case FactKind.String:
                    return new CriterionKind[] { CriterionKind.Is };
            }

            return Array.Empty<CriterionKind>();
        }
    }
}
