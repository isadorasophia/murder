using System.Diagnostics;
using System.Text;

namespace Murder.Core.Dialogs
{
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public readonly struct CriterionNode
    {
        public readonly Criterion Criterion = new();
        public readonly CriterionNodeKind Kind = CriterionNodeKind.And;

        public CriterionNode() { }

        public CriterionNode(Criterion criterion) =>
            Criterion = criterion;

        public CriterionNode(Criterion criterion, CriterionNodeKind kind) =>
            (Criterion, Kind) = (criterion, kind);

        public CriterionNode WithCriterion(Criterion criterion) => new(criterion, Kind);

        public CriterionNode WithKind(CriterionNodeKind kind) => new(Criterion, kind);

        public string DebuggerDisplay()
        {
            StringBuilder result = new();

            result.Append($"{Criterion.Fact.Name} {Criterion.Kind} ");

            switch (Criterion.Fact.Kind)
            {
                case FactKind.Bool:
                    result = result.Append(Criterion.BoolValue);
                    break;

                case FactKind.Int:
                    result = result.Append(Criterion.IntValue);
                    break;

                case FactKind.String:
                    result = result.Append(Criterion.StrValue);
                    break;

                case FactKind.Weight:
                    result = result.Append(Criterion.IntValue);
                    break;

                case FactKind.Float:
                    result = result.Append(Criterion.FloatValue);
                    break;
            }

            return result.ToString();
        }
    }
}