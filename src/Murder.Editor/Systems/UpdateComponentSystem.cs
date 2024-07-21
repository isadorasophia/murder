using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Messages;
using Murder.Editor.Utilities;

namespace Murder.Editor.Systems;

[EditorSystem]
[Messager(typeof(AssetUpdatedMessage))]
public class UpdateComponentSystem : IMessagerSystem
{
    public void OnMessage(World world, Entity entity, IMessage message)
    {
        if (world.TryGetUnique<EditorComponent>() is not EditorComponent editor)
        {
            return;
        }
        EditorHook hook = editor.EditorHook;

        Type type = ((AssetUpdatedMessage)message).UpdatedComponent;

        if (type == typeof(PositionComponent))
        {
            hook.OnComponentModified?.Invoke(entity.EntityId, entity.GetTransform());
        }
        else if (type == typeof(SpriteComponent))
        {
            hook.OnComponentModified?.Invoke(entity.EntityId, entity.GetSprite());
        }
        else if (type == typeof(AgentSpriteComponent))
        {
            hook.OnComponentModified?.Invoke(entity.EntityId, entity.GetAgentSprite());
        }
        else if (type == typeof(ColliderComponent))
        {
            hook.OnComponentModified?.Invoke(entity.EntityId, entity.GetCollider());
        }
        else if (type == typeof(SoundShapeComponent))
        {
            hook.OnComponentModified?.Invoke(entity.EntityId, entity.GetSoundShape());
        }
    }
}