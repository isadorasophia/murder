using Murder.Assets;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Services;

namespace Murder.Editor.CustomDiagnostics;

[CustomDiagnostic(typeof(SituationComponent))]
internal class SituationComponentDiagnostic : ICustomDiagnostic
{
    public bool IsValid(string identifier, in object target, bool outputResult)
    {
        if (target is null)
        {
            return true;
        }

        SituationComponent situation = (SituationComponent)target;
        if (situation.Situation is null || 
            Game.Data.TryGetAsset<CharacterAsset>(situation.Character) is not CharacterAsset character)
        {
            return false;
        }

        if (DialogueServices.CreateCharacterFrom(situation.Character, situation.Situation) is not CharacterRuntime runtime)
        {
            return false;
        }

        // Now, check if we have all the lines available.
        if (character.TryFetchSituation(situation.Situation) is Situation situationOnDialogue)
        {
            foreach (Dialog d in situationOnDialogue.Dialogs)
            {
                foreach (Line l in d.Lines)
                {
                    if (!l.IsText)
                    {
                        continue;
                    }

                    if (LocalizationServices.TryGetLocalizedString(l.Text) is null)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}