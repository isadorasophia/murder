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
        public static Criterion Weight => new(Fact.Weight, CriterionKind.Is, @int: 1);

        /// <summary>
        /// Creates a fact of type <see cref="FactKind.Component"/>.
        /// </summary>
        public static Criterion Component => new(Fact.Component, CriterionKind.Is, @bool: true);

        public Criterion(Fact fact, CriterionKind kind, string? @string = default, int? @int = default, bool? @bool = default)
        {
            // Do not propagate previous values.
            switch (fact.Kind)
            {
                case FactKind.Bool:
                    @bool ??= false;
                    @int = null;
                    @string = null;
                    break;

                case FactKind.Int:
                    @int ??= 0;
                    @string = null;
                    @bool = null;
                    break;
                    
                case FactKind.String:
                    @string ??= string.Empty;
                    @int = null;
                    @bool = null;
                    break;

                case FactKind.Weight:
                    @int ??= 1;
                    @bool = null;
                    @string = null;
                    break;
            }

            (Fact, Kind, StrValue, IntValue, BoolValue) = (fact, kind, @string, @int, @bool);
        }

        public Criterion WithFact(Fact fact)
        {
            return new(fact, Kind, StrValue, IntValue, BoolValue);
        }
        
        public Criterion WithKind(CriterionKind kind)
        {
            return new(Fact, kind, StrValue, IntValue, BoolValue);
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
                    return new CriterionKind[] { CriterionKind.Less, CriterionKind.LessOrEqual, CriterionKind.Equal, CriterionKind.BiggerOrEqual, CriterionKind.Bigger };
                    
                case FactKind.String:
                    return new CriterionKind[] { CriterionKind.Matches };
            }

            return new CriterionKind[] { };
        }
    }
}
