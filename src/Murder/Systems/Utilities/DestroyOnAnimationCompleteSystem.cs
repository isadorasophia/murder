using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Physics;
using Murder.Messages;
using Murder.Prefabs;
using Murder.Services;

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

            SpriteComponent? sprite = entity.TryGetSprite();
            if (sprite is not null && sprite.Value.NextAnimations.Length > 1)
            {
                return;
            }

            DestroyOnAnimationCompleteComponent destroyOnComplete = entity.GetDestroyOnAnimationComplete();
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
                    entity.SetAlpha(AlphaSources.Alpha, 0);
                    return;

                case DestroyOnAnimationCompleteFlags.None:
                    return;

                case DestroyOnAnimationCompleteFlags.RemoveDeactivateHighlight:
                    entity.RemoveDeactivateHighlightSprite();
                    return;
            }
        }
    }
}