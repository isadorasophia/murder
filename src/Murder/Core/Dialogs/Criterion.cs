namespace Murder.Core.Dialogs
{
    public readonly struct Criterion
    {
        public readonly Fact Fact = new();

        public readonly CriterionKind Kind = CriterionKind.Is;

        public readonly string? StrValue = null;

        public readonly int? IntValue = null;

        public readonly float? FloatValue = null;

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

        public Criterion(Fact fact, CriterionKind kind, bool? @bool, int? @int, float? @float, string? @string)
        {
            (Fact, Kind, StrValue, IntValue, FloatValue, BoolValue) = (fact, kind, @string, @int, @float, @bool);
        }

        public Criterion(Fact fact, CriterionKind kind, object @value)
        {
            bool? @bool = null;
            int? @int = null;
            float? @float = null;
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

                case FactKind.Float:
                    @float = (float)@value;
                    break;

                case FactKind.String:
                    @string = (string)@value;
                    break;

                case FactKind.Weight:
                    @int = (int)@value;
                    break;
            }

            (Fact, Kind, StrValue, IntValue, FloatValue, BoolValue) = (fact, kind, @string, @int, @float, @bool);
        }
    }
}