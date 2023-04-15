using Bang.StateMachines;
using Bang.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Utilities;
using Murder.Services;
using Murder.Messages;
using Bang.Components;
using Murder.Diagnostics;

namespace Murder.StateMachines
{
    public class DialogStateMachine : StateMachine
    {
        private Character? _character;

        private int? _choice = null;

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

            _character = DialogServices.CreateCharacterFrom(situation.Character, situation.Situation);
            if (_character is null)
            {
                Entity.Destroy();
            }
        }

        public IEnumerator<Wait> Talk()
        {
            Debug.Assert(_character is not null);

            while (true)
            {
                if (_character.NextLine(World, Entity) is not DialogLine dialogLine)
                {
                    // No line was ever added, destroy the dialog.
                    if (!Entity.HasLine())
                    {
                        Entity.Destroy();
                        yield break;
                    }

                    Entity.RemoveLine();
                    yield break;
                }

                if (dialogLine.Line is Line line)
                {
                    LineComponent lineComponent = DialogServices.CreateLine(line);
                    Entity.SetLine(lineComponent);

                    if (line.IsText)
                    {
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
                    // Remove any previous line components.
                    Entity.RemoveLine();

                    ChoiceComponent choiceComponent = new(choice);
                    Entity.SetChoice(choiceComponent);

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