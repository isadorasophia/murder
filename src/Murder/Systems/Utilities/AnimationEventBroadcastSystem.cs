using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;

namespace Murder.Systems;

[Filter(typeof(AnimationEventBroadcasterComponent))]
[Messager(typeof(AnimationEventMessage))]
public class AnimationEventBroadcastSystem : IMessagerSystem
{
    private readonly HashSet<int> _removeIds = new();
    public void OnMessage(World world, Entity entity, IMessage message)
    {
        var broadcaster = entity.GetAnimationEventBroadcaster();

        _removeIds.Clear();
        foreach (var id in broadcaster.BroadcastTo)
        {
            bool success = false;
            if (world.TryGetEntity(id) is Entity listener)
            {
                if (listener.IsActive)
                {
                    success = true;
                    listener.SendMessage(message);
                }
            }

            if (!success)
                _removeIds.Add(id);
        }
        entity.SetAnimationEventBroadcaster(broadcaster.BroadcastTo.Except(_removeIds));
    }
}
