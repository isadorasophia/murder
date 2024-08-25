namespace Murder.Core.Dialogs;

[AttributeUsage(AttributeTargets.Field)]
public class BlackboardFieldAttribute : Attribute 
{
    public readonly BlackboardKind Kind;

    public BlackboardFieldAttribute(BlackboardKind kind) => Kind = kind;
}
