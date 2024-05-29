using Murder.Core.Dialogs;
using System.Text.Json.Serialization;

namespace Murder.Core.Sounds
{
    /// <summary>
    /// This is a generic blackboard action with a command.
    /// </summary>
    public readonly struct SoundRuleAction
    {
        public readonly SoundFact Fact = default;
        public readonly BlackboardActionKind Kind = BlackboardActionKind.Set;
        public readonly int? Value = null;

        public SoundRuleAction() { }

        [JsonConstructor]
        public SoundRuleAction(SoundFact fact, BlackboardActionKind kind, int? value)
        {
            Fact = fact;
            Kind = kind;
            Value = value;
        }
    }
}