using Bang.Components;

namespace Murder.Core.Dialogs
{
    public readonly struct DialogAction
    {
        public readonly int Id = 0;

        public readonly Fact Fact = default;

        public readonly BlackboardActionKind Kind = BlackboardActionKind.Set;

        public readonly string? StrValue = null;

        public readonly int? IntValue = null;

        public readonly bool? BoolValue = null;

        public readonly IComponent? ComponentValue = null;

        public DialogAction() { }

        public DialogAction(int id, Fact fact, BlackboardActionKind kind, string? @string, int? @int, bool? @bool, IComponent? component)
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
            }

            (Id, Fact, Kind, StrValue, IntValue, BoolValue, ComponentValue) = (id, fact, kind, @string, @int, @bool, component);
        }

        public DialogAction WithComponent(IComponent c)
        {
            return new(Id, Fact, Kind, StrValue, IntValue, BoolValue, c);
        }
    }
}
