using Murder.Core.Dialogs;

namespace Murder.Editor.Services
{
    internal static class BlackboardServices
    {
        public static Criterion WithFact(this Criterion @this, Fact fact, Type? targetType)
        {
            bool? @bool = @this.BoolValue;
            int? @int = @this.IntValue;
            float? @float = @this.FloatValue;
            string? @string = @this.StrValue;

            object? value = null;

            switch (fact.Kind)
            {
                case FactKind.Bool:
                    @bool ??= false;
                    break;

                case FactKind.Enum:
                case FactKind.Int:
                    @int ??= 0;
                    break;

                case FactKind.String:
                    @string ??= string.Empty;
                    break;

                case FactKind.Float:
                    @float = 0.0f;
                    break;

                default:
                    value = targetType is not null ? Activator.CreateInstance(targetType) : null;
                    break;
            }

            return new(fact, @this.Kind, @bool, @int, @float, @string);
        }

        public static Criterion WithKind(this Criterion @this, CriterionKind kind)
        {
            return new(@this.Fact, kind, @this.BoolValue, @this.IntValue, @this.FloatValue, @this.StrValue);
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
                    return [CriterionKind.Is];

                case FactKind.Int:
                case FactKind.Float:
                    return [CriterionKind.Less, CriterionKind.LessOrEqual, CriterionKind.Is, CriterionKind.Different, CriterionKind.BiggerOrEqual, CriterionKind.Bigger];

                case FactKind.String:
                case FactKind.Enum:
                case FactKind.Any:
                    return [CriterionKind.Is];
            }

            return [];
        }
    }
}