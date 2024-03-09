using Bang.Components;
using System.Collections.Immutable;

namespace Murder.Core.Dialogs
{
    public readonly struct DialogAction
    {
        public readonly int Id = 0;

        public readonly Fact Fact = default;

        public readonly BlackboardActionKind Kind = BlackboardActionKind.Set;

        public readonly string? StrValue = null;

        public readonly int? IntValue = null;

        public readonly float? FloatValue = null;

        public readonly bool? BoolValue = null;

        public readonly IComponent? ComponentValue = null;

        public DialogAction() { }

        public DialogAction(int id, Fact fact, BlackboardActionKind kind, string? @string, int? @int, bool? @bool, float? @float, IComponent? component)
        {
            // Do not propagate previous values.
            switch (fact.Kind)
            {
                case FactKind.Bool:
                    @bool ??= false;
                    @int = null;
                    @string = null;
                    @float = null;
                    break;

                case FactKind.Int:
                    @int ??= 0;
                    @string = null;
                    @bool = null;
                    @float = null;
                    break;

                case FactKind.String:
                    @string ??= string.Empty;
                    @int = null;
                    @bool = null;
                    @float = null;
                    break;

                case FactKind.Float:
                    @string ??= null;
                    @int = null;
                    @bool = null;
                    @float ??= 0;
                    break;
            }

            (Id, Fact, Kind, StrValue, IntValue, BoolValue, ComponentValue) = (id, fact, kind, @string, @int, @bool, component);
        }

        public DialogAction WithComponent(IComponent c)
        {
            return new(Id, Fact, Kind, StrValue, IntValue, BoolValue, FloatValue, c);
        }
        public DialogAction WithFact(Fact fact)
        {
            return new(Id, fact, Kind, StrValue, IntValue, BoolValue, FloatValue, ComponentValue);
        }

        public DialogAction WithKind(BlackboardActionKind kind)
        {
            return new(Id, Fact, kind, StrValue, IntValue, BoolValue, FloatValue, ComponentValue);
        }
    }
}