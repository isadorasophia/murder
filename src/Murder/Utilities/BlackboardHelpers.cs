using Bang;
using Murder.Core.Dialogs;
using Murder.Save;
using System.Collections.Immutable;

namespace Murder.Utilities
{
    public static class BlackboardHelpers
    {
        public static bool Match(World world, BlackboardTracker tracker, ImmutableArray<CriterionNode> requirements)
        {
            foreach (CriterionNode node in requirements)
            {
                if (!tracker.Matches(node.Criterion, /* character */ null, world, /* target */ null, out int weight) &&
                    node.Kind == CriterionNodeKind.And)
                {
                    // Nope, give up.
                    return false;
                }
            }

            return true;
        }

    }
}
