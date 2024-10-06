using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Messages;

namespace Murder.Systems.Util
{
    [Filter(typeof(DestroyOnAnimationCompleteComponent))]
    [Messager(typeof(AnimationCompleteMessage))]
    internal class DestroyOnAnimationCompleteSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            var msg = (AnimationCompleteMessage)message;

            if (msg.CompleteStyle != AnimationCompleteStyle.Sequence)
            {
                return;
            }

            if (entity.TryGetSprite() is SpriteComponent sprite)
            {
                if (sprite.NextAnimations.Length > 1)
                {
                    return;
                }
            }

            if (entity.GetDestroyOnAnimationComplete().DeactivateOnComplete)
            {
                entity.RemoveAnimationComplete();
                entity.RemoveAnimationStarted();
                entity.Deactivate();
            }
            else
            {
                entity.Destroy();
            }
        }
    }
}