using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Core
{
    public class MurderTagsBase
    {
        public const int NONE = 0;
        public const int PLAYER = 1 << 0;
        public const int TRIGGER = 1 << 2;

        /// <summary>
        /// This class should never be instanced
        /// </summary>
        public MurderTagsBase()
        {
        }
    }
}
