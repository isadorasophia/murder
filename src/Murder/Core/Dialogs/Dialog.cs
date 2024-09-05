using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace Murder.Core.Dialogs
{
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public readonly struct Dialog
    {
        /// <summary>
        /// Stop playing this dialog until this number.
        /// If -1, this will play forever.
        /// </summary>
        public readonly int PlayUntil = -1;

        /// <summary>
        /// Chance of this line actually appearing.
        /// </summary>
        public readonly float Chance = 1;

        public readonly int Id = 0;

        public readonly ImmutableArray<CriterionNode> Requirements = ImmutableArray<CriterionNode>.Empty;

        public readonly ImmutableArray<Line> Lines = ImmutableArray<Line>.Empty;

        public readonly ImmutableArray<DialogAction>? Actions = null;

        /// <summary>
        /// Go to another dialog with a specified id.
        /// </summary>
        public readonly string? GoTo = null;

        public readonly bool IsChoice = false;

        public readonly bool IsExit = false;

        public Dialog() { }

        public Dialog(
            int id,
            int playUntil,
            float chance,
            ImmutableArray<CriterionNode> requirements,
            ImmutableArray<Line> lines,
            ImmutableArray<DialogAction>? actions,
            string? @goto,
            bool isExit,
            bool isChoice) : this()
        {
            Id = id;
            PlayUntil = playUntil;
            Chance = chance;
            Requirements = requirements;
            Lines = lines;
            Actions = actions;
            GoTo = @goto;
            IsChoice = isChoice;
        }

        public Dialog WithActions(ImmutableArray<DialogAction>? actions) => new(Id, PlayUntil, Chance, Requirements, Lines, actions, GoTo, IsExit, IsChoice);

        public Dialog WithLineAt(int index, Line line) => new(Id, PlayUntil, Chance, Requirements, Lines.SetItem(index, line), Actions, GoTo, IsExit, IsChoice);

        public string DebuggerDisplay()
        {
            StringBuilder result = new();
            _ = result.Append(
                $"[{Id}, Requirements = {Requirements.Length}, Lines = {Lines.Length}, Actions = {Actions?.Length ?? 0}]");

            return result.ToString();
        }
    }
}