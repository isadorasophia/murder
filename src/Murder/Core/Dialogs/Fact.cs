namespace Murder.Core.Dialogs
{
    public enum FactKind
    { 
        Invalid,
        Int,
        Bool,
        String
    }

    public readonly struct Fact
    {
        public readonly string Blackboard = "Global";
        public readonly string Name = string.Empty;
        public readonly FactKind Kind = FactKind.Invalid;

        public readonly string EditorName => Name; //$"{Blackboard}.{Name}";

        public Fact() { }

        public Fact(string blackboard, string name, FactKind kind) =>
            (Blackboard, Name, Kind) = (blackboard, name, kind);
    }
}
