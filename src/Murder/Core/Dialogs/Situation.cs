using Murder.Attributes;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Core.Dialogs
{
    public readonly struct Situation
    {
        [JsonProperty]
        [HideInEditor]
        public readonly int Id = 0;

        public readonly string Name = string.Empty;

        public readonly ImmutableArray<Dialog> Dialogs = ImmutableArray<Dialog>.Empty;

        public Situation() { }

        public Situation(int id, string name) 
        {
            Id = id;
            Name = name;
        }

        public Situation(int id, string name, ImmutableArray<Dialog> dialogs) : this(id, name)
        {
            Dialogs = dialogs;
        }

        public Situation WithName(string name) => new(Id, name, Dialogs);

        public Situation WithNewDialog(Dialog dialog) => new(Id, Name, Dialogs.Add(dialog));

        public Situation WithDialogAt(int index, Dialog dialog) => new(Id, Name, Dialogs.SetItem(index, dialog));

        public Situation ReorderDialogAt(int previousIndex, int newIndex)
        {
            Dialog targetDialog = Dialogs[previousIndex];
            ImmutableArray<Dialog> dialogs = Dialogs.RemoveAt(previousIndex).Insert(newIndex, targetDialog);

            return new(Id, Name, dialogs);
        }

        public Situation RemoveDialogAt(int index) => new(Id, Name, Dialogs.RemoveAt(index));
    }
}
