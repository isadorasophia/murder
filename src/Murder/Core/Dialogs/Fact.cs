namespace Murder.Core.Dialogs
{
    public enum FactKind
    { 
        Invalid,
        Int,
        Bool,
        String,
        
        /// <summary>
        /// Used when the fact is only a weight which will be applied when picking
        /// the most suitable dialog.
        /// </summary>
        Weight
    }

    public readonly struct Fact
    {
        public readonly string Blackboard = "Global";
        public readonly string Name = string.Empty;
        public readonly FactKind Kind = FactKind.Invalid;

        public readonly string EditorName => Name; //$"{Blackboard}.{Name}";

        public Fact() { }

        private Fact(FactKind kind) => Kind = kind;

        public Fact(string blackboard, string name, FactKind kind) =>
            (Blackboard, Name, Kind) = (blackboard, name, kind);

        /// <summary>
        /// Creates a fact of type <see cref="FactKind.Weight"/>.
        /// </summary>
        internal static Fact Weight => new(FactKind.Weight);
    }
}
