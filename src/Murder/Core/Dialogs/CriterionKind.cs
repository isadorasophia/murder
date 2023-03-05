
using Murder.Utilities.Attributes;

namespace Murder.Core.Dialogs
{
    public enum CriterionKind
    {
        [CustomName("Is")]
        Is,             // Boolean comparison
        [CustomName("<")]
        Less,           // Integer comparison
        [CustomName("<=")]
        LessOrEqual,    // Integer comparison
        [CustomName("==")]
        Equal,          // Integer comparison
        [CustomName(">")]
        Bigger,         // Integer comparison
        [CustomName(">=")]
        BiggerOrEqual,  // Integer comparison
        [CustomName("Matches")]
        Matches         // String comparison
    }
}
