using Murder.Core.Dialogs;

namespace Murder.Core.Sounds
{
    /// <summary>
    /// This is a generic blackboard action with a command.
    /// </summary>
    public readonly struct SoundRuleAction
    {
        public readonly SoundFact Fact = default;
        public readonly BlackboardActionKind Kind = BlackboardActionKind.Set;
        public readonly object? Value = null;

        public SoundRuleAction() { }

        public SoundRuleAction(SoundFact fact, BlackboardActionKind kind, object? value)
        {
            Fact = fact;
            Kind = kind;
            Value = value;
        }
    }
}