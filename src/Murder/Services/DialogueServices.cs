using Bang.Entities;
using Bang;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Diagnostics;

namespace Murder.Services
{
    public static class DialogueServices
    {
        public static Character? CreateCharacterFrom(Guid character, int situation)
        {
            Character? result = MurderSaveServices.CreateOrGetSave().BlackboardTracker
                .FetchCharacterFor(character);

            if (result is null)
            {
                return null;
            }

            result.StartAtSituation(situation);
            return result;
        }
        
        public static LineComponent CreateLine(Line line)
        {
            return new(line, Game.NowUnscaled);
        }

        public static Line[] FetchAllLines(World world, Entity target, SituationComponent situation)
        {
            Character? character = CreateCharacterFrom(situation.Character, situation.Situation);
            if (character is null)
            {
                return Array.Empty<Line>();
            }

            List<Line> lines = new();

            while (character.NextLine(world, target) is DialogLine dialogLine)
            {
                if (dialogLine.Line is Line line)
                {
                    lines.Add(line);
                }
                else if (dialogLine.Choice is ChoiceLine)
                {
                    GameLogger.Warning("We did not implement choices for fetching all lines yet.");
                    break;
                }
            }

            return lines.ToArray();
        }
    }
}
