using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [Unique]
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct LineComponent : IComponent
    {
        public readonly Line Line;
        public readonly float Start = 0;

        /// <summary>
        /// This is the line sequence for the situation.
        /// </summary>
        public readonly int Sequence = 0;

        public LineComponent(Line line, float start, int sequence)
        {
            Line = line;
            Start = start;

            Sequence = sequence;
        }
    }
}