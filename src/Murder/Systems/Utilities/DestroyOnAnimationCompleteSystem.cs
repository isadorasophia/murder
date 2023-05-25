using Murder.Components;
using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Messages;

namespace Murder.Systems.Util
{
    [Filter(typeof(DestroyOnAnimationCompleteComponent))]
    [Messager(typeof(AnimationCompleteMessage))]
    internal class DestroyOnAnimationCompleteSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            if (entity.GetDestroyOnAnimationComplete().DeactivateOnComplete)
            {
                entity.Deactivate();
            }
            else
            {
                entity.Destroy();
            }
        }
    }
}
