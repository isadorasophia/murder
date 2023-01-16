using Bang.Components;
using Bang.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Messages
{
    public readonly struct IsInsideOfMessage : IMessage
    {
        public readonly int InsideOf;
        public IsInsideOfMessage(int insideOf)
        {
            InsideOf = insideOf;
        }
    }
}
