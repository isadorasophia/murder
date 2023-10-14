using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Messages
{

    public readonly struct CollidedWithMessage : IMessage
    {
        public readonly int EntityId;
        /// <summary>
        /// Signals a collision with another entity
        /// </summary>
        /// <param name="entityId">The scene ID of the other entity</param>
        public CollidedWithMessage(int entityId)
        {
            EntityId = entityId;
        }
    }
}