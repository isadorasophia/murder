using Murder.Attributes;
using System.Collections.Immutable;

namespace Murder.Core.Dialogs
{
    public readonly struct Situation
    {
        [Bang.Serialize]
        [HideInEditor]
        public readonly int Id = 0;

        public readonly string Name = string.Empty;

        public readonly ImmutableArray<Dialog> Dialogs = ImmutableArray<Dialog>.Empty;

        public readonly ImmutableDictionary<int, DialogEdge> Edges = ImmutableDictionary<int, DialogEdge>.Empty;

        public Situation() { }

        public Situation(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Situation(int id, string name, ImmutableArray<Dialog> dialogs, ImmutableDictionary<int, DialogEdge> edges)
            : this(id, name)
        {
            Dialogs = dialogs;
            Edges = edges;
        }

        public Situation WithName(string name) => new(Id, name, Dialogs, Edges);

        public Situation WithDialogAt(int index, Dialog dialog) => new(Id, Name, Dialogs.SetItem(index, dialog), Edges);
    }
}