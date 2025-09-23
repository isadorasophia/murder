using Bang;
using Murder.Assets;
using Murder.Components;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using Murder.Save;
using Murder.Services;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace Murder.Utilities;

public static class BlackboardHelpers
{
    public static bool Match(World world, ImmutableArray<CriterionNode> requirements)
    {
        if (requirements.IsEmpty)
        {
            return true;
        }

        if (MurderSaveServices.TryGetSave() is not SaveData save)
        {
            return false;
        }

        return Match(world, save.BlackboardTracker, requirements);
    }

    public static bool Match(World world, BlackboardTracker tracker, ImmutableArray<CriterionNode> requirements)
    {
        // whether it matched at least once
        bool matched = false;

        for (int i = 0; i < requirements.Length; ++i)
        {
            CriterionNode node = requirements[i];
            CriterionNodeKind nextKind = i + 1 < requirements.Length ? requirements[i + 1].Kind : node.Kind;

            bool matchedCurrentNode = tracker.Matches(node.Criterion, /* character */ null, world, /* target */ null, out int weight);
            matched |= matchedCurrentNode;

            if (!matchedCurrentNode && nextKind == CriterionNodeKind.And)
            {
                // Nope, give up.
                return false;
            }
        }

        return matched;
    }

    public static bool FormatText(string text, out string newText)
    {
        newText = text;

        MatchCollection matches = Regex.Matches(text, "{([^}]+)}");
        if (matches.Count == 0)
        {
            return false;
        }

        ReadOnlySpan<char> rawText = text;

        StringBuilder result = new();
        int lastIndex = 0;
        foreach (Match match in matches)
        {
            result.Append(rawText.Slice(lastIndex, match.Index - lastIndex));

            string fieldName = match.Groups[1].Value;
            string? value = MurderSaveServices.CreateOrGetSave().BlackboardTracker.GetValueAsString(fieldName);

            if (value is null)
            {
                GameLogger.Error($"Unable to fetch dialog value of {fieldName}.");
            }

            result.Append(value);

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < rawText.Length)
        {
            result.Append(rawText.Slice(lastIndex));
        }

        newText = result.ToString();
        return true;
    }

    public static bool IsSatisfied(World world, ImmutableArray<ICondition> conditions)
    {
        foreach (ICondition condition in conditions)
        {
            if (!condition.IsSatisfiedBy(world))
            {
                return false;
            }
        }

        return true;
    }

    public static bool Match(World world, OnlyApplyOnRuleComponent onlyApplyWhen)
    {
        return Match(world, onlyApplyWhen.Requirements) && IsSatisfied(world, onlyApplyWhen.Conditions);
    }
}