using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Editor.Systems;

[OnlyShowOnDebugView]
[Messager(typeof(AnimationEventMessage))]
internal class AnimationEventDebugSystem : IMessagerSystem
{
    public void OnMessage(World world, Entity entity, IMessage message)
    {
        if (world.TryGetUnique<EditorComponent>()?.EditorHook is not EditorHook hook || !hook.DrawAnimationEvents)
        {
            return;
        }

        if (!entity.HasTransform())
        {
            return;
        }
        
        AnimationEventMessage msg = (AnimationEventMessage)message;
        DebugServices.DrawText(world, msg.Event, entity.GetGlobalTransform().Vector2 + new Core.Geometry.Vector2(Game.Random.NextFloat(-5, 5), Game.Random.NextFloat(-5, 5)), 0.5f);
    }
}

