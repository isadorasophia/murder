using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using Murder.Utilities;

namespace Murder.Editor.Systems;

[OnlyShowOnDebugView]
[Messager(typeof(AnimationEventMessage))]
internal class AnimationEventDebugSystem : IMessagerSystem
{
    public void OnMessage(World world, Entity entity, IMessage message)
    {
        if (Game.Instance.IsSkippingDeltaTimeOnUpdate)
        {
            return;
        }

        if (world.TryGetUnique<EditorComponent>()?.EditorHook is not EditorHook hook || !hook.DrawAnimationEvents)
        {
            return;
        }

        if (!entity.HasTransform())
        {
            return;
        }

        if (entity.TryFetchParent()?.HasAnimationEventMessage() ?? false)
        {
            // Parent already received the message and this was just propagated, let's not show it twice.
            return;
        }

        AnimationEventMessage msg = (AnimationEventMessage)message;
        EditorDebugServices.DrawText(world, msg.Event, entity.GetGlobalTransform().Vector2 +
            new System.Numerics.Vector2(Game.Random.NextFloat(-5, 5), Game.Random.NextFloat(-5, 5)), 0.5f,
            msg.BroadcastedEvent ? Color.Orange : Color.Green);
    }
}