using Murder.Core.Sounds;
using Murder.Editor.CustomComponents;

namespace Murder.Editor.CustomDiagnostics
{
    [CustomDiagnostic(typeof(SoundRuleAction))]
    internal class SoundRuleActionDiagnostic : ICustomDiagnostic
    {
        public bool IsValid(string identifier, in object target, bool outputResult)
        {
            if (target is null)
            {
                return true;
            }

            SoundRuleAction action = (SoundRuleAction)target;
            Type? targetType = SoundRuleActionComponent.FetchTargetFieldType(action.Fact);

            return targetType is not null;
        }
    }
}
