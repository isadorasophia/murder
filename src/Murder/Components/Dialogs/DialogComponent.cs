using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;

namespace Murder.Components
{
    [DoNotPersistEntityOnSave]
    public readonly struct LineComponent : IComponent
    {
        public readonly Line Line;
        
        public readonly float Start = 0;

        public LineComponent(Line line, float start) 
        {
            Line = line;
            Start = start;
        }
    }
}
