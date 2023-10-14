using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Messages;

namespace Murder.Systems.Physics
{
    [Filter(typeof(DestroyOnCollisionComponent))]
    [Messager(typeof(CollidedWithMessage))]
    internal class DestroyOnCollisionSystem : IMessagerSystem
    {
        public void OnMessage(World world, Entity entity, IMessage message)
        {
            if (!entity.IsDestroyed)
            {
                if (entity.GetDestroyOnCollision().KillInstead)
                {
                    entity.SendMessage(new FatalDamageMessage());
                }
                else
                {
                    entity.Destroy();
                }
            }
        }
    }
}