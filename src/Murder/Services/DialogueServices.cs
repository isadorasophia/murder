using Bang;
using Bang.Entities;
using Murder.Assets;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using System.Text;

namespace Murder.Services
{
    public static class DialogueServices
    {
        public static CharacterRuntime? CreateCharacterFrom(Guid character, string situation)
        {
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
                return Array.Empty<Line>();
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

            return lines?.ToArray() ?? Array.Empty<Line>();
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
            CharacterRuntime? character = CreateCharacterFrom(situation.Character, situation.Situation);
            if (character is null)
            {
                return false;
            }

            return character.HasContentOnNextDialogueLine(world, e, checkForNewContentOnly: false);
        }
    }
}