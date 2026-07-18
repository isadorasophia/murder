using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [Unique]
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct ChoiceComponent : IComponent
    {
        public readonly ChoiceLine Choice;
        public readonly int Sequence = 0;

        public ChoiceComponent(ChoiceLine choice, int sequence)
        {
            Choice = choice;
            Sequence = sequence;
        }
    }
}