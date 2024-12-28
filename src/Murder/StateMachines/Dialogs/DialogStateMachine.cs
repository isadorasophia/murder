using Bang.Components;
using Bang.Entities;
using Bang.StateMachines;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Diagnostics;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Murder.StateMachines
{
    public class DialogStateMachine : StateMachine
    {
        private CharacterRuntime? _character;

        private int? _choice = null;

        private readonly bool _destroyAfterDone = true;

        public DialogStateMachine(bool destroyAfterDone) : this()
        {
            _destroyAfterDone = destroyAfterDone;
        }

        public DialogStateMachine()
        {
            State(Talk);
        }

        [MemberNotNull(nameof(_character))]
        protected override void OnStart()
        {
            if (Entity.TryGetSituation() is not SituationComponent situation)
            {
                throw new ArgumentNullException(nameof(LineComponent));
            }

            _character = DialogueServices.CreateCharacterFrom(situation.Character, situation.Situation);
            if (_character is null)
            {
                _character = null!;

                Entity.Destroy();
            }
        }

        public IEnumerator<Wait> Talk()
        {
            if (_character is null)
            {
                GameLogger.Fail("Unable to fetch character.");
                yield break;
            }

            if (Entity.HasAutomaticNextDialogue())
            {
                Entity.SetDoNotPause();
            }

            while (true)
            {
                if (_character.NextLine(World, Entity) is not DialogLine dialogLine)
                {
                    // No line was ever added, destroy the dialog.
                    if (!Entity.HasLine())
                    {
                        if (_destroyAfterDone)
                        {
                            Entity.Destroy();
                        }
                        else
                        {
                            Entity.RemoveStateMachine();
                        }

                        yield break;
                    }

                    if (_destroyAfterDone)
                    {
                        Entity.Destroy();
                    }
                    else
                    {
                        Entity.RemoveLine();
                        Entity.RemoveStateMachine();
                    }

                    yield break;
                }

                if (dialogLine.Line is Line line)
                {
                    LineComponent lineComponent = DialogueServices.CreateLine(line);
                    Entity.SetLine(lineComponent);

                    if (line.IsText)
                    {
                        yield return Wait.NextFrame; // wait until next frame, if the entity still has a NextDialogMessage
                        yield return Wait.ForMessage<NextDialogMessage>();
                    }
                    else if (line.Delay is float delay)
                    {
                        int ms = Calculator.RoundToInt(delay * 1000);
                        yield return Wait.ForMs(ms);
                    }
                }
                else if (dialogLine.Choice is ChoiceLine choice)
                {
                    ChoiceComponent choiceComponent = new(choice);
                    Entity.SetChoice(choiceComponent);

                    yield return Wait.NextFrame; // wait until next frame, if the entity still has a PickChoiceMessage
                    yield return Wait.ForMessage<PickChoiceMessage>();

                    if (_choice is not int choiceIndex)
                    {
                        GameLogger.Error("How do we not track a choice made by the player?");

                        Entity.Destroy();
                        yield break;
                    }

                    _character.DoChoice(choiceIndex, World, Entity);
                    Entity.RemoveChoice();
                }
            }
        }

        protected override void OnMessage(IMessage message)
        {
            if (message is PickChoiceMessage pickChoiceMessage)
            {
                _choice = pickChoiceMessage.Choice;
            }
        }
    }
}