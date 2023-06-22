using Murder.Core.Dialogs;

namespace Murder.Core.Sounds
{
    /// <summary>
    /// This is a generic blackboard action with a command.
    /// </summary>
    public readonly struct ParameterRuleAction
    {
        public readonly ParameterId Parameter = default;
        public readonly BlackboardActionKind Kind = BlackboardActionKind.Set;
        public readonly float Value = 0;

        public ParameterRuleAction() { }

        public ParameterRuleAction(ParameterId parameter, BlackboardActionKind kind, float value)
        {
            Parameter = parameter;
            Kind = kind;
            Value = value;
        }
    }
}
