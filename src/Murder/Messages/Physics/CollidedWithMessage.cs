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
        public readonly int Layer;
        public readonly int EntityId;
        /// <summary>
        /// Signals a collision with another entity
        /// </summary>
        /// <param name="entityId">The Entity ID of the other entity</param>
        /// <param name="layer">The collision layer of said entity collider</param>
        public CollidedWithMessage(int entityId, int layer)
        {
            EntityId = entityId;
            Layer = layer;
        }
    }
}