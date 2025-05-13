using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Core.Input
{
    /// <summary>
    /// Base class for input axis constants, numbers from 100 to 120 are reserved for the engine.
    /// We recomend that if you need to create new constants for more gameplay axis, start at 0.
    /// </summary>
    public class MurderInputAxis
    {
        public const int Movement = 100;
        public const int Ui = 101;
        public const int UiTab = 102;
        public const int EditorCamera = 103;
    }
}