using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Messages;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Systems.Util
{
    [Filter(typeof(DestroyOnAnimationCompleteComponent))]
    [Messager(typeof(AnimationCompleteMessage))]
    [Watch(typeof(AnimationCompleteComponent))]
    internal class DestroyOnAnimationCompleteSystem : IReactiveSystem, IMessagerSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (Entity e in entities)
            {
                Complete(e);
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities) { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities) { }

        public void OnMessage(World world, Entity entity, IMessage message)
        {
            AnimationCompleteMessage msg = (AnimationCompleteMessage)message;
            if (msg.CompleteStyle != AnimationCompleteStyle.Sequence)
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