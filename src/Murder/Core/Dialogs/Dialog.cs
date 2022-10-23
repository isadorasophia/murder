using System.Collections.Immutable;

namespace Murder.Core.Dialogs
{
    public readonly struct Dialog
    {
        /// <summary>
        /// Stop playing this dialog after playing it for an amount of times.
        /// </summary>
        public readonly bool PlayOnce = false;

        public readonly ImmutableArray<Criterion> Requirements = ImmutableArray<Criterion>.Empty;

        public readonly ImmutableArray<Line> Lines = ImmutableArray<Line>.Empty;

        public readonly ImmutableArray<DialogAction>? Actions = null;

        /// <summary>
        /// Go to another dialog with a specified id.
        /// </summary>
        public readonly int? GoTo = null;
        
        public Dialog() { }

        public Dialog(
            bool playOnce,
            ImmutableArray<Criterion> requirements,
            ImmutableArray<Line> lines,
            ImmutableArray<DialogAction>? actions,
            int? @goto) : this()
        {
            PlayOnce = playOnce;
            Requirements = requirements;
            Lines = lines;
            Actions = actions;
            GoTo = @goto;
        }

        public Dialog FlipPlayOnce() => new(!PlayOnce, Requirements, Lines, Actions, GoTo);

        public Dialog AddLine(Line line) => new(PlayOnce, Requirements, Lines.Add(line), Actions, GoTo);

        public Dialog WithLines(ImmutableArray<Line> lines) => new(PlayOnce, Requirements, lines, Actions, GoTo);

        public Dialog SetLineAt(int index, Line line) => new(PlayOnce, Requirements, Lines.SetItem(index, line), Actions, GoTo);

        public Dialog ReorderLineAt(int previousIndex, int newIndex)
        {
            Line targetLine = Lines[previousIndex];
            ImmutableArray<Line> lines = Lines.RemoveAt(previousIndex).Insert(newIndex, targetLine);

            return new(PlayOnce, Requirements, lines, Actions, GoTo);
        }

        public Dialog AddRequirement(Criterion requirement) => new(PlayOnce, Requirements.Add(requirement), Lines, Actions, GoTo);

        public Dialog WithRequirements(ImmutableArray<Criterion> requirements) => new(PlayOnce, requirements, Lines, Actions, GoTo);

        public Dialog WithActions(ImmutableArray<DialogAction>? actions) => new(PlayOnce, Requirements, Lines, actions, GoTo);

        public Dialog WithGoTo(int? @goto) => new(PlayOnce, Requirements, Lines, Actions, @goto);
    }
}
