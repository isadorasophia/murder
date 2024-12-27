using Murder.Core.Dialogs;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Data
{
    public class BlackboardInfo
    {
        public readonly string Name;

        public readonly Guid? Guid;

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
        public readonly Type Type;

        public readonly IBlackboard Blackboard;

        public BlackboardInfo(
            string name, 
            Guid? guid, 
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] Type type, 
            IBlackboard blackboard)
        {
            Name = name;
            Guid = guid;
            Type = type;
            Blackboard = blackboard;
        }
    }
}