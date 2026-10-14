using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Messages;
using Murder.Services;

namespace Murder.Systems.Util
{
    [Filter(typeof(DestroyOnAnimationCompleteComponent))]
    [Messager(typeof(AnimationCompleteMessage))]
    internal class DestroyOnAnimationCompleteSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            // This should be message only!
            // Otherwise, we could be setting a component between draw methods.
            // Then, the reactive component will be set after a whole update has completed,
            // in which the animation might have already changed.

            AnimationCompleteMessage msg = (AnimationCompleteMessage)message;
            if (msg.CompleteStyle != AnimationCompleteStyle.Single)
            {
                return;
            }

            Complete(entity);
        }

        private void Complete(Entity entity)
        {
            SpriteComponent? sprite = entity.TryGetSprite();
            if (sprite is not null && sprite.Value.NextAnimations.Length > 1)
            {
                return;
            }

            if (entity.TryGetDestroyOnAnimationComplete() is not DestroyOnAnimationCompleteComponent destroyOnComplete)
            {
                // already dealt with?
                return;
            }

            if (!destroyOnComplete.KeepComponentAfterTriggered)
            {
                entity.RemoveDestroyOnAnimationComplete();
            }

            if (destroyOnComplete.ChangeSpriteBatchOnComplete is int batch && sprite is not null)
            {
                entity.SetSprite(sprite.Value.SetBatch(batch));
            }

            switch (destroyOnComplete.Settings)
            {
                case DestroyOnAnimationCompleteFlags.Deactivate:
                    entity.RemoveDeactivateHighlightSprite();

                    entity.RemoveAnimationComplete();
                    entity.RemoveAnimationStarted();
                    entity.Deactivate();

                    return;

                case DestroyOnAnimationCompleteFlags.RemoveSolid:
                    EffectsServices.RemoveSolid(entity);
                    return;

                case DestroyOnAnimationCompleteFlags.Destroy:
                    entity.Destroy();
                    return;

                case DestroyOnAnimationCompleteFlags.AlphaZero:
                    entity.SetAlpha(0);
                    return;

                case DestroyOnAnimationCompleteFlags.None:
                    return;

                case DestroyOnAnimationCompleteFlags.RemoveDeactivateHighlight:
                    entity.RemoveDeactivateHighlightSprite();
                    return;

                case DestroyOnAnimationCompleteFlags.RemoveSprite:
                    entity.RemoveAgentSprite();
                    entity.RemoveSprite();
                    return;
            }
        }
    }
}