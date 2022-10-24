using Bang.StateMachines;
using Bang.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Entities;
using Murder.Utilities;
using Murder.Services;
using Murder.Messages;

namespace Murder.StateMachines
{
    internal class DialogStateMachine : StateMachine
    {
        private Character? _character;

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
        }

        public IEnumerator<Wait> Talk()
        {
            Debug.Assert(_character is not null);

            while (true)
            {
                if (_character.NextLine() is not Line line)
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
        }
    }
}