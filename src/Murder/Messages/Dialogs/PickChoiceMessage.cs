using Bang.Components;

namespace Murder.Messages
{
    public readonly struct PickChoiceMessage : IMessage
    {
        public readonly int Choice = 0;

        public readonly bool IsCancel = false;

        public PickChoiceMessage(int choice) => Choice = choice;

        public PickChoiceMessage(bool cancel) => IsCancel = cancel;
    }
}