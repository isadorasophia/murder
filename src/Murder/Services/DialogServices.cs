using Murder.Assets;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Diagnostics;

namespace Murder.Services
{
    public static class DialogServices
    {
        public static Character CreateCharacterFrom(Guid character, int situation)
        {
            if (Game.Data.TryGetAsset<CharacterAsset>(character) is not CharacterAsset asset)
            {
                GameLogger.Error("Unable to find character asset!");
                throw new InvalidOperationException();
            }

            return new(character, asset.Situations, situation);
        }
        
        public static LineComponent CreateLine(Line line)
        {
            return new(line, Game.NowUnescaled);
        }
    }
}
