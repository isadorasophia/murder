using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Utilities;

namespace Murder.Services
{
    public static class DialogServices
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
    }
}
