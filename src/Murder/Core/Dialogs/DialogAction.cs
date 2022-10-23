namespace Murder.Core.Dialogs
{
    public readonly struct DialogAction
    {
        public readonly Fact? Fact = null;

        public readonly BlackboardActionKind Kind = BlackboardActionKind.Set;

        public readonly string? StrValue = null;

        public readonly int? IntValue = null;

        public readonly bool? BoolValue = null;

        public DialogAction() { }

        public DialogAction(Fact? fact, BlackboardActionKind kind, string? @string, int? @int, bool? @bool)
        {
            // Do not propagate previous values.
            switch (fact?.Kind)
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
            }

            (Fact, Kind, StrValue, IntValue, BoolValue) = (fact, kind, @string, @int, @bool);
        }

        public DialogAction WithFact(Fact fact)
        {
            return new(fact, Kind, StrValue, IntValue, BoolValue);
        }

        public DialogAction WithKind(BlackboardActionKind kind)
        {
            return new(Fact, kind, StrValue, IntValue, BoolValue);
        }

        /// <summary>
        /// This returns a list of all the valid <see cref="BlackboardActionKind"/> for the <see cref="Fact"/>.
        /// </summary>
        public BlackboardActionKind[] FetchValidActionKind()
        {
            if (Fact is null)
            {
                return new BlackboardActionKind[] { };
            }

            switch (Fact.Value.Kind)
            {
                case FactKind.Bool:
                    return new BlackboardActionKind[] { BlackboardActionKind.Set };

                case FactKind.Int:
                    return new BlackboardActionKind[] { BlackboardActionKind.Set, BlackboardActionKind.Add, BlackboardActionKind.Minus };

                case FactKind.String:
                    return new BlackboardActionKind[] { BlackboardActionKind.Set };
            }

            return new BlackboardActionKind[] { };
        }
    }
}
