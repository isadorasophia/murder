using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Systems.Graphics
{
    [Messager(typeof(AnimationCompleteMessage))]
    [Filter(typeof(ChangeSpriteBatchOnAnimationCompleteComponent))]
    internal class ChangeSpriteBatchOnAnimationCompleteSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            var msg = (AnimationCompleteMessage)message;
            var component = entity.GetChangeSpriteBatchOnAnimationComplete();

            if(component.OnCompleteStyle == AnimationCompleteStyle.Single)
            {
                ChangeToBatch(entity, component.SpriteBatch);
            }
            else if (component.OnCompleteStyle == msg.CompleteStyle)
            {
                ChangeToBatch(entity, component.SpriteBatch);
            }
        }

        private void ChangeToBatch(Entity entity, int spriteBatch)
        {
            if (entity.TryGetSprite() is SpriteComponent sprite)
            {
                entity.SetSprite(sprite with { TargetSpriteBatch = spriteBatch });
                entity.RemoveChangeSpriteBatchOnAnimationComplete();
            }
        }
    }
}
