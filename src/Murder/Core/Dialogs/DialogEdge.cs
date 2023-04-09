using System.Collections.Immutable;
using System.Text;
using System.Diagnostics;

namespace Murder.Core.Dialogs
{
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public readonly struct DialogEdge
    {
        public readonly MatchKind Kind;

        public readonly ImmutableArray<int> Dialogs;

        public DialogEdge(MatchKind kind, ImmutableArray<int> dialogs) =>
            (Kind, Dialogs) = (kind, dialogs);

        public string DebuggerDisplay()
        {
            StringBuilder result = new();

            result = result.Append(
            $"[{Kind}, Blocks = {{");

            bool isFirst = true;
            foreach (int i in Dialogs)
            {
                if (!isFirst)
                {
                    result = result.Append(", ");
                }
                else
                {
                    isFirst = false;
                }

                result = result.Append($"{i}");
            }

            result = result.Append($"}}]");

            return result.ToString();
        }
    }
}
