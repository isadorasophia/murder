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
        Weight,

        /// <summary>
        /// Used when checking for required components.
        /// </summary>
        Component,
        Float,
        Enum,
        Any
    }

    public readonly struct Fact
    {
        /// <summary>
        /// If null, grab the default blackboard.
        /// </summary>
        public readonly string? Blackboard = null;

        public readonly string Name = string.Empty;
        public readonly FactKind Kind = FactKind.Invalid;

        /// <summary>
        /// Set when the fact is of type <see cref="FactKind.Component"/>
        /// </summary>
        public readonly Type? ComponentType = null;

        public readonly string EditorName => Name; //$"{Blackboard}.{AtlasId}";

        public Fact() { }

        private Fact(FactKind kind) => Kind = kind;

        public Fact(Type componentType) => (Kind, ComponentType) = (FactKind.Component, componentType);

        public Fact(string? blackboard, string name, FactKind kind, Type? componentType = null) =>
            (Blackboard, Name, Kind, ComponentType) = (blackboard, name, kind, componentType);

        /// <summary>
        /// Creates a fact of type <see cref="FactKind.Weight"/>.
        /// </summary>
        internal static Fact Weight => new(FactKind.Weight);

        /// <summary>
        /// Creates a fact of type <see cref="FactKind.Component"/>.
        /// </summary>
        internal static Fact Component => new(FactKind.Component);
    }
}