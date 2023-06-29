using Murder.Core.Dialogs;

namespace Murder.Editor.Services
{
    internal static class BlackboardServices
    {
        public static Criterion WithFact(this Criterion @this, Fact fact, Type targetType)
        {
            bool? @bool = @this.BoolValue;
            int? @int = @this.IntValue;
            string? @string = @this.StrValue;

            object? value = null;

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

                case FactKind.Float:
                    value = 0.0f;
                    break;

                default:
                    value = Activator.CreateInstance(targetType);
                    break;
            }

            return new(fact, @this.Kind, @bool, @int, @string, value);
        }

        public static Criterion WithKind(this Criterion @this, CriterionKind kind)
        {
            return new(@this.Fact, kind, @this.BoolValue, @this.IntValue, @this.StrValue, @this.Value);
        }

        /// <summary>
        /// This returns a list of all the valid criteria kind for the fact.
        /// </summary>
        /// <returns></returns>
        public static CriterionKind[] FetchValidCriteriaKind(this Criterion @this)
        {
            switch (@this.Fact.Kind)
            {
                case FactKind.Bool:
                case FactKind.Component:
                    return new CriterionKind[] { CriterionKind.Is };

                case FactKind.Int:
                case FactKind.Float:
                    return new CriterionKind[] { CriterionKind.Less, CriterionKind.LessOrEqual, CriterionKind.Is, CriterionKind.Different, CriterionKind.BiggerOrEqual, CriterionKind.Bigger };

                case FactKind.String:
                case FactKind.Enum:
                case FactKind.Any:
                    return new CriterionKind[] { CriterionKind.Is };
            }

            return Array.Empty<CriterionKind>();
        }
    }
}
