using Bang;
using Bang.Entities;
using Murder.Assets;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using System.Collections.Immutable;
using System.Text;

namespace Murder.Services;

public static class DialogueServices
{
    public static CharacterRuntime? CreateCharacterFrom(Guid character, string? situation)
    {
        if (situation is null)
        {
            return null;
        }

        Character? result = MurderSaveServices.CreateOrGetSave().BlackboardTracker
            .FetchCharacterFor(character);

        if (result is null)
        {
            return null;
        }

        return new CharacterRuntime(result.Value, situation);
    }

    public static LineComponent CreateLine(Line line)
    {
        return new(line, Game.NowUnscaled);
    }

    public static Line[] FetchAllLines(World? world, Entity? target, SituationComponent situation)
    {
        CharacterRuntime? character = CreateCharacterFrom(situation.Character, situation.Situation);
        if (character is null)
        {
            return [];
        }

        List<Line>? lines = null;

        while (character.NextLine(world, target) is DialogLine dialogLine)
        {
            if (dialogLine.Line is Line line)
            {
                lines ??= [];
                lines.Add(line);
            }
            else if (dialogLine.Choice is ChoiceLine)
            {
                GameLogger.Warning("We did not implement choices for fetching all lines yet.");
                break;
            }
        }

        return lines?.ToArray() ?? [];
    }

    public static string FetchAllLinesSeparatedBy(World? world, Entity? target, SituationComponent situation, string separator)
    {
        bool first = true;

        StringBuilder builder = new();
        foreach (Line line in FetchAllLines(world, target, situation))
        {
            if (line.IsText && line.Text is LocalizedString localizedString)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(separator);
                }

                builder.Append(LocalizationServices.GetLocalizedString(localizedString));
            }
        }

        return builder.ToString();
    }

    public static string FetchFirstLine(World? world, Entity? target, SituationComponent situation)
    {
        if (situation.Character == Guid.Empty)
        {
            return string.Empty;
        }

        CharacterRuntime? character = CreateCharacterFrom(situation.Character, situation.Situation);
        if (character is null)
        {
            return string.Empty;
        }

        while (character.NextLine(world, target) is DialogLine dialogLine)
        {
            if (dialogLine.Line is Line line && line.IsText && line.Text is LocalizedString localizedString)
            {
                return LocalizationServices.GetLocalizedString(localizedString);
            }
            else if (dialogLine.Choice is ChoiceLine)
            {
                break;
            }
        }

        return string.Empty;
    }

    public static bool HasNewDialogue(World world, Entity? e, SituationComponent situation)
    {
        if (situation.Character == Guid.Empty)
        {
            return false;
        }

        CharacterRuntime? character = CreateCharacterFrom(situation.Character, situation.Situation);
        if (character is null)
        {
            return false;
        }

        if (character.HasContentOnNextDialogueLine(world, e, checkForNewContentOnly: true))
        {
            return true;
        }

        return false;
    }

    public static bool HasDialogue(World world, Entity? e, SituationComponent situation)
    {
        if (situation.Character == Guid.Empty)
        {
            return false;
        }

        CharacterRuntime? character = CreateCharacterFrom(situation.Character, situation.Situation);
        if (character is null)
        {
            return false;
        }

        return character.HasContentOnNextDialogueLine(world, e, checkForNewContentOnly: false);
    }

    public static void SetOverrideSituation(this Entity e, SituationOrigin origin, SituationComponent situation)
    {
        if (e.TryGetOverrideSituation() is not OverrideSituationComponent existingSituation)
        {
            var builder = ImmutableDictionary.CreateBuilder<SituationOrigin, SituationComponent>();
            builder[origin] = situation;

            e.SetOverrideSituation(builder.ToImmutable());
            return;
        }

        e.SetOverrideSituation(existingSituation.WithSituation(origin, situation));
    }

    public static void RemoveOverrideSituation(this Entity e, SituationOrigin origin)
    {
        if (e.TryGetOverrideSituation() is not OverrideSituationComponent existingSituation)
        {
            return;
        }

        existingSituation = existingSituation.WithoutSituation(origin);
        if (existingSituation.IsEmpty)
        {
            e.RemoveOverrideSituation();
        }
        else
        {
            e.SetOverrideSituation(existingSituation);
        }
    }

    public static string GetPortraitName(SpeakerAsset speaker, string? portrait, string? fallback = null)
    {
        if (portrait is not null)
        {
            return portrait;
        }

        if (fallback is not null)
        {
            return fallback;
        }

        return speaker.DefaultPortrait ?? speaker.Portraits.Keys.First();
    }
}