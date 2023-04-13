using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct ChoiceComponent : IComponent
    {
        public readonly ChoiceLine Choice;
        
        public ChoiceComponent(ChoiceLine choice) 
        {
            Choice = choice;
        }
    }
}
